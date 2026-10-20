using AcademiaDoZe.Presentation.AppMaui.ViewModels;
using Microsoft.Maui.Controls;

namespace AcademiaDoZe.Presentation.AppMaui.Views;

[QueryProperty(nameof(MatriculaId), "Id")]
public partial class MatriculaPage : ContentPage
{
    private readonly MatriculaViewModel _viewModel;

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
        int? id = int.TryParse(MatriculaId, out var parsedId) ? parsedId : null;
        await _viewModel.LoadAsync(id);
    }
}
