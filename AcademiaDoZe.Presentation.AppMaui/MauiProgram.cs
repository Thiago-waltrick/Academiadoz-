using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using AcademiaDoZe.Presentation.AppMaui.Configuration;
using AcademiaDoZe.Presentation.AppMaui.ViewModels;
using AcademiaDoZe.Presentation.AppMaui.Views;

namespace AcademiaDoZe.Presentation.AppMaui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts => {
                    fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");
                });

            // Configure app services and repositories
            ConfigurationHelper.ConfigureServices(builder.Services);

            // Register ViewModels
            builder.Services.AddTransient<ConnectionViewModel>();
            builder.Services.AddTransient<DashboardListViewModel>();
            builder.Services.AddTransient<MatriculaListViewModel>();
            builder.Services.AddTransient<MatriculaViewModel>();
            builder.Services.AddTransient<LogradouroListViewModel>();
            builder.Services.AddTransient<LogradouroViewModel>();
            builder.Services.AddTransient<ConfigViewModel>();
            builder.Services.AddTransient<TreinoExecucaoViewModel>();
            builder.Services.AddTransient<ProfileViewModel>();

            // Register Views
            builder.Services.AddTransient<ConnectionPage>();
            builder.Services.AddTransient<AppShell>();
            builder.Services.AddTransient<DashboardListPage>();
            builder.Services.AddTransient<MatriculaListPage>();
            builder.Services.AddTransient<MatriculaPage>();
            builder.Services.AddTransient<LogradouroListPage>();
            builder.Services.AddTransient<LogradouroPage>();
            builder.Services.AddTransient<ConfigPage>();
            builder.Services.AddTransient<TreinoExecucaoPage>();
            builder.Services.AddTransient<ProfilePage>();

            Routing.RegisterRoute("logradouro", typeof(LogradouroPage));
            Routing.RegisterRoute("matricula", typeof(MatriculaPage));

            return builder.Build();
        }
#if IOS || MACCATALYST
    [Foundation.Register("AppDelegate")]
    public class AppDelegate : Microsoft.Maui.MauiUIApplicationDelegate
    {
        protected override Microsoft.Maui.Hosting.MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }

    public static class Program
    {
        public static void Main(string[] args) =>
            UIKit.UIApplication.Main(args, null, typeof(AppDelegate));
    }
#endif
}
}
