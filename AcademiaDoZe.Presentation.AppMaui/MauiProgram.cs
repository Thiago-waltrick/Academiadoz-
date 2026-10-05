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
            builder.Services.AddTransient<LogradouroListViewModel>();
            builder.Services.AddTransient<LogradouroViewModel>();
            builder.Services.AddTransient<ConfigViewModel>();
            builder.Services.AddTransient<TreinoExecucaoViewModel>();

            // Register Views
            builder.Services.AddTransient<ConnectionPage>();
            builder.Services.AddTransient<AppShell>();
            builder.Services.AddTransient<DashboardListPage>();
            builder.Services.AddTransient<MatriculaListPage>();
            builder.Services.AddTransient<LogradouroListPage>();
            builder.Services.AddTransient<LogradouroPage>();
            builder.Services.AddTransient<ConfigPage>();
            builder.Services.AddTransient<TreinoExecucaoPage>();

            Routing.RegisterRoute("logradouro", typeof(LogradouroPage));

            return builder.Build();
        }
    }
}
