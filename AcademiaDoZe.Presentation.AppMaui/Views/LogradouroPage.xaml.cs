using AcademiaDoZe.Presentation.AppMaui.ViewModels;
using AcademiaDoZe.Presentation.AppMaui.Services;
using Microsoft.Maui.Controls;

namespace AcademiaDoZe.Presentation.AppMaui.Views
{
    public partial class LogradouroPage : ContentPage
    {
        private readonly LogradouroViewModel _viewModel;

        public LogradouroPage(LogradouroViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = viewModel;
            ThemeToggleOverlay.Attach(this);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.InitializeCommand.ExecuteAsync(null);
        }
    }
}
