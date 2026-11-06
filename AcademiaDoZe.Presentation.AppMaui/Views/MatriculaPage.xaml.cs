using AcademiaDoZe.Presentation.AppMaui.ViewModels;
using Microsoft.Maui.Controls;

namespace AcademiaDoZe.Presentation.AppMaui.Views;

[QueryProperty(nameof(MatriculaId), "Id")]
public partial class MatriculaPage : ContentPage
{
    private readonly MatriculaViewModel _viewModel;
    private bool _initialized;

    public string? MatriculaId { get; set; }

    public MatriculaPage(MatriculaViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_initialized) return;
        _initialized = true;
        int? id = int.TryParse(MatriculaId, out var parsedId) ? parsedId : null;
        await _viewModel.LoadAsync(id);
    }

    private void PlanoPicker_SelectedIndexChanged(object? sender, EventArgs e) => _viewModel.CalculateFimFromPlanoAndInicio();

    private void InicioDatePicker_DateSelected(object? sender, DateChangedEventArgs e) => _viewModel.CalculateFimFromPlanoAndInicio();
}
