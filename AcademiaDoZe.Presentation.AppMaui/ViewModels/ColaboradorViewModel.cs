using System.IO;
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Enums;
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

[QueryProperty(nameof(ColaboradorId), "Id")]
public partial class ColaboradorViewModel : BaseViewModel
{
    private readonly IColaboradorService _colaboradorService;
    private readonly ILogradouroService _logradouroService;

    [ObservableProperty]
    private int _colaboradorId;

    [ObservableProperty]
    private ColaboradorDto _colaborador = new();

    [ObservableProperty]
    private LogradouroDto _endereco = new();

    [ObservableProperty]
    private string _cpfBusca = string.Empty;

    [ObservableProperty]
    private DateOnly _dataNascimento = DateOnly.FromDateTime(DateTime.Today.AddYears(-18));

    [ObservableProperty]
    private DateOnly _dataAdmissao = DateOnly.FromDateTime(DateTime.Today);

    [ObservableProperty]
    private bool _isEditMode;

    [ObservableProperty]
    private bool _hasEnderecoVinculado;

    [ObservableProperty]
    private string _confirmarSenha = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public IReadOnlyList<AppColaboradorTipo> Tipos { get; } = Enum.GetValues<AppColaboradorTipo>();
    public IReadOnlyList<AppColaboradorVinculo> Vinculos { get; } = Enum.GetValues<AppColaboradorVinculo>();

    public ColaboradorViewModel(IColaboradorService colaboradorService, ILogradouroService logradouroService)
    {
        _colaboradorService = colaboradorService;
        _logradouroService = logradouroService;
        Title = "Colaborador";
    }

    partial void OnColaboradorIdChanged(int value) => IsEditMode = value > 0;

    partial void OnEnderecoChanged(LogradouroDto value) =>
        HasEnderecoVinculado = !string.IsNullOrWhiteSpace(value.Nome);

    [RelayCommand]
    public async Task InitializeAsync()
    {
        ErrorMessage = string.Empty;
        if (ColaboradorId <= 0)
        {
            IsEditMode = false;
            Colaborador = new ColaboradorDto
            {
                DataNascimento = DataNascimento.ToDateTime(TimeOnly.MinValue),
                DataAdmissao = DataAdmissao.ToDateTime(TimeOnly.MinValue)
            };
            Endereco = new LogradouroDto();
            ConfirmarSenha = string.Empty;
            return;
        }

        IsBusy = true;
        try
        {
            var item = await _colaboradorService.ObterPorIdAsync(ColaboradorId);
            AplicarColaborador(item);
            IsEditMode = true;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Não foi possível carregar o colaborador: {ex.GetBaseException().Message}";
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
            ErrorMessage = string.Join(Environment.NewLine, cpf.Notifications.Select(item => item.Mensagem));
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            var item = await _colaboradorService.ObterPorCpfAsync(cpf.Value.Valor);
            AplicarColaborador(item);
            IsEditMode = item.Id > 0;
            ColaboradorId = item.Id;
            ErrorMessage = "Colaborador encontrado. Os dados foram carregados.";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Não foi possível localizar o colaborador: {ex.GetBaseException().Message}";
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
            ErrorMessage = string.Join(Environment.NewLine, cep.Notifications.Select(item => item.Mensagem));
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
            HasEnderecoVinculado = true;
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
    public async Task SaveColaboradorAsync()
    {
        var validation = Validate();
        if (validation.Count > 0)
        {
            ErrorMessage = string.Join(Environment.NewLine, validation);
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            Colaborador.Cpf = CpfValueObject.Criar(Colaborador.Cpf).Value.Valor;
            Colaborador.Email = EmailValueObject.Criar(Colaborador.Email).Value.Endereco;
            Colaborador.DataNascimento = DataNascimento.ToDateTime(TimeOnly.MinValue);
            Colaborador.DataAdmissao = DataAdmissao.ToDateTime(TimeOnly.MinValue);
            Colaborador.Endereco = Endereco;

            if (IsEditMode)
                await _colaboradorService.AtualizarAsync(Colaborador);
            else
                await _colaboradorService.CriarAsync(Colaborador);

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Não foi possível salvar o colaborador: {ex.GetBaseException().Message}";
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
            var action = await shell.DisplayActionSheetAsync(
                "Foto do colaborador",
                "Cancelar",
                null,
                "Galeria",
                "Câmera",
                "Remover Foto");

            if (action == "Remover Foto")
            {
                Colaborador.FotoConteudo = null;
                OnPropertyChanged(nameof(Colaborador));
                return;
            }

            FileResult? photo = action switch
            {
                "Galeria" => await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Escolha uma foto",
                    FileTypes = FilePickerFileType.Images
                }),
                "Câmera" => await CapturarFotoAsync(),
                _ => null
            };

            if (photo is null)
                return;

            await using var stream = await photo.OpenReadAsync();
            using var memory = new MemoryStream();
            await stream.CopyToAsync(memory);
            Colaborador.FotoConteudo = memory.ToArray();
            OnPropertyChanged(nameof(Colaborador));
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

    private void AplicarColaborador(ColaboradorDto item)
    {
        Colaborador = item;
        CpfBusca = item.Cpf;
        DataNascimento = DateOnly.FromDateTime(item.DataNascimento);
        DataAdmissao = DateOnly.FromDateTime(item.DataAdmissao);
        Endereco = item.Endereco ?? new LogradouroDto();
    }

    private IReadOnlyList<string> Validate()
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(Colaborador.Nome))
            errors.Add("Nome é obrigatório.");
        if (CpfValueObject.Criar(Colaborador.Cpf).IsFailure)
            errors.Add("CPF deve conter 11 dígitos válidos.");
        if (EmailValueObject.Criar(Colaborador.Email).IsFailure)
            errors.Add("Informe um e-mail válido.");
        if (DataNascimento == default || DataNascimento > DateOnly.FromDateTime(DateTime.Today))
            errors.Add("Informe uma data de nascimento válida.");
        if (DataAdmissao == default || DataAdmissao > DateOnly.FromDateTime(DateTime.Today))
            errors.Add("Informe uma data de admissão válida.");
        if (Endereco is null || CepValueObject.Criar(Endereco.Cep).IsFailure)
            errors.Add("CEP deve conter 8 dígitos.");
        if (string.IsNullOrWhiteSpace(Endereco?.Nome) || string.IsNullOrWhiteSpace(Endereco.Bairro) ||
            string.IsNullOrWhiteSpace(Endereco.Cidade) || string.IsNullOrWhiteSpace(Endereco.Estado) ||
            Endereco.Estado.Trim().Length != 2)
            errors.Add("Busque ou informe rua, bairro, cidade e UF.");
        if (Colaborador.Tipo == AppColaboradorTipo.Administrador && Colaborador.Vinculo != AppColaboradorVinculo.CLT)
            errors.Add("Administrador deve possuir vínculo CLT.");

        return errors;
    }
}
