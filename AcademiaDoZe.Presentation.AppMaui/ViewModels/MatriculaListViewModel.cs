using System.Collections.ObjectModel;
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

public partial class MatriculaListViewModel : BaseViewModel
{
    private readonly IMatriculaService _matriculaService;
    private string _searchText = string.Empty;

    public ObservableCollection<MatriculaDto> Matriculas { get; } = new();
    public string SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }
    public IAsyncRelayCommand SearchCommand { get; }
    public IAsyncRelayCommand AddCommand { get; }
    public IAsyncRelayCommand<MatriculaDto> EditCommand { get; }
    public IAsyncRelayCommand<MatriculaDto> DeleteCommand { get; }

    [ObservableProperty]
    private string _loadErrorMessage = string.Empty;

    public MatriculaListViewModel(IMatriculaService matriculaService)
    {
        _matriculaService = matriculaService;
        Title = "Matrículas";
        SearchCommand = new AsyncRelayCommand(SearchAsync);
        AddCommand = new AsyncRelayCommand(AddAsync);
        EditCommand = new AsyncRelayCommand<MatriculaDto>(EditAsync);
        DeleteCommand = new AsyncRelayCommand<MatriculaDto>(DeleteAsync);
    }

    [RelayCommand]
    public async Task LoadMatriculasAsync()
    {
        if (IsBusy)
            return;

        IsBusy = true;
        IsRefreshing = true;
        LoadErrorMessage = string.Empty;

        try
        {
            var items = await _matriculaService.ObterTodasAsync();
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Matriculas.Clear();
                foreach (var item in items)
                    Matriculas.Add(item);
            });
        }
        catch (Exception ex)
        {
            LoadErrorMessage = $"Não foi possível carregar as matrículas: {ex.GetBaseException().Message}";
        }
        finally
        {
            IsRefreshing = false;
            IsBusy = false;
        }
    }

    private async Task SearchAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        LoadErrorMessage = string.Empty;
        try
        {
            var items = await _matriculaService.BuscarAsync(SearchText);
            await MainThread.InvokeOnMainThreadAsync(() => ReplaceItems(items));
        }
        catch (Exception ex)
        {
            LoadErrorMessage = $"Não foi possível pesquisar as matrículas: {ex.GetBaseException().Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private Task AddAsync() => Shell.Current.GoToAsync("matricula");

    private Task EditAsync(MatriculaDto? matricula) => matricula is null
        ? Task.CompletedTask
        : Shell.Current.GoToAsync($"matricula?Id={matricula.Id}");

    private async Task DeleteAsync(MatriculaDto? matricula)
    {
        if (matricula is null || IsBusy) return;
        var confirmar = await Shell.Current.DisplayAlertAsync(
            "Excluir matrícula",
            $"Deseja excluir a matrícula #{matricula.Id} de {matricula.Aluno.Nome}?",
            "Excluir",
            "Cancelar");
        if (!confirmar) return;

        IsBusy = true;
        try
        {
            await _matriculaService.RemoverAsync(matricula.Id);
            await Shell.Current.DisplayAlertAsync("Matrícula excluída", "O registro foi removido do SQL Server.", "OK");
            var items = await _matriculaService.BuscarAsync(SearchText);
            await MainThread.InvokeOnMainThreadAsync(() => ReplaceItems(items));
        }
        catch (Exception ex)
        {
            LoadErrorMessage = $"Não foi possível excluir a matrícula: {ex.GetBaseException().Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void ReplaceItems(IEnumerable<MatriculaDto> items)
    {
        Matriculas.Clear();
        foreach (var item in items)
            Matriculas.Add(item);
    }
}
