using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels
{
    public partial class LogradouroListViewModel : BaseViewModel
    {
        private readonly ILogradouroService _logradouroService;

        public ObservableCollection<string> FilterTypes { get; } = new() { "Cidade", "Id", "Cep" };
        public ObservableCollection<LogradouroDto> Logradouros { get; } = new();

        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private string _selectedFilterType = "Cidade";

        public LogradouroListViewModel(ILogradouroService logradouroService)
        {
            _logradouroService = logradouroService;
            Title = "Logradouros";
        }

        [RelayCommand]
        public async Task LoadLogradourosAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var items = await _logradouroService.ObterTodosAsync();
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Logradouros.Clear();
                    foreach (var item in items) Logradouros.Add(item);
                });
            }
            catch (Exception ex)
            {
                await ShowAlertAsync("Erro", $"Não foi possível carregar os logradouros. {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task SearchLogradourosAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var all = await _logradouroService.ObterTodosAsync();
                var query = SearchText.Trim();
                var matches = all.Where(item => string.IsNullOrEmpty(query) || SelectedFilterType switch
                {
                    "Id" => item.Id.ToString().Contains(query, StringComparison.OrdinalIgnoreCase),
                    "Cep" => item.Cep?.Contains(query, StringComparison.OrdinalIgnoreCase) == true,
                    _ => item.Cidade.Contains(query, StringComparison.OrdinalIgnoreCase)
                }).ToArray();

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Logradouros.Clear();
                    foreach (var item in matches) Logradouros.Add(item);
                });
            }
            catch (Exception ex)
            {
                await ShowAlertAsync("Erro", $"Não foi possível pesquisar logradouros. {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public Task AddLogradouroAsync() => Shell.Current.GoToAsync("logradouro");

        [RelayCommand]
        public Task EditLogradouroAsync(LogradouroDto logradouro)
        {
            ArgumentNullException.ThrowIfNull(logradouro);
            return Shell.Current.GoToAsync($"logradouro?Id={logradouro.Id}");
        }

        [RelayCommand]
        public async Task DeleteLogradouroAsync(LogradouroDto logradouro)
        {
            ArgumentNullException.ThrowIfNull(logradouro);
            var shell = Shell.Current;
            if (shell is null) return;

            var confirmed = await shell.DisplayAlertAsync("Confirmar exclusão", $"Excluir '{logradouro.Nome}'?", "Excluir", "Cancelar");
            if (!confirmed) return;

            try
            {
                await _logradouroService.RemoverAsync(logradouro.Id);
                await MainThread.InvokeOnMainThreadAsync(() => Logradouros.Remove(logradouro));
            }
            catch (Exception ex)
            {
                var detail = ex.Message.Contains("FOREIGN KEY", StringComparison.OrdinalIgnoreCase)
                    ? "O logradouro está associado a outros registros e não pode ser excluído."
                    : $"Não foi possível excluir o logradouro. {ex.Message}";
                await ShowAlertAsync("Erro ao excluir", detail);
            }
        }

        [RelayCommand]
        public async Task RefreshAsync()
        {
            IsRefreshing = true;
            try
            {
                await LoadLogradourosAsync();
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        private static Task ShowAlertAsync(string title, string message) =>
            Shell.Current is { } shell
                ? shell.DisplayAlertAsync(title, message, "OK")
                : Task.CompletedTask;
    }
}
