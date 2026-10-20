using System.IO;
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;
using CepValueObject = AcademiaDoZe.Domain.ValueObjects.Cep;
using CpfValueObject = AcademiaDoZe.Domain.ValueObjects.Cpf;
using EmailValueObject = AcademiaDoZe.Domain.ValueObjects.Email;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

[QueryProperty(nameof(AlunoId), "Id")]
public partial class AlunoViewModel : BaseViewModel
{
    private readonly IAlunoService _alunoService;
    private readonly ILogradouroService _logradouroService;

    [ObservableProperty]
    private int _alunoId;

    [ObservableProperty]
    private AlunoDto _aluno = new();

    [ObservableProperty]
    private LogradouroDto _endereco = new();

    [ObservableProperty]
    private string _cpfBusca = string.Empty;

    [ObservableProperty]
    private DateOnly _dataNascimento = DateOnly.FromDateTime(DateTime.Today.AddYears(-18));

    [ObservableProperty]
    private bool _hasEnderecoVinculado;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public AlunoViewModel(IAlunoService alunoService, ILogradouroService logradouroService)
    {
        _alunoService = alunoService;
        _logradouroService = logradouroService;
        Title = "Aluno";
    }

    partial void OnEnderecoChanged(LogradouroDto value) =>
        HasEnderecoVinculado = !string.IsNullOrWhiteSpace(value?.Nome);

    [RelayCommand]
    public async Task InitializeAsync()
    {
        ErrorMessage = string.Empty;
        if (AlunoId <= 0)
        {
            Aluno = new AlunoDto { DataNascimento = DataNascimento.ToDateTime(TimeOnly.MinValue) };
            Endereco = new LogradouroDto();
            return;
        }

        IsBusy = true;
        try
        {
            AplicarAluno(await _alunoService.ObterPorIdAsync(AlunoId));
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Não foi possível carregar o aluno: {ex.GetBaseException().Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task SearchByCpfAsync()
    {
        var cpf = CpfValueObject.Criar(CpfBusca);
        if (cpf.IsFailure)
        {
            ErrorMessage = "CPF deve conter 11 dígitos válidos.";
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            var item = await _alunoService.ObterPorCpfAsync(cpf.Value.Valor);
            AplicarAluno(item);
            AlunoId = item.Id;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Não foi possível localizar o aluno: {ex.GetBaseException().Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task SearchByCepAsync()
    {
        var cep = CepValueObject.Criar(Endereco.Cep);
        if (cep.IsFailure)
        {
            ErrorMessage = "CEP deve conter 8 dígitos.";
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            var item = (await _logradouroService.BuscarPorCepAsync(cep.Value.Codigo)).FirstOrDefault();
            if (item is null)
            {
                ErrorMessage = "Não há endereço cadastrado para esse CEP.";
                return;
            }

            Endereco = item;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Não foi possível buscar o CEP: {ex.GetBaseException().Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task SaveAlunoAsync()
    {
        var validation = Validate();
        if (validation.Count != 0)
        {
            ErrorMessage = string.Join(Environment.NewLine, validation);
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            Aluno.Cpf = CpfValueObject.Criar(Aluno.Cpf).Value.Valor;
            Aluno.Email = EmailValueObject.Criar(Aluno.Email).Value.Endereco;
            Aluno.DataNascimento = DataNascimento.ToDateTime(TimeOnly.MinValue);
            Aluno.Endereco = Endereco;

            if (AlunoId > 0)
                await _alunoService.AtualizarAsync(Aluno);
            else
                await _alunoService.CriarAsync(Aluno);

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Não foi possível salvar o aluno: {ex.GetBaseException().Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task SelecionarFotoAsync()
    {
        var shell = Shell.Current;
        if (shell is null)
            return;

        try
        {
            var action = await shell.DisplayActionSheetAsync("Foto do aluno", "Cancelar", null, "Galeria", "Câmera", "Remover Foto");
            if (action == "Remover Foto")
            {
                Aluno.FotoConteudo = null;
                OnPropertyChanged(nameof(Aluno));
                return;
            }

            FileResult? photo = action switch
            {
                "Galeria" => await FilePicker.Default.PickAsync(new PickOptions { PickerTitle = "Escolha uma foto", FileTypes = FilePickerFileType.Images }),
                "Câmera" => await CapturarFotoAsync(),
                _ => null
            };
            if (photo is null)
                return;

            await using var stream = await photo.OpenReadAsync();
            using var memory = new MemoryStream();
            await stream.CopyToAsync(memory);
            Aluno.FotoConteudo = memory.ToArray();
            OnPropertyChanged(nameof(Aluno));
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Não foi possível selecionar a foto: {ex.GetBaseException().Message}";
        }
    }

    [RelayCommand]
    public Task CancelAsync() => Shell.Current.GoToAsync("..");

    private async Task<FileResult?> CapturarFotoAsync()
    {
        if (!MediaPicker.Default.IsCaptureSupported)
        {
            ErrorMessage = "A captura de fotos não está disponível neste dispositivo.";
            return null;
        }

        var permission = await Permissions.RequestAsync<Permissions.Camera>();
        if (permission != PermissionStatus.Granted)
        {
            ErrorMessage = "A permissão da câmera é necessária para tirar uma foto.";
            return null;
        }

        return await MediaPicker.Default.CapturePhotoAsync();
    }

    private void AplicarAluno(AlunoDto item)
    {
        Aluno = item;
        CpfBusca = item.Cpf;
        DataNascimento = DateOnly.FromDateTime(item.DataNascimento);
        Endereco = item.Endereco ?? new LogradouroDto();
    }

    private IReadOnlyList<string> Validate()
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(Aluno.Nome))
            errors.Add("Nome é obrigatório.");
        if (CpfValueObject.Criar(Aluno.Cpf).IsFailure)
            errors.Add("CPF deve conter 11 dígitos válidos.");
        if (EmailValueObject.Criar(Aluno.Email).IsFailure)
            errors.Add("Informe um e-mail válido.");
        if (DataNascimento == default || DataNascimento > DateOnly.FromDateTime(DateTime.Today))
            errors.Add("Informe uma data de nascimento válida.");
        if (Endereco is null || CepValueObject.Criar(Endereco.Cep).IsFailure)
            errors.Add("CEP deve conter 8 dígitos.");
        if (string.IsNullOrWhiteSpace(Endereco?.Nome) || string.IsNullOrWhiteSpace(Endereco.Bairro) ||
            string.IsNullOrWhiteSpace(Endereco.Cidade) || string.IsNullOrWhiteSpace(Endereco.Estado) || Endereco.Estado.Trim().Length != 2)
            errors.Add("Busque ou informe rua, bairro, cidade e UF.");
        return errors;
    }
}
