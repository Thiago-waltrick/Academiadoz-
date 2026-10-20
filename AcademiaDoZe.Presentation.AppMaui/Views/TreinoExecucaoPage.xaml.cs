using AcademiaDoZe.Presentation.AppMaui.ViewModels;
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
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.CarregarCommand.ExecuteAsync(null);
        }
    }
}
