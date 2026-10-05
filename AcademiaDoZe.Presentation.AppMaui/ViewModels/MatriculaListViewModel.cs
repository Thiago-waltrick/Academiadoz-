using System.Collections.ObjectModel;
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

public partial class MatriculaListViewModel : BaseViewModel
{
    private readonly IMatriculaService _matriculaService;

    public ObservableCollection<MatriculaDto> Matriculas { get; } = new();

    [ObservableProperty]
    private string _loadErrorMessage = string.Empty;

    public MatriculaListViewModel(IMatriculaService matriculaService)
    {
        _matriculaService = matriculaService;
        Title = "Matrículas";
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
}
