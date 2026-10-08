using Microsoft.Maui.Controls;
using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using AcademiaDoZe.Presentation.AppMaui.Messages;
using AcademiaDoZe.Presentation.AppMaui.Services;
using AcademiaDoZe.Presentation.AppMaui.Views;

namespace AcademiaDoZe.Presentation.AppMaui
{
    public partial class App : Microsoft.Maui.Controls.Application
    {
        private readonly IServiceProvider _services;

        public App(IServiceProvider services)
        {
            _services = services;
            InitializeComponent();

            ThemeManager.Apply(Microsoft.Maui.Storage.Preferences.Get("AppTheme", "System"));
            WeakReferenceMessenger.Default.Register<TemaPreferencesUpdatedMessage>(this, (r, m) =>
                MainThread.BeginInvokeOnMainThread(() => ThemeManager.Apply(m.Value)));
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(_services.GetRequiredService<ConnectionPage>())
            {
                Title = "Academia do Zé"
            };
        }
    }
}
