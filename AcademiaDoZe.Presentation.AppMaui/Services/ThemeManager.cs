using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using AcademiaDoZe.Presentation.AppMaui.Messages;

namespace AcademiaDoZe.Presentation.AppMaui.Services;

public static class ThemeManager
{
    private const string PreferenceKey = "AppTheme";

    public static AppTheme CurrentTheme
    {
        get
        {
            var application = global::Microsoft.Maui.Controls.Application.Current;
            if (application is null)
                return AppTheme.Light;

            return application.UserAppTheme == AppTheme.Unspecified
                ? application.RequestedTheme
                : application.UserAppTheme;
        }
    }

    public static void Apply(string? theme)
    {
        var value = theme?.Trim().ToLowerInvariant();
        if (global::Microsoft.Maui.Controls.Application.Current is not { } application)
            return;

        application.UserAppTheme = value switch
        {
            "light" => AppTheme.Light,
            "dark" => AppTheme.Dark,
            _ => AppTheme.Unspecified
        };
    }

    public static void Toggle()
    {
        var nextTheme = CurrentTheme == AppTheme.Dark ? AppTheme.Light : AppTheme.Dark;
        var value = nextTheme == AppTheme.Dark ? "Dark" : "Light";

        Preferences.Set(PreferenceKey, value);
        WeakReferenceMessenger.Default.Send(new TemaPreferencesUpdatedMessage(value));
    }
}
