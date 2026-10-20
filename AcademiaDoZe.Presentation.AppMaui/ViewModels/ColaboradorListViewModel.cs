using System.Collections.ObjectModel;
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

public partial class ColaboradorListViewModel : BaseViewModel
{
    private readonly IColaboradorService _colaboradorService;
    private IReadOnlyCollection<ColaboradorDto> _todosColaboradores = Array.Empty<ColaboradorDto>();

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public ObservableCollection<ColaboradorDto> Colaboradores { get; } = new();

    public ColaboradorListViewModel(IColaboradorService colaboradorService)
    {
        _colaboradorService = colaboradorService;
        Title = "Colaboradores";
    }

    partial void OnSearchTextChanged(string value) => AplicarFiltro();

    [RelayCommand]
    public async Task LoadColaboradoresAsync()
    {
        if (IsBusy)
            return;

        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            _todosColaboradores = await _colaboradorService.ObterTodosAsync();
            await MainThread.InvokeOnMainThreadAsync(AplicarFiltro);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Não foi possível carregar os colaboradores: {ex.GetBaseException().Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public void Search() => AplicarFiltro();

    [RelayCommand]
    public async Task RefreshAsync()
    {
        IsRefreshing = true;
        try
        {
            await LoadColaboradoresAsync();
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    public Task AddColaboradorAsync() => Shell.Current.GoToAsync("colaborador");

    [RelayCommand]
    public Task EditColaboradorAsync(ColaboradorDto? colaborador) => colaborador is null
        ? Task.CompletedTask
        : Shell.Current.GoToAsync($"colaborador?Id={colaborador.Id}");

    [RelayCommand]
    public async Task DeleteColaboradorAsync(ColaboradorDto? colaborador)
    {
        if (colaborador is null || IsBusy)
            return;

        var shell = Shell.Current;
        if (shell is null)
            return;

        var confirmed = await shell.DisplayAlertAsync(
            "Excluir colaborador",
            $"Deseja excluir {colaborador.Nome}?",
            "Excluir",
            "Cancelar");
        if (!confirmed)
            return;

        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            await _colaboradorService.RemoverAsync(colaborador.Id);
            _todosColaboradores = _todosColaboradores.Where(item => item.Id != colaborador.Id).ToArray();
            await MainThread.InvokeOnMainThreadAsync(AplicarFiltro);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Não foi possível excluir o colaborador: {ex.GetBaseException().Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void AplicarFiltro()
    {
        var query = SearchText.Trim();
        var filtrados = string.IsNullOrWhiteSpace(query)
            ? _todosColaboradores
            : _todosColaboradores.Where(item =>
                item.Nome.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                item.Cpf.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                item.Email.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                item.Telefone.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                item.Tipo.ToString().Contains(query, StringComparison.OrdinalIgnoreCase) ||
                item.Vinculo.ToString().Contains(query, StringComparison.OrdinalIgnoreCase));

        Colaboradores.Clear();
        foreach (var item in filtrados)
            Colaboradores.Add(item);
    }
}
