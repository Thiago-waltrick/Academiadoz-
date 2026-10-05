using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Storage;
using System.Threading.Tasks;
using AcademiaDoZe.Presentation.AppMaui.Messages;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels
{
    /// <summary>
    /// ViewModel de configurações da aplicação.
    /// Lê e grava Preferences e notifica via WeakReferenceMessenger.
    /// </summary>
    public partial class ConfigViewModel : ObservableObject
    {
        // Tema: "System", "Light", "Dark"
        [ObservableProperty]
        private string _selectedTheme = "System";

        // Esta aplicação usa exclusivamente SQL Server.
        [ObservableProperty]
        private string _tipoBanco = "SqlServer";

        [ObservableProperty]
        private string _servidor = string.Empty;

        [ObservableProperty]
        private string _bancoDados = string.Empty;

        [ObservableProperty]
        private string _usuario = string.Empty;

        public ConfigViewModel()
        {
            // Carrega preferências iniciais
            SelectedTheme = Preferences.Get("AppTheme", "System");
            TipoBanco = "SqlServer";
            Servidor = Preferences.Get("DbServidor", string.Empty);
            BancoDados = Preferences.Get("DbBanco", string.Empty);
            Usuario = Preferences.Get("DbUsuario", string.Empty);
            Preferences.Remove("DbSenha");
        }

        [RelayCommand]
        public Task SalvarConfiguracoesAsync()
        {
            // Salva em Preferences
            Preferences.Set("AppTheme", SelectedTheme);
            Preferences.Set("TipoBanco", "SqlServer");
            Preferences.Set("DbServidor", Servidor);
            Preferences.Set("DbBanco", BancoDados);
            Preferences.Set("DbUsuario", Usuario);

            // Envia mensagens de atualização
            WeakReferenceMessenger.Default.Send(new TemaPreferencesUpdatedMessage(SelectedTheme));
            WeakReferenceMessenger.Default.Send(new BancoPreferencesUpdatedMessage("SqlServer"));

            return Task.CompletedTask;
        }
    }
}
