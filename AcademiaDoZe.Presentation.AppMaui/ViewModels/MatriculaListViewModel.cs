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
    private CancellationTokenSource? _searchCancellation;

    [ObservableProperty]
    private string _selectedFilter = "Ativas";

    public IReadOnlyList<string> FilterTypes { get; } = new[] { "Id", "Id Aluno", "Ativas", "Vencendo em dias", "Todas" };

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
        await SearchMatriculasAsync();
    }

    [RelayCommand]
    public async Task RefreshAsync() => await SearchMatriculasAsync();

    [RelayCommand(AllowConcurrentExecutions = true)]
    public async Task SearchMatriculasAsync()
    {
        _searchCancellation?.Cancel();
        var source = new CancellationTokenSource();
        _searchCancellation = source;
        var token = source.Token;
        IsBusy = true;
        LoadErrorMessage = string.Empty;
        try
        {
            var filter = SelectedFilter;
            var input = SearchText?.Trim() ?? string.Empty;
            IReadOnlyCollection<MatriculaDto> items;
            if (filter == "Ativas")
            {
                var ativas = await _matriculaService.ObterAtivasAsync(token);
                items = string.IsNullOrEmpty(input)
                    ? ativas
                    : ativas.Where(item => CorrespondeAoTermo(item, input)).ToArray();
            }
            else if (filter == "Todas")
                items = await _matriculaService.BuscarAsync(input, token);
            else if (!int.TryParse(input, out var number) || number < 0 || (filter != "Vencendo em dias" && number == 0))
                throw new ArgumentException("Informe um número válido para o filtro selecionado.");
            else if (filter == "Id")
                items = new[] { await _matriculaService.ObterPorIdAsync(number, token) };
            else if (filter == "Id Aluno")
                items = await _matriculaService.ObterPorAlunoIdAsync(number, token);
            else
                items = await _matriculaService.ObterVencendoEmDiasAsync(number, token);

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                if (!token.IsCancellationRequested)
                    ReplaceItems(items);
            });
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested) { }
        catch (Exception ex)
        {
            if (!token.IsCancellationRequested)
            {
                LoadErrorMessage = $"Não foi possível pesquisar as matrículas: {ex.GetBaseException().Message}";
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    if (!token.IsCancellationRequested) Matriculas.Clear();
                });
            }
        }
        finally
        {
            if (ReferenceEquals(_searchCancellation, source))
            {
                _searchCancellation = null;
                IsBusy = false;
                IsRefreshing = false;
            }
            source.Dispose();
        }
    }

    private Task SearchAsync() => SearchMatriculasAsync();

    private static bool CorrespondeAoTermo(MatriculaDto matricula, string termo)
    {
        if (matricula.Aluno?.Nome?.Contains(termo, StringComparison.OrdinalIgnoreCase) == true ||
            matricula.Id.ToString().Contains(termo, StringComparison.OrdinalIgnoreCase))
            return true;

        if (!termo.All(c => char.IsDigit(c) || c == '.' || c == '-' || char.IsWhiteSpace(c)))
            return false;

        var digitos = new string(termo.Where(char.IsDigit).ToArray());
        var cpf = new string((matricula.Aluno?.Cpf ?? string.Empty).Where(char.IsDigit).ToArray());
        return digitos.Length > 0 && cpf.Contains(digitos, StringComparison.Ordinal);
    }

    [RelayCommand]
    public Task AddMatriculaAsync() => AddAsync();

    [RelayCommand]
    public Task EditMatriculaAsync(MatriculaDto? matricula) => EditAsync(matricula);

    [RelayCommand]
    public Task DeleteMatriculaAsync(MatriculaDto? matricula) => DeleteAsync(matricula);

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
            await SearchMatriculasAsync();
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
