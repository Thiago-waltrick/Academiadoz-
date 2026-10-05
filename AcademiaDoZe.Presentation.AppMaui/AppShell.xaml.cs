using Microsoft.Maui.Controls;
using Microsoft.Extensions.DependencyInjection;
using AcademiaDoZe.Presentation.AppMaui.Views;

namespace AcademiaDoZe.Presentation.AppMaui
{
    public partial class AppShell : Shell
    {
        public AppShell(IServiceProvider services)
        {
            InitializeComponent();
            DashboardShellContent.Content = services.GetRequiredService<DashboardListPage>();
            LogradourosShellContent.Content = services.GetRequiredService<LogradouroListPage>();
            TreinoShellContent.Content = services.GetRequiredService<TreinoExecucaoPage>();
            MatriculasShellContent.Content = services.GetRequiredService<MatriculaListPage>();
        }
    }
}
