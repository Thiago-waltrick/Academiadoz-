using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels
{
    public sealed class TreinoExercicioViewModel : ObservableObject
    {
        private readonly ITreinoService _treinoService;
        private readonly Action<string, bool> _feedback;
        private string _novaCargaKg = "0";
        private string _novasRepeticoes = "10";
        private bool _isBusy;

        public int Id { get; }
        public string Nome { get; }
        public ObservableCollection<TreinoSerieViewModel> Series { get; } = new();
        public AsyncRelayCommand AdicionarSerieCommand { get; }

        public string NovaCargaKg
        {
            get => _novaCargaKg;
            set => SetProperty(ref _novaCargaKg, value);
        }

        public string NovasRepeticoes
        {
            get => _novasRepeticoes;
            set => SetProperty(ref _novasRepeticoes, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            private set => SetProperty(ref _isBusy, value);
        }

        public string ResumoSeries => $"{Series.Count} {(Series.Count == 1 ? "série registrada" : "séries registradas")}";

        public TreinoExercicioViewModel(TreinoExercicioDto exercicio, ITreinoService treinoService, Action<string, bool> feedback)
        {
            Id = exercicio.Id;
            Nome = exercicio.Nome;
            _treinoService = treinoService;
            _feedback = feedback;
            foreach (var serie in exercicio.Series)
                Series.Add(CriarSerieViewModel(serie));
            Series.CollectionChanged += (_, _) => OnPropertyChanged(nameof(ResumoSeries));
            AdicionarSerieCommand = new AsyncRelayCommand(AdicionarSerieAsync);
        }

        public async Task AdicionarSerieAsync()
        {
            if (IsBusy) return;
            if (!decimal.TryParse(NovaCargaKg, NumberStyles.Number, CultureInfo.CurrentCulture, out var carga) &&
                !decimal.TryParse(NovaCargaKg, NumberStyles.Number, CultureInfo.InvariantCulture, out carga))
            {
                _feedback("Informe uma carga válida em kg.", true);
                return;
            }

            if (!int.TryParse(NovasRepeticoes, NumberStyles.Integer, CultureInfo.CurrentCulture, out var repeticoes) || repeticoes <= 0)
            {
                _feedback("As repetições devem ser um número inteiro maior que zero.", true);
                return;
            }

            IsBusy = true;
            try
            {
                var serie = await _treinoService.AdicionarSerieAsync(Id, carga, repeticoes);
                Series.Add(CriarSerieViewModel(serie));
                _feedback($"Série {serie.Numero} registrada em {Nome}.", false);
            }
            catch (Exception ex)
            {
                _feedback($"Não foi possível registrar a série. {ex.Message}", true);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private TreinoSerieViewModel CriarSerieViewModel(TreinoSerieDto serie) =>
            new(serie, _treinoService, _feedback);
    }
}