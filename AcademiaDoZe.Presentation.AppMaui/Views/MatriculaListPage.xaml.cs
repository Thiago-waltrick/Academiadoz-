using AcademiaDoZe.Presentation.AppMaui.ViewModels;
using AcademiaDoZe.Presentation.AppMaui.Services;
using Microsoft.Maui.Controls;

namespace AcademiaDoZe.Presentation.AppMaui.Views
{
    public partial class MatriculaListPage : ContentPage
    {
        private readonly MatriculaListViewModel _viewModel;

        public MatriculaListPage(MatriculaListViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
            ThemeToggleOverlay.Attach(this);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadMatriculasCommand.ExecuteAsync(null);
        }
    }
}
