using AcademiaDoZe.Presentation.AppMaui.ViewModels;
using Microsoft.Maui.Controls;

namespace AcademiaDoZe.Presentation.AppMaui.Views;

[QueryProperty(nameof(AlunoId), "Id")]
public partial class AlunoPage : ContentPage
{
    private readonly AlunoViewModel _viewModel;
    public string? AlunoId { get; set; }

    public AlunoPage(AlunoViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.AlunoId = int.TryParse(AlunoId, out var id) ? id : 0;
        await _viewModel.InitializeAsync();
    }
}
