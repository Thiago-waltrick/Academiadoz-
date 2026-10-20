using AcademiaDoZe.Presentation.AppMaui.ViewModels;
using Microsoft.Maui.Controls;

namespace AcademiaDoZe.Presentation.AppMaui.Views;

[QueryProperty(nameof(ColaboradorId), "Id")]
public partial class ColaboradorPage : ContentPage
{
    private readonly ColaboradorViewModel _viewModel;
    public string? ColaboradorId { get; set; }

    public ColaboradorPage(ColaboradorViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.ColaboradorId = int.TryParse(ColaboradorId, out var id) ? id : 0;
        await _viewModel.InitializeAsync();
    }
}
