using AcademiaDoZe.Presentation.AppMaui.ViewModels;
using AcademiaDoZe.Presentation.AppMaui.Services;
using Microsoft.Maui.Controls;

namespace AcademiaDoZe.Presentation.AppMaui.Views
{
    public partial class TreinoExecucaoPage : ContentPage
    {
        private readonly TreinoExecucaoViewModel _viewModel;

        public TreinoExecucaoPage(TreinoExecucaoViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = viewModel;
            ThemeToggleOverlay.Attach(this);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.CarregarCommand.ExecuteAsync(null);
        }
    }
}
