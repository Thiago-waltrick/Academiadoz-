using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using AcademiaDoZe.Presentation.AppMaui.Messages;

namespace AcademiaDoZe.Presentation.AppMaui.Services;

public static class ThemeToggleOverlay
{
    public static void Attach(ContentPage page, double bottomMargin = 20)
    {
        if (page.Content is not { } content)
            return;

        page.Content = null;

        var layout = new Grid();
        layout.Children.Add(content);
        layout.Children.Add(new ThemeToggleButton(bottomMargin));
        page.Content = layout;
    }

    private sealed class ThemeToggleButton : Button
    {
        public ThemeToggleButton(double bottomMargin)
        {
            AutomationId = "ThemeToggleButton";
            WidthRequest = 56;
            HeightRequest = 56;
            CornerRadius = 28;
            Padding = 14;
            BackgroundColor = Color.FromArgb("#7C3AED");
            HorizontalOptions = LayoutOptions.End;
            VerticalOptions = LayoutOptions.End;
            Margin = new Thickness(0, 0, 20, bottomMargin);
            ZIndex = 10;
            Clicked += (_, _) => ThemeManager.Toggle();
            WeakReferenceMessenger.Default.Register<TemaPreferencesUpdatedMessage>(this, static (recipient, _) =>
            {
                if (recipient is ThemeToggleButton button)
                    button.Dispatcher.Dispatch(button.UpdateIcon);
            });
            UpdateIcon();
        }

        private void UpdateIcon()
        {
            var isDark = ThemeManager.CurrentTheme == AppTheme.Dark;
            ImageSource = new FontImageSource
            {
                FontFamily = "MaterialIcons",
                Glyph = isDark ? "\uE518" : "\uE51C",
                Color = Colors.White,
                Size = 24
            };

            SemanticProperties.SetDescription(this, isDark ? "Ativar tema claro" : "Ativar tema escuro");
        }
    }
}
