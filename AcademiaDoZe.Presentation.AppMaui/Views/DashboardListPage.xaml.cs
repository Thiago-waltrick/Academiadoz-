using System;
using AcademiaDoZe.Presentation.AppMaui.ViewModels;
using AcademiaDoZe.Presentation.AppMaui.Services;
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
            ThemeToggleOverlay.Attach(this);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadDashboardDataCommand.ExecuteAsync(null);
        }
    }
}
