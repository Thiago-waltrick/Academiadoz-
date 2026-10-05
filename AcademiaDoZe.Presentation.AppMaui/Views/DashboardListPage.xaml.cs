using System;
using AcademiaDoZe.Presentation.AppMaui.ViewModels;
using Microsoft.Maui.Controls;

namespace AcademiaDoZe.Presentation.AppMaui.Views
{
    public partial class DashboardListPage : ContentPage
    {
        private readonly DashboardListViewModel _viewModel;

        public DashboardListPage(DashboardListViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadDashboardDataCommand.ExecuteAsync(null);
        }
    }
}
