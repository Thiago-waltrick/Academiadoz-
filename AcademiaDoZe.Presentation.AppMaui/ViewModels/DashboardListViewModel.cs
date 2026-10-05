using System;
using System.Threading;
using System.Threading.Tasks;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

public partial class DashboardListViewModel : BaseViewModel
{
    private readonly ILogradouroService _logradouroService;
    private readonly IAlunoService _alunoService;
    private readonly IColaboradorService _colaboradorService;
    private readonly IMatriculaService _matriculaService;

    [ObservableProperty] private int _totalLogradouros;
    [ObservableProperty] private int _totalAlunos;
    [ObservableProperty] private int _totalColaboradores;
    [ObservableProperty] private int _totalMatriculas;
    [ObservableProperty] private string _loadErrorMessage = string.Empty;

    public DashboardListViewModel(
        ILogradouroService logradouroService,
        IAlunoService alunoService,
        IColaboradorService colaboradorService,
        IMatriculaService matriculaService)
    {
        _logradouroService = logradouroService;
        _alunoService = alunoService;
        _colaboradorService = colaboradorService;
        _matriculaService = matriculaService;
        Title = "Dashboard";
    }

    [RelayCommand]
    public async Task LoadDashboardDataAsync(CancellationToken cancellationToken = default)
    {
        if (IsBusy) return;
        IsBusy = true;
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(5));
        try
        {
            LoadErrorMessage = string.Empty;
            var logradourosTask = _logradouroService.ObterTodosAsync(timeout.Token);
            var alunosTask = _alunoService.ObterTodosAsync(timeout.Token);
            var colaboradoresTask = _colaboradorService.ObterTodosAsync(timeout.Token);
            var matriculasTask = _matriculaService.ObterTodasAsync(timeout.Token);
            await Task.WhenAll(logradourosTask, alunosTask, colaboradoresTask, matriculasTask);

            TotalLogradouros = logradourosTask.Result.Count;
            TotalAlunos = alunosTask.Result.Count;
            TotalColaboradores = colaboradoresTask.Result.Count;
            TotalMatriculas = matriculasTask.Result.Count;
        }
        catch (Exception ex)
        {
            LoadErrorMessage = $"Não foi possível carregar os dados do painel: {ex.GetBaseException().Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public Task NavigateToLogradourosAsync() => Shell.Current.GoToAsync("//logradouros");

    [RelayCommand]
    public Task NavigateToMatriculasAsync() => Shell.Current.GoToAsync("//matriculas");

    [RelayCommand]
    public Task NavigateToTreinoAsync() => Shell.Current.GoToAsync("//treino");
}
