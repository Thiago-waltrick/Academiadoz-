using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels
{
    public sealed class TreinoExecucaoViewModel : BaseViewModel
    {
        private readonly ITreinoService _treinoService;
        private bool _treinoAtivo;
        private string _nomeExercicio = string.Empty;
        private string _inicioTreinoTexto = string.Empty;
        private string _feedback = string.Empty;
        private string _corFeedback = "#A78BFA";

        public ObservableCollection<TreinoExercicioViewModel> Exercicios { get; } = new();
        public AsyncRelayCommand CarregarCommand { get; }
        public AsyncRelayCommand IniciarTreinoCommand { get; }
        public AsyncRelayCommand AdicionarExercicioCommand { get; }
        public AsyncRelayCommand FinalizarTreinoCommand { get; }

        public bool TreinoAtivo
        {
            get => _treinoAtivo;
            private set
            {
                if (!SetProperty(ref _treinoAtivo, value)) return;
                OnPropertyChanged(nameof(TreinoInativo));
            }
        }

        public bool TreinoInativo => !TreinoAtivo;

        public string NomeExercicio
        {
            get => _nomeExercicio;
            set => SetProperty(ref _nomeExercicio, value);
        }

        public string InicioTreinoTexto
        {
            get => _inicioTreinoTexto;
            private set => SetProperty(ref _inicioTreinoTexto, value);
        }

        public string Feedback
        {
            get => _feedback;
            private set
            {
                if (!SetProperty(ref _feedback, value)) return;
                OnPropertyChanged(nameof(TemFeedback));
            }
        }

        public bool TemFeedback => !string.IsNullOrEmpty(Feedback);

        public string CorFeedback
        {
            get => _corFeedback;
            private set => SetProperty(ref _corFeedback, value);
        }

        public TreinoExecucaoViewModel(ITreinoService treinoService)
        {
            _treinoService = treinoService;
            Title = "Treino ativo";
            CarregarCommand = new AsyncRelayCommand(CarregarAsync);
            IniciarTreinoCommand = new AsyncRelayCommand(IniciarTreinoAsync);
            AdicionarExercicioCommand = new AsyncRelayCommand(AdicionarExercicioAsync);
            FinalizarTreinoCommand = new AsyncRelayCommand(FinalizarTreinoAsync);
        }

        public async Task CarregarAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            Feedback = string.Empty;
            try
            {
                var sessao = await _treinoService.ObterTreinoAtivoAsync();
                AtualizarTela(sessao);
            }
            catch (Exception ex)
            {
                ExibirFeedback($"Não foi possível carregar o treino. {ex.Message}", true);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task IniciarTreinoAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var sessao = await _treinoService.IniciarTreinoAsync();
                AtualizarTela(sessao);
                ExibirFeedback("Treino iniciado. Registre o primeiro exercício.", false);
            }
            catch (Exception ex)
            {
                ExibirFeedback($"Não foi possível iniciar o treino. {ex.Message}", true);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task AdicionarExercicioAsync()
        {
            if (IsBusy) return;
            if (string.IsNullOrWhiteSpace(NomeExercicio))
            {
                ExibirFeedback("Informe o nome do exercício.", true);
                return;
            }

            IsBusy = true;
            try
            {
                var exercicio = await _treinoService.AdicionarExercicioAsync(NomeExercicio);
                Exercicios.Add(CriarExercicioViewModel(exercicio));
                NomeExercicio = string.Empty;
                ExibirFeedback("Exercício adicionado ao treino.", false);
            }
            catch (Exception ex)
            {
                ExibirFeedback($"Não foi possível adicionar o exercício. {ex.Message}", true);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task FinalizarTreinoAsync()
        {
            if (IsBusy) return;
            var shell = Shell.Current;
            if (shell is not null && !await shell.DisplayAlertAsync("Finalizar treino", "Deseja encerrar esta sessão?", "Finalizar", "Continuar"))
                return;

            IsBusy = true;
            try
            {
                await _treinoService.FinalizarTreinoAsync();
                AtualizarTela(null);
                ExibirFeedback("Treino finalizado e salvo com sucesso.", false);
            }
            catch (Exception ex)
            {
                ExibirFeedback($"Não foi possível finalizar o treino. {ex.Message}", true);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void AtualizarTela(TreinoSessaoDto? sessao)
        {
            Exercicios.Clear();
            TreinoAtivo = sessao is not null;
            InicioTreinoTexto = sessao is null
                ? string.Empty
                : $"Iniciado às {sessao.IniciadaEm.ToLocalTime():HH:mm}";

            if (sessao is null) return;

            foreach (var exercicio in sessao.Exercicios)
                Exercicios.Add(CriarExercicioViewModel(exercicio));
        }

        private TreinoExercicioViewModel CriarExercicioViewModel(TreinoExercicioDto exercicio) =>
            new(exercicio, _treinoService, ExibirFeedback);

        private void ExibirFeedback(string mensagem, bool erro)
        {
            Feedback = mensagem;
            CorFeedback = erro ? "#FCA5A5" : "#C4B5FD";
        }
    }
}