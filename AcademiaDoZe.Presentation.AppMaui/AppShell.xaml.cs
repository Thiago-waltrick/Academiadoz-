using Microsoft.Maui.Controls;
using Microsoft.Extensions.DependencyInjection;
using AcademiaDoZe.Presentation.AppMaui.Views;

namespace AcademiaDoZe.Presentation.AppMaui
{
    public partial class AppShell : Shell
    {
        private readonly IServiceProvider _services;

        public AppShell(IServiceProvider services)
        {
            InitializeComponent();
            _services = services;
            DashboardShellContent.Content = services.GetRequiredService<DashboardListPage>();
            LogradourosShellContent.Content = services.GetRequiredService<LogradouroListPage>();
            TreinoShellContent.Content = services.GetRequiredService<TreinoExecucaoPage>();
            MatriculasShellContent.Content = services.GetRequiredService<MatriculaListPage>();
            ProfileShellContent.Content = services.GetRequiredService<ProfilePage>();
            ConfigShellContent.Content = services.GetRequiredService<ConfigPage>();
        }

        private void OnLogoutClicked(object? sender, EventArgs e)
        {
            var window = global::Microsoft.Maui.Controls.Application.Current?.Windows.FirstOrDefault();
            if (window is not null)
                window.Page = _services.GetRequiredService<ConnectionPage>();
        }
    }
}
