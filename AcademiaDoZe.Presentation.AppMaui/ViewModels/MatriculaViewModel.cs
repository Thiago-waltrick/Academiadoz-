using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Domain.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Media;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

[QueryProperty(nameof(MatriculaId), "Id")]
public partial class MatriculaViewModel : BaseViewModel
{
    private readonly IMatriculaService _matriculaService;
    private readonly IAlunoService _alunoService;

    private int _matriculaId;
    private string _cpfBusca = string.Empty;
    private AlunoDto? _alunoSelecionado;
    private ImageSource? _fotoAluno;
    private AppMatriculaPlano _planoSelecionado = AppMatriculaPlano.Mensal;
    private DateTime _dataInicio = DateTime.Today;
    private DateTime _dataFim = DateTime.Today.AddMonths(1);
    private string _objetivo = string.Empty;
    private string _observacoesRestricao = string.Empty;
    private ArquivoDto? _laudoMedico;
    private string _errorMessage = string.Empty;
    private string _laudoStatus = "Nenhum arquivo anexado";
    private AppMatriculaRestricoes _outrasRestricoes;

    public int MatriculaId
    {
        get => _matriculaId;
        set
        {
            if (SetProperty(ref _matriculaId, value))
                OnPropertyChanged(nameof(IsEditing));
        }
    }
    public string CpfBusca { get => _cpfBusca; set => SetProperty(ref _cpfBusca, value); }
    public AlunoDto? AlunoSelecionado
    {
        get => _alunoSelecionado;
        set
        {
            if (!SetProperty(ref _alunoSelecionado, value)) return;
            FotoAluno = value?.FotoConteudo is { Length: > 0 } foto
                ? ImageSource.FromStream(() => new MemoryStream(foto))
                : null;
            OnPropertyChanged(nameof(HasAluno));
            OnPropertyChanged(nameof(SemAluno));
            NotifyLaudoState();
        }
    }
    public ImageSource? FotoAluno { get => _fotoAluno; private set => SetProperty(ref _fotoAluno, value); }
    public AppMatriculaPlano PlanoSelecionado
    {
        get => _planoSelecionado;
        set
        {
            if (SetProperty(ref _planoSelecionado, value))
                CalculateFimFromPlanoAndInicio();
        }
    }
    public DateTime DataInicio
    {
        get => _dataInicio;
        set
        {
            if (SetProperty(ref _dataInicio, value))
                CalculateFimFromPlanoAndInicio();
        }
    }
    public DateTime DataFim { get => _dataFim; private set => SetProperty(ref _dataFim, value); }
    public string Objetivo { get => _objetivo; set => SetProperty(ref _objetivo, value); }
    public string ObservacoesRestricao { get => _observacoesRestricao; set => SetProperty(ref _observacoesRestricao, value); }
    public ArquivoDto? LaudoMedico
    {
        get => _laudoMedico;
        set
        {
            if (SetProperty(ref _laudoMedico, value))
            {
                LaudoStatus = value?.Nome ?? "Nenhum arquivo anexado";
                NotifyLaudoState();
            }
        }
    }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public string LaudoStatus { get => _laudoStatus; private set => SetProperty(ref _laudoStatus, value); }

    public IAsyncRelayCommand<int?> LoadCommand { get; }
    public IAsyncRelayCommand SearchByCpfCommand { get; }
    public IAsyncRelayCommand AttachLaudoCommand { get; }
    public IRelayCommand RemoveLaudoCommand { get; }
    public IAsyncRelayCommand SaveCommand { get; }
    public IAsyncRelayCommand CancelCommand { get; }

    public IReadOnlyList<AppMatriculaPlano> Planos { get; } = Enum.GetValues<AppMatriculaPlano>();
    public ObservableCollection<RestricaoOpcao> RestricoesDisponiveis { get; } = new();
    public ObservableCollection<RestricaoSelectable> RestricoesMulti { get; } = new();
    public AppMatriculaRestricoes Restricoes => RestricoesMulti
        .Where(item => item.IsSelected)
        .Aggregate(_outrasRestricoes, (flags, item) => flags | item.Value);
    public bool IsEditing => MatriculaId > 0;
    public bool HasAluno => AlunoSelecionado is not null;
    public bool SemAluno => !HasAluno;
    public bool IsMenor16 => AlunoSelecionado is { DataNascimento: var data } && data != default && data.Date > DateTime.Today.AddYears(-16);
    public bool HasRestricoes => Restricoes != AppMatriculaRestricoes.Nenhuma;
    public bool ExigeLaudo => IsMenor16 || HasRestricoes;
    public bool HasLaudo => LaudoMedico?.Conteudo is { Length: > 0 };
    public bool LaudoPendente => ExigeLaudo && !HasLaudo;
    public bool LaudoOpcional => !ExigeLaudo && !HasLaudo;

    public MatriculaViewModel(IMatriculaService matriculaService, IAlunoService alunoService)
    {
        _matriculaService = matriculaService;
        _alunoService = alunoService;
        Title = "Matrícula";
        LoadCommand = new AsyncRelayCommand<int?>(LoadAsync);
        SearchByCpfCommand = new AsyncRelayCommand(SearchByCpfAsync);
        AttachLaudoCommand = new AsyncRelayCommand(AttachLaudoAsync);
        RemoveLaudoCommand = new RelayCommand(RemoveLaudo);
        SaveCommand = new AsyncRelayCommand(SaveAsync);
        CancelCommand = new AsyncRelayCommand(CancelAsync);

        foreach (var item in new[]
        {
            (AppMatriculaRestricoes.Piscina, "Piscina"),
            (AppMatriculaRestricoes.SalaDeMusculacao, "Sala de musculação"),
            (AppMatriculaRestricoes.HorarioNoturno, "Horário noturno"),
            (AppMatriculaRestricoes.AtividadesEspecificas, "Atividades específicas"),
            (AppMatriculaRestricoes.Diabetes, "Diabetes"),
            (AppMatriculaRestricoes.PressaoAlta, "Pressão alta"),
            (AppMatriculaRestricoes.Labirintite, "Labirintite"),
            (AppMatriculaRestricoes.Alergias, "Alergias"),
            (AppMatriculaRestricoes.ProblemasRespiratorios, "Problemas respiratórios"),
            (AppMatriculaRestricoes.RemedioContinuo, "Remédio contínuo")
        })
        {
            RestricoesDisponiveis.Add(new RestricaoOpcao(item.Item1, item.Item2, OnRestricaoChanged));
        }

        BuildRestricoesMulti();
        CalculateFimFromPlanoAndInicio();
    }

    public async Task LoadAsync(int? id = null)
    {
        ErrorMessage = string.Empty;
        if (id is null or <= 0)
        {
            Reset();
            return;
        }

        IsBusy = true;
        try
        {
            var matricula = await _matriculaService.ObterPorIdAsync(id.Value);
            MatriculaId = matricula.Id;
            AlunoSelecionado = matricula.Aluno;
            PlanoSelecionado = matricula.Plano;
            DataInicio = matricula.DataInicio;
            Objetivo = matricula.Objetivo;
            ObservacoesRestricao = matricula.ObsRestricao ?? string.Empty;
            LaudoMedico = matricula.LaudoMedico;
            LaudoStatus = LaudoMedico?.Nome ?? "Nenhum arquivo anexado";

            SyncSelectionFromFlags(matricula.Restricoes);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Não foi possível carregar a matrícula: {ex.GetBaseException().Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SearchByCpfAsync()
    {
        ErrorMessage = string.Empty;
        if (string.IsNullOrWhiteSpace(CpfBusca))
        {
            ErrorMessage = "Informe o CPF do aluno.";
            return;
        }

        IsBusy = true;
        try
        {
            AlunoSelecionado = await _alunoService.ObterPorCpfAsync(new string(CpfBusca.Where(char.IsDigit).ToArray()));
        }
        catch (Exception ex)
        {
            AlunoSelecionado = null;
            ErrorMessage = $"Aluno não encontrado: {ex.GetBaseException().Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task SelecionarFotoAsync()
    {
        try
        {
            var action = await Shell.Current.DisplayActionSheetAsync("Laudo médico", "Cancelar", null, "Galeria", "Câmera", "Remover laudo");
            if (action == "Remover laudo") { RemoveLaudo(); return; }
            FileResult? file = null;
            if (action == "Galeria")
                file = await FilePicker.Default.PickAsync(new PickOptions { PickerTitle = "Selecione o laudo médico" });
            else if (action == "Câmera")
            {
                if (!MediaPicker.Default.IsCaptureSupported)
                {
                    ErrorMessage = "A captura de fotos não está disponível neste dispositivo.";
                    return;
                }
                if (await Permissions.RequestAsync<Permissions.Camera>() != PermissionStatus.Granted)
                {
                    ErrorMessage = "A permissão da câmera é necessária para fotografar o laudo.";
                    return;
                }
                file = await MediaPicker.Default.CapturePhotoAsync();
            }
            if (file is null) return;

            await using var stream = await file.OpenReadAsync();
            using var memory = new MemoryStream();
            await stream.CopyToAsync(memory);
            LaudoMedico = new ArquivoDto
            {
                Nome = file.FileName,
                ContentType = file.ContentType ?? "application/octet-stream",
                Tamanho = memory.Length,
                Conteudo = memory.ToArray()
            };
            LaudoStatus = file.FileName;
            ErrorMessage = string.Empty;
        }

        catch (Exception ex)
        {
            ErrorMessage = $"Não foi possível anexar o laudo: {ex.GetBaseException().Message}";
        }
    }

    private Task AttachLaudoAsync() => SelecionarFotoAsync();

    private void RemoveLaudo()
    {
        LaudoMedico = null;
        LaudoStatus = "Nenhum arquivo anexado";
    }

    [RelayCommand]
    public async Task SaveMatriculaAsync()
    {
        ErrorMessage = string.Empty;
        var notifications = await ValidateMatriculaAsync();
        if (notifications.Count > 0)
        {
            ErrorMessage = string.Join(Environment.NewLine, notifications.Select(n => n.Mensagem));
            return;
        }

        IsBusy = true;
        try
        {
            var dto = new MatriculaDto
            {
                Id = MatriculaId,
                AlunoId = AlunoSelecionado!.Id,
                Aluno = AlunoSelecionado,
                Plano = PlanoSelecionado,
                DataInicio = DataInicio.Date,
                DataFim = DataFim.Date,
                Objetivo = Objetivo.Trim(),
                Restricoes = Restricoes,
                ObsRestricao = string.IsNullOrWhiteSpace(ObservacoesRestricao) ? null : ObservacoesRestricao.Trim(),
                LaudoMedico = LaudoMedico
            };

            if (IsEditing)
                await _matriculaService.AtualizarAsync(dto);
            else
                await _matriculaService.CriarAsync(dto);

            await Shell.Current.GoToAsync("..");
        }

        catch (Exception ex)
        {
            ErrorMessage = $"Não foi possível salvar a matrícula: {ex.GetBaseException().Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private Task SaveAsync() => SaveMatriculaAsync();

    public Task<IReadOnlyList<Notification>> ValidateMatriculaAsync()
    {
        var notifications = new List<Notification>();
        if (AlunoSelecionado is null || AlunoSelecionado.Id <= 0)
            notifications.Add(new Notification(nameof(AlunoSelecionado), "Selecione um aluno válido antes de salvar a matrícula."));
        else if (AlunoSelecionado.DataNascimento == default || AlunoSelecionado.DataNascimento.Date > DateTime.Today)
            notifications.Add(new Notification(nameof(AlunoSelecionado), "A data de nascimento do aluno deve ser válida para verificar a necessidade de laudo."));
        if (LaudoPendente)
            notifications.Add(new Notification(nameof(LaudoMedico), "O laudo médico é obrigatório para menores de 16 anos ou alunos com restrições."));
        return Task.FromResult<IReadOnlyList<Notification>>(notifications);
    }

    private Task CancelAsync() => Shell.Current.GoToAsync("..");

    private void Reset()
    {
        MatriculaId = 0;
        CpfBusca = string.Empty;
        AlunoSelecionado = null;
        PlanoSelecionado = AppMatriculaPlano.Mensal;
        DataInicio = DateTime.Today;
        Objetivo = string.Empty;
        ObservacoesRestricao = string.Empty;
        LaudoMedico = null;
        LaudoStatus = "Nenhum arquivo anexado";
        SyncSelectionFromFlags(AppMatriculaRestricoes.Nenhuma);
        ErrorMessage = string.Empty;
    }

    public void CalculateFimFromPlanoAndInicio()
    {
        DataFim = PlanoSelecionado switch
        {
            AppMatriculaPlano.Mensal => DataInicio.AddMonths(1),
            AppMatriculaPlano.Trimestral => DataInicio.AddMonths(3),
            AppMatriculaPlano.Semestral => DataInicio.AddMonths(6),
            AppMatriculaPlano.Anual => DataInicio.AddYears(1),
            _ => DataInicio
        };
    }

    private void OnRestricaoChanged(RestricaoOpcao _) => OnPropertyChanged(nameof(Restricoes));

    public void BuildRestricoesMulti()
    {
        foreach (var item in RestricoesMulti)
            item.PropertyChanged -= OnSelectionChanged;
        RestricoesMulti.Clear();
        foreach (var value in Enum.GetValues<AppMatriculaRestricoes>().Where(value => value != AppMatriculaRestricoes.Nenhuma))
        {
            var item = new RestricaoSelectable { Value = value };
            item.PropertyChanged += OnSelectionChanged;
            RestricoesMulti.Add(item);
        }
        RecomputeRestricoesFlags();
    }

    public void SyncSelectionFromFlags(AppMatriculaRestricoes flags)
    {
        var known = RestricoesMulti.Aggregate(AppMatriculaRestricoes.Nenhuma, (current, item) => current | item.Value);
        _outrasRestricoes = flags & ~known;
        foreach (var item in RestricoesMulti)
            item.IsSelected = (flags & item.Value) == item.Value;
        RecomputeRestricoesFlags();
    }

    public void RecomputeRestricoesFlags()
    {
        OnPropertyChanged(nameof(Restricoes));
        NotifyLaudoState();
    }

    private void OnSelectionChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(RestricaoSelectable.IsSelected)) RecomputeRestricoesFlags();
    }

    private void NotifyLaudoState()
    {
        OnPropertyChanged(nameof(IsMenor16));
        OnPropertyChanged(nameof(HasRestricoes));
        OnPropertyChanged(nameof(ExigeLaudo));
        OnPropertyChanged(nameof(HasLaudo));
        OnPropertyChanged(nameof(LaudoPendente));
        OnPropertyChanged(nameof(LaudoOpcional));
    }
}

public sealed class RestricaoOpcao : ObservableObject
{
    private bool _isSelected;
    private readonly Action<RestricaoOpcao> _onChanged;

    public AppMatriculaRestricoes Valor { get; }
    public string Nome { get; }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (SetProperty(ref _isSelected, value))
                _onChanged(this);
        }
    }

    public RestricaoOpcao(AppMatriculaRestricoes valor, string nome, Action<RestricaoOpcao> onChanged)
    {
        Valor = valor;
        Nome = nome;
        _onChanged = onChanged;
    }
}
