using Microsoft.Maui.Controls;
using AcademiaDoZe.Presentation.AppMaui.ViewModels;
using AcademiaDoZe.Presentation.AppMaui.Services;

namespace AcademiaDoZe.Presentation.AppMaui.Views
{
    public partial class ConfigPage : ContentPage
    {
        public ConfigPage(ConfigViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            ThemeToggleOverlay.Attach(this);
        }
    }
}
