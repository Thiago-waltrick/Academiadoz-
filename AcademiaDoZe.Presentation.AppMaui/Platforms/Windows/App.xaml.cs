using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace AcademiaDoZe.Presentation.AppMaui.WinUI
{
    public partial class App : MauiWinUIApplication
    {
        public App()
        {
            InitializeComponent();
        }

        protected override MauiApp CreateMauiApp() =>
            global::AcademiaDoZe.Presentation.AppMaui.MauiProgram.CreateMauiApp();
    }
}
