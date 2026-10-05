using AcademiaDoZe.Presentation.AppMaui.ViewModels;
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
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadMatriculasCommand.ExecuteAsync(null);
        }
    }
}
