using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Storage;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

public partial class ProfileViewModel : ObservableObject
{
    private const string PasswordKey = "DbSenha";
    private const string DisplayNameKey = "ProfileDisplayName";
    private const string ImagePathKey = "ProfileImagePath";

    [ObservableProperty]
    private string _displayName = string.Empty;

    [ObservableProperty]
    private string _sqlUsername = string.Empty; 

    [ObservableProperty]
    private string _sqlPassword = string.Empty;

    [ObservableProperty]
    private bool _isPasswordHidden = true;

    [ObservableProperty]
    private string _passwordVisibilityText = "Mostrar";

    [ObservableProperty]
    private string? _profileImagePath;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    public ProfileViewModel()
    {
        SqlUsername = Preferences.Get("DbUsuario", string.Empty);
        DisplayName = Preferences.Get(DisplayNameKey, SqlUsername);

        var savedImagePath = Preferences.Get(ImagePathKey, string.Empty);
        ProfileImagePath = !string.IsNullOrWhiteSpace(savedImagePath) && File.Exists(savedImagePath)
            ? savedImagePath
            : null;
    }

    public async Task LoadAsync()
    {
        IsPasswordHidden = true;
        PasswordVisibilityText = "Mostrar";

        try
        {
            SqlPassword = await SecureStorage.Default.GetAsync(PasswordKey) ?? string.Empty;
            if (string.IsNullOrEmpty(SqlPassword))
                StatusMessage = "A senha ainda não está salva neste dispositivo. Entre novamente para salvá-la com segurança.";
            else
                StatusMessage = string.Empty;
        }
        catch
        {
            SqlPassword = string.Empty;
            StatusMessage = "Não foi possível acessar a senha protegida neste dispositivo.";
        }
    }

    [RelayCommand]
    private Task SaveProfileAsync()
    {
        var name = DisplayName?.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            StatusMessage = "Informe um nome para o perfil.";
            return Task.CompletedTask;
        }

        DisplayName = name;
        Preferences.Set(DisplayNameKey, name);
        StatusMessage = "Nome do perfil salvo.";
        return Task.CompletedTask;
    }

    [RelayCommand]
    private void TogglePasswordVisibility()
    {
        IsPasswordHidden = !IsPasswordHidden;
        PasswordVisibilityText = IsPasswordHidden ? "Mostrar" : "Ocultar";
    }

    [RelayCommand]
    private async Task ChooseImageAsync()
    {
        try
        {
            var photo = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Escolha uma foto de perfil",
                FileTypes = FilePickerFileType.Images
            });

            if (photo is null)
                return;

            var extension = Path.GetExtension(photo.FileName);
            if (string.IsNullOrWhiteSpace(extension))
                extension = ".img";

            var destinationPath = Path.Combine(
                FileSystem.AppDataDirectory,
                $"profile-{Guid.NewGuid():N}{extension}");

            await using (var source = await photo.OpenReadAsync())
            await using (var destination = File.Create(destinationPath))
                await source.CopyToAsync(destination);

            var previousPath = ProfileImagePath;
            ProfileImagePath = destinationPath;
            Preferences.Set(ImagePathKey, destinationPath);
            StatusMessage = "Foto do perfil atualizada.";

            if (!string.IsNullOrWhiteSpace(previousPath) && File.Exists(previousPath))
                File.Delete(previousPath);
        }
        catch
        {
            StatusMessage = "Não foi possível carregar a imagem escolhida.";
        }
    }
}
