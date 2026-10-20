using System.Collections.ObjectModel;
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

public partial class AlunoListViewModel : BaseViewModel
{
    private readonly IAlunoService _alunoService;
    private IReadOnlyCollection<AlunoDto> _todosAlunos = Array.Empty<AlunoDto>();

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public ObservableCollection<AlunoDto> Alunos { get; } = new();

    public AlunoListViewModel(IAlunoService alunoService)
    {
        _alunoService = alunoService;
        Title = "Alunos";
    }

    partial void OnSearchTextChanged(string value) => AplicarFiltro();

    [RelayCommand]
    public async Task LoadAlunosAsync()
    {
        if (IsBusy)
            return;

        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            _todosAlunos = await _alunoService.ObterTodosAsync();
            await MainThread.InvokeOnMainThreadAsync(AplicarFiltro);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Não foi possível carregar os alunos: {ex.GetBaseException().Message}";
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
            await LoadAlunosAsync();
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    public Task AddAlunoAsync() => Shell.Current.GoToAsync("aluno");

    [RelayCommand]
    public Task EditAlunoAsync(AlunoDto? aluno) => aluno is null
        ? Task.CompletedTask
        : Shell.Current.GoToAsync($"aluno?Id={aluno.Id}");

    [RelayCommand]
    public async Task DeleteAlunoAsync(AlunoDto? aluno)
    {
        if (aluno is null || IsBusy || Shell.Current is not { } shell)
            return;

        var confirmed = await shell.DisplayAlertAsync("Excluir aluno", $"Deseja excluir {aluno.Nome}?", "Excluir", "Cancelar");
        if (!confirmed)
            return;

        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            await _alunoService.RemoverAsync(aluno.Id);
            _todosAlunos = _todosAlunos.Where(item => item.Id != aluno.Id).ToArray();
            await MainThread.InvokeOnMainThreadAsync(AplicarFiltro);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Não foi possível excluir o aluno: {ex.GetBaseException().Message}";
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
            ? _todosAlunos
            : _todosAlunos.Where(item =>
                item.Nome.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                item.Cpf.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                item.Email.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                item.Telefone.Contains(query, StringComparison.OrdinalIgnoreCase));

        Alunos.Clear();
        foreach (var item in filtrados)
            Alunos.Add(item);
    }
}
