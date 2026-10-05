using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Domain.ValueObjects;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels
{
    [QueryProperty(nameof(LogradouroId), "Id")]
    public partial class LogradouroViewModel : BaseViewModel
    {
        private readonly ILogradouroService _logradouroService;

        [ObservableProperty]
        private LogradouroDto _logradouro = new();

        [ObservableProperty]
        private int _logradouroId;

        [ObservableProperty]
        private bool _isEditMode;

        partial void OnLogradouroIdChanged(int value)
        {
            if (value > 0) IsEditMode = true;
        }

        public LogradouroViewModel(ILogradouroService logradouroService)
        {
            _logradouroService = logradouroService;
            Title = "Logradouro";
        }

        [RelayCommand]
        public async Task InitializeAsync()
        {
            if (LogradouroId <= 0)
            {
                IsEditMode = false;
                Logradouro = new LogradouroDto();
                return;
            }

            IsBusy = true;
            try
            {
                var item = await _logradouroService.ObterPorIdAsync(LogradouroId);
                if (item is null)
                {
                    await ShowAlertAsync("Não encontrado", "O logradouro solicitado não existe mais.");
                    return;
                }

                Logradouro = item;
                IsEditMode = true;
            }
            catch (Exception ex)
            {
                await ShowAlertAsync("Erro", $"Não foi possível carregar o logradouro. {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task SearchByCepAsync()
        {
            var cep = Cep.Criar(Logradouro.Cep);
            if (cep.IsFailure)
            {
                await ShowAlertAsync("CEP inválido", string.Join(Environment.NewLine, cep.Notifications.Select(n => n.Mensagem)));
                return;
            }

            IsBusy = true;
            try
            {
                var result = await _logradouroService.BuscarPorCepAsync(cep.Value.Codigo);
                var found = result.FirstOrDefault();
                if (found is null)
                {
                    await ShowAlertAsync("CEP não encontrado", "Não há logradouro cadastrado para esse CEP.");
                    return;
                }

                Logradouro = new LogradouroDto
                {
                    Id = Logradouro.Id,
                    Cep = found.Cep,
                    Nome = found.Nome,
                    Bairro = found.Bairro,
                    Cidade = found.Cidade,
                    Estado = found.Estado
                };
            }
            catch (Exception ex)
            {
                await ShowAlertAsync("Erro", $"Não foi possível buscar o CEP. {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task SaveLogradouroAsync()
        {
            var errors = ValidateLogradouro(Logradouro);
            if (errors.Count != 0)
            {
                await ShowAlertAsync("Verifique os dados", string.Join(Environment.NewLine, errors));
                return;
            }

            IsBusy = true;
            try
            {
                if (IsEditMode)
                {
                    Logradouro.Id = LogradouroId;
                    await _logradouroService.AtualizarAsync(Logradouro);
                }
                else
                {
                    await _logradouroService.CriarAsync(Logradouro);
                }

                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await ShowAlertAsync("Erro ao salvar", ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public Task CancelAsync() => Shell.Current.GoToAsync("..");

        public IReadOnlyList<string> ValidateLogradouro(LogradouroDto value)
        {
            ArgumentNullException.ThrowIfNull(value);
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(value.Cep) || Cep.Criar(value.Cep).IsFailure)
                errors.Add("CEP deve conter 8 dígitos.");
            if (string.IsNullOrWhiteSpace(value.Nome)) errors.Add("Rua é obrigatória.");
            if (string.IsNullOrWhiteSpace(value.Bairro)) errors.Add("Bairro é obrigatório.");
            if (string.IsNullOrWhiteSpace(value.Cidade)) errors.Add("Cidade é obrigatória.");
            if (string.IsNullOrWhiteSpace(value.Estado) || value.Estado.Trim().Length != 2)
                errors.Add("Estado deve ser a sigla UF com 2 letras.");

            return errors;
        }

        private static Task ShowAlertAsync(string title, string message) =>
            Shell.Current is { } shell
                ? shell.DisplayAlertAsync(title, message, "OK")
                : Task.CompletedTask;
    }
}
