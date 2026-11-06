using AcademiaDoZe.Application.DTOs;
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

        private async void OnMenuClicked(object? sender, EventArgs e)
        {
            if (sender is not Button { BindingContext: MatriculaDto matricula }) return;
            var action = await DisplayActionSheetAsync("Matrícula", "Cancelar", null, "Editar", "Excluir");
            if (action == "Editar") await _viewModel.EditMatriculaCommand.ExecuteAsync(matricula);
            else if (action == "Excluir") await _viewModel.DeleteMatriculaCommand.ExecuteAsync(matricula);
        }
    }
}
