using AcademiaDoZe.Application;
using AcademiaDoZe.Infrastructure.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.SqlClient;
using Microsoft.Maui.Storage;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

public partial class ConnectionViewModel : ObservableObject
{
    private readonly RepositoryConfig _repositoryConfig;

    [ObservableProperty]
    private string _server = Preferences.Get("DbServidor", "localhost");

    [ObservableProperty]
    private string _database = Preferences.Get("DbBanco", "db_academia_do_ze");

    [ObservableProperty]
    private string _username = Preferences.Get("DbUsuario", "sa");

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private bool _isConnecting;

    public event EventHandler? ConnectionSucceeded;

    public ConnectionViewModel(RepositoryConfig repositoryConfig)
    {
        _repositoryConfig = repositoryConfig;
        Preferences.Remove("DbSenha");

        if (string.Equals(Database, "AcademiaDoZe", StringComparison.OrdinalIgnoreCase))
        {
            Database = "db_academia_do_ze";
            Preferences.Set("DbBanco", Database);
        }
    }

    [RelayCommand]
    private async Task ConnectAsync()
    {
        if (IsConnecting)
            return;

        if (string.IsNullOrWhiteSpace(Server) ||
            string.IsNullOrWhiteSpace(Database) ||
            string.IsNullOrWhiteSpace(Username) ||
            string.IsNullOrWhiteSpace(Password))
        {
            StatusMessage = "Preencha servidor, banco, usuário e senha.";
            return;
        }

        IsConnecting = true;
        StatusMessage = string.Empty;

        try
        {
            var connectionString = new SqlConnectionStringBuilder
            {
                DataSource = Server.Trim(),
                InitialCatalog = Database.Trim(),
                UserID = Username.Trim(),
                Password = Password,
                IntegratedSecurity = false,
                Encrypt = true,
                TrustServerCertificate = true,
                ConnectTimeout = 5
            }.ConnectionString;

            var provider = new DbProvider("Microsoft.Data.SqlClient", connectionString);
            await provider.ExecuteScalarAsync("SELECT 1");

            _repositoryConfig.DatabaseType = AcademiaDoZe.Application.Enums.AppDatabaseType.SqlServer;
            _repositoryConfig.ConnectionString = connectionString;
            Preferences.Set("DbServidor", Server.Trim());
            Preferences.Set("DbBanco", Database.Trim());
            Preferences.Set("DbUsuario", Username.Trim());
            Password = string.Empty;
            ConnectionSucceeded?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Não foi possível conectar: {ex.GetBaseException().Message}";
        }
        finally
        {
            IsConnecting = false;
        }
    }
}
