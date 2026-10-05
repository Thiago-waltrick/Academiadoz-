using Microsoft.Maui.Controls;
using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using AcademiaDoZe.Presentation.AppMaui.Messages;
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

            // Subscribe to theme changes
            WeakReferenceMessenger.Default.Register<TemaPreferencesUpdatedMessage>(this, (r, m) =>
            {
                var val = m.Value?.ToLowerInvariant() ?? "system";
                if (val == "light") global::Microsoft.Maui.Controls.Application.Current.UserAppTheme = AppTheme.Light;
                else if (val == "dark") global::Microsoft.Maui.Controls.Application.Current.UserAppTheme = AppTheme.Dark;
                else global::Microsoft.Maui.Controls.Application.Current.UserAppTheme = AppTheme.Unspecified;
            });
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(_services.GetRequiredService<ConnectionPage>());
        }
    }
}
