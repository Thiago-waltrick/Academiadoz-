using System;
using System.Globalization;
using System.Threading.Tasks;
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels
{
    public sealed class TreinoSerieViewModel : ObservableObject
    {
        private readonly ITreinoService _treinoService;
        private readonly Action<string, bool> _feedback;
        private string _cargaKg;
        private string _repeticoes;
        private bool _concluida;
        private bool _isBusy;

        public int Id { get; }
        public int Numero { get; }
        public AsyncRelayCommand SalvarCommand { get; }
        public AsyncRelayCommand MarcarConcluidaCommand { get; }

        public string CargaKg
        {
            get => _cargaKg;
            set => SetProperty(ref _cargaKg, value);
        }

        public string Repeticoes
        {
            get => _repeticoes;
            set => SetProperty(ref _repeticoes, value);
        }

        public bool Concluida
        {
            get => _concluida;
            private set
            {
                if (!SetProperty(ref _concluida, value)) return;
                OnPropertyChanged(nameof(PodeEditar));
                OnPropertyChanged(nameof(StatusTexto));
                OnPropertyChanged(nameof(CorStatus));
            }
        }

        public bool IsBusy
        {
            get => _isBusy;
            private set => SetProperty(ref _isBusy, value);
        }

        public bool PodeEditar => !Concluida;
        public string StatusTexto => Concluida ? "CONCLUÍDA" : "PENDENTE";
        public string CorStatus => Concluida ? "#86EFAC" : "#C4B5FD";

        public TreinoSerieViewModel(TreinoSerieDto serie, ITreinoService treinoService, Action<string, bool> feedback)
        {
            Id = serie.Id;
            Numero = serie.Numero;
            _cargaKg = serie.CargaKg.ToString("0.##", CultureInfo.CurrentCulture);
            _repeticoes = serie.Repeticoes.ToString(CultureInfo.CurrentCulture);
            _concluida = serie.Concluida;
            _treinoService = treinoService;
            _feedback = feedback;
            SalvarCommand = new AsyncRelayCommand(SalvarAsync);
            MarcarConcluidaCommand = new AsyncRelayCommand(MarcarConcluidaAsync);
        }

        public async Task SalvarAsync()
        {
            if (IsBusy || Concluida) return;
            if (!TentarLerValores(out var carga, out var repeticoes)) return;

            IsBusy = true;
            try
            {
                await _treinoService.AtualizarSerieAsync(Id, carga, repeticoes);
                CargaKg = carga.ToString("0.##", CultureInfo.CurrentCulture);
                Repeticoes = repeticoes.ToString(CultureInfo.CurrentCulture);
                _feedback($"Série {Numero} salva.", false);
            }
            catch (Exception ex)
            {
                _feedback($"Não foi possível salvar a série. {ex.Message}", true);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task MarcarConcluidaAsync()
        {
            if (IsBusy || Concluida) return;
            if (!TentarLerValores(out var carga, out var repeticoes)) return;

            IsBusy = true;
            try
            {
                await _treinoService.AtualizarSerieAsync(Id, carga, repeticoes);
                await _treinoService.ConcluirSerieAsync(Id);
                CargaKg = carga.ToString("0.##", CultureInfo.CurrentCulture);
                Repeticoes = repeticoes.ToString(CultureInfo.CurrentCulture);
                Concluida = true;
                _feedback($"Série {Numero} concluída.", false);
            }
            catch (Exception ex)
            {
                _feedback($"Não foi possível concluir a série. {ex.Message}", true);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool TentarLerValores(out decimal carga, out int repeticoes)
        {
            if (!decimal.TryParse(CargaKg, NumberStyles.Number, CultureInfo.CurrentCulture, out carga) &&
                !decimal.TryParse(CargaKg, NumberStyles.Number, CultureInfo.InvariantCulture, out carga))
            {
                _feedback("Informe uma carga válida em kg.", true);
                repeticoes = 0;
                return false;
            }

            if (!int.TryParse(Repeticoes, NumberStyles.Integer, CultureInfo.CurrentCulture, out repeticoes) || repeticoes <= 0)
            {
                _feedback("As repetições devem ser um número inteiro maior que zero.", true);
                return false;
            }

            return true;
        }
    }
}