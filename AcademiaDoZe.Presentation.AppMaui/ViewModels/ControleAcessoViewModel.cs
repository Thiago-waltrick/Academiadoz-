using System.Collections.ObjectModel;
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels;

public partial class ControleAcessoViewModel : BaseViewModel
{
    private readonly IAcessoService _acessos;
    private readonly IAlunoService _alunos;
    private readonly IColaboradorService _colaboradores;
    private readonly IMatriculaService _matriculas;
    private AcessoDto? _aberto;
    private int _selectionVersion;
    private bool _selectionValid;

    public ObservableCollection<string> Modos { get; } = new() { "Aluno", "Colaborador" };
    public ObservableCollection<AlunoDto> Alunos { get; } = new();
    public ObservableCollection<ColaboradorDto> Colaboradores { get; } = new();
    public ObservableCollection<AcessoHojeItem> AcessosHoje { get; } = new();

    [ObservableProperty] private string modoSelecionado = "Aluno";
    [ObservableProperty] private AlunoDto? selectedAluno;
    [ObservableProperty] private ColaboradorDto? selectedColaborador;
    [ObservableProperty] private string status = "FORA DA ACADEMIA";
    [ObservableProperty] private string acao = "Registrar Entrada";
    [ObservableProperty] private string errorMessage = string.Empty;
    [ObservableProperty] private bool verificandoSelecao;

    public bool IsAluno => ModoSelecionado == "Aluno";
    public bool IsColaborador => !IsAluno;
    public PessoaDto? PessoaSelecionada => IsAluno ? SelectedAluno : SelectedColaborador;
    public bool TemPessoaSelecionada => PessoaSelecionada != null;
    public bool PodeSelecionar => !IsBusy;
    public bool PodeRegistrar => !IsBusy && !VerificandoSelecao && TemPessoaSelecionada && _selectionValid;

    public ControleAcessoViewModel(IAcessoService acessos, IAlunoService alunos, IColaboradorService colaboradores, IMatriculaService matriculas)
    {
        _acessos = acessos;
        _alunos = alunos;
        _colaboradores = colaboradores;
        _matriculas = matriculas;
        Title = "Controle de Acesso / Catraca";
        PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(IsBusy) || e.PropertyName == nameof(VerificandoSelecao))
            {
                OnPropertyChanged(nameof(PodeSelecionar));
                OnPropertyChanged(nameof(PodeRegistrar));
            }
        };
    }

    partial void OnModoSelecionadoChanged(string value)
    {
        OnPropertyChanged(nameof(IsAluno));
        OnPropertyChanged(nameof(IsColaborador));
        _ = AtualizarSelecaoAsync();
    }
    partial void OnSelectedAlunoChanged(AlunoDto? value) => _ = AtualizarSelecaoAsync();
    partial void OnSelectedColaboradorChanged(ColaboradorDto? value) => _ = AtualizarSelecaoAsync();

    private async Task AtualizarSelecaoAsync()
    {
        var version = ++_selectionVersion;
        _aberto = null;
        _selectionValid = false;
        OnPropertyChanged(nameof(PodeRegistrar));
        Status = PessoaSelecionada == null ? "FORA DA ACADEMIA" : "VERIFICANDO ACESSO";
        Acao = "Registrar Entrada";
        OnPropertyChanged(nameof(PessoaSelecionada));
        OnPropertyChanged(nameof(TemPessoaSelecionada));
        VerificandoSelecao = true;
        try
        {
            var pessoa = PessoaSelecionada;
            var aberto = pessoa == null ? null : IsAluno
                ? await _acessos.ObterEntradaAbertaAlunoAsync(pessoa.Id)
                : await _acessos.ObterEntradaAbertaColaboradorAsync(pessoa.Id);
            if (version != _selectionVersion) return;
            _aberto = aberto;
            _selectionValid = true;
            Status = aberto == null ? "FORA DA ACADEMIA" : "DENTRO DA ACADEMIA";
            Acao = aberto == null ? "Registrar Entrada" : "Registrar Saída";
            ErrorMessage = string.Empty;
        }
        catch (Exception ex)
        {
            if (version == _selectionVersion) ErrorMessage = $"Não foi possível verificar o acesso: {ex.GetBaseException().Message}";
        }
        finally
        {
            if (version == _selectionVersion) VerificandoSelecao = false;
        }
    }

    public async Task InitializeAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            ErrorMessage = string.Empty;
            var alunos = await _alunos.ObterTodosAsync();
            var colaboradores = await _colaboradores.ObterTodosAsync();
            Alunos.Clear();
            foreach (var aluno in alunos.OrderBy(a => a.Nome)) Alunos.Add(aluno);
            Colaboradores.Clear();
            foreach (var colaborador in colaboradores.OrderBy(c => c.Nome)) Colaboradores.Add(colaborador);
            await CarregarAcessosAsync();
            await AtualizarSelecaoAsync();
        }
        catch (Exception ex) { ErrorMessage = $"Não foi possível carregar os dados: {ex.GetBaseException().Message}"; }
        finally { IsBusy = false; }
    }

    private async Task CarregarAcessosAsync()
    {
        var acessos = await _acessos.ObterAcessosHojeAsync();
        AcessosHoje.Clear();
        foreach (var acesso in acessos)
        {
            PessoaDto? pessoa = acesso.AlunoId.HasValue ? Alunos.FirstOrDefault(a => a.Id == acesso.AlunoId) : Colaboradores.FirstOrDefault(c => c.Id == acesso.ColaboradorId);
            AcessosHoje.Add(new AcessoHojeItem
            {
                Nome = pessoa?.Nome ?? "Pessoa não encontrada",
                FotoConteudo = pessoa?.FotoConteudo,
                Tipo = acesso.AlunoId.HasValue ? "Aluno" : "Colaborador",
                Entrada = $"Entrada: {acesso.Entrada.ToLocalTime():HH:mm:ss}",
                Saida = acesso.HistoricoLegado ? "Histórico (saída não informada)" : acesso.Saida.HasValue ? $"Saída: {acesso.Saida.Value.ToLocalTime():HH:mm:ss}" : "Saída: em aberto"
            });
        }
    }

    [RelayCommand]
    private async Task RegistrarAcessoAsync()
    {
        if (!PodeRegistrar) return;
        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            if (IsAluno && SelectedAluno != null)
            {
                if (_aberto != null)
                {
                    var saida = await _acessos.RegistrarSaidaAlunoAsync(_aberto.Id);
                    await CarregarAcessosAsync();
                    await AtualizarSelecaoAsync();
                    await Shell.Current.DisplayAlertAsync("Saída Registrada", $"Saída registrada. Tempo de permanência: {FormatarTempo(TimeSpan.FromMinutes(saida.DuracaoMinutos ?? 0))}.", "OK");
                }
                else
                {
                    var matriculas = await _matriculas.ObterPorAlunoIdAsync(SelectedAluno.Id);
                    var hoje = DateTime.Today;
                    var ativa = matriculas.Where(m => m.DataInicio.Date <= hoje && m.DataFim.HasValue && m.DataFim.Value.Date >= hoje).OrderByDescending(m => m.DataFim).FirstOrDefault();
                    if (ativa == null)
                    {
                        await Shell.Current.DisplayAlertAsync("Acesso Negado", "Acesso negado: aluno não possui matrícula ativa.", "OK");
                        return;
                    }
                    await _acessos.RegistrarEntradaAlunoAsync(SelectedAluno.Id);
                    await CarregarAcessosAsync();
                    await AtualizarSelecaoAsync();
                    await Shell.Current.DisplayAlertAsync("Entrada Autorizada", $"Entrada autorizada. Dias restantes de plano: {(ativa.DataFim!.Value.Date - hoje).Days} dias.", "OK");
                }
            }
            else if (SelectedColaborador != null)
            {
                if (_aberto != null)
                {
                    var saida = await _acessos.RegistrarSaidaColaboradorAsync(_aberto.Id);
                    var total = TimeSpan.FromMinutes(saida.MinutosTrabalhadosDia ?? 0);
                    await CarregarAcessosAsync();
                    await AtualizarSelecaoAsync();
                    var aviso = saida.ExcedeuJornada == true ? " A carga horária diária foi excedida." : string.Empty;
                    await Shell.Current.DisplayAlertAsync("Saída Registrada", $"Tempo total de permanência hoje: {FormatarTempo(total)}.{aviso}", "OK");
                }
                else
                {
                    var total = await _acessos.ObterTempoColaboradorHojeAsync(SelectedColaborador.Id);
                    var limite = SelectedColaborador.Vinculo switch { AppColaboradorVinculo.CLT => TimeSpan.FromHours(8), AppColaboradorVinculo.Estagio => TimeSpan.FromHours(6), _ => throw new InvalidOperationException("Vínculo de colaborador inválido.") };
                    if (total >= limite)
                    {
                        await Shell.Current.DisplayAlertAsync("Acesso Negado", "Limite diário de horas para o vínculo atingido.", "OK");
                        return;
                    }
                    await _acessos.RegistrarEntradaColaboradorAsync(SelectedColaborador.Id);
                    await CarregarAcessosAsync();
                    await AtualizarSelecaoAsync();
                    await Shell.Current.DisplayAlertAsync("Entrada Autorizada", "Entrada autorizada.", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            await AtualizarSelecaoAsync();
            ErrorMessage = $"Não foi possível concluir a operação: {ex.GetBaseException().Message}";
            await Shell.Current.DisplayAlertAsync("Erro", ErrorMessage, "OK");
        }
        finally { IsBusy = false; }
    }

    private static string FormatarTempo(TimeSpan tempo) => $"{(int)tempo.TotalHours:00}:{tempo.Minutes:00}:{tempo.Seconds:00}";
}

public class AcessoHojeItem
{
    public string Nome { get; set; } = string.Empty;
    public byte[]? FotoConteudo { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Entrada { get; set; } = string.Empty;
    public string Saida { get; set; } = string.Empty;
}
