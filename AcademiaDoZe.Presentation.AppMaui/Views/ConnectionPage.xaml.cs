using AcademiaDoZe.Presentation.AppMaui.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;

namespace AcademiaDoZe.Presentation.AppMaui.Views
{
    public partial class ConnectionPage : ContentPage
    {
        private readonly IServiceProvider _services;

        public ConnectionPage(ConnectionViewModel viewModel, IServiceProvider services)
        {
            InitializeComponent();
            _services = services;
            BindingContext = viewModel;
            viewModel.ConnectionSucceeded += OnConnectionSucceeded;
        }

        private void OnConnectionSucceeded(object? sender, EventArgs e)
        {
            var window = global::Microsoft.Maui.Controls.Application.Current?.Windows.FirstOrDefault();
            if (window is not null)
                window.Page = _services.GetRequiredService<AppShell>();
        }
    }
}
