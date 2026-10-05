using System;
using System.Linq;
using System.Threading.Tasks;
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;

namespace AcademiaDoZe.Application.Services
{
    public sealed class TreinoService : ITreinoService
    {
        private readonly ITreinoRepository _repository;

        public TreinoService(ITreinoRepository repository)
        {
            _repository = repository;
        }

        public async Task<TreinoSessaoDto?> ObterTreinoAtivoAsync()
        {
            var sessao = await _repository.ObterSessaoAtivaAsync();
            return sessao is null ? null : await MapearSessaoAsync(sessao);
        }

        public async Task<TreinoSessaoDto> IniciarTreinoAsync()
        {
            var atual = await _repository.ObterSessaoAtivaAsync();
            if (atual is not null)
                return await MapearSessaoAsync(atual);

            var sessao = TreinoSessao.Criar(DateTime.UtcNow);
            var id = await _repository.IniciarSessaoAsync(sessao);
            return await MapearSessaoAsync(TreinoSessao.Reconstituir(id, sessao.IniciadaEm, null));
        }

        public async Task<TreinoExercicioDto> AdicionarExercicioAsync(string nome)
        {
            var sessao = await ObterSessaoAtivaObrigatoriaAsync();
            var exercicio = TreinoExercicio.Criar(sessao.Id, nome);
            var id = await _repository.AdicionarExercicioAsync(exercicio);
            return new TreinoExercicioDto { Id = id, Nome = exercicio.Nome };
        }

        public async Task<TreinoSerieDto> AdicionarSerieAsync(int exercicioId, decimal cargaKg, int repeticoes)
        {
            var sessao = await ObterSessaoAtivaObrigatoriaAsync();
            var exercicios = await _repository.ObterExerciciosAsync(sessao.Id);
            if (!exercicios.Any(item => item.Id == exercicioId))
                throw new InvalidOperationException("O exercício não pertence ao treino ativo.");

            var series = await _repository.ObterSeriesAsync(exercicioId);
            var numero = series.Count == 0 ? 1 : series.Max(item => item.Numero) + 1;
            var serie = TreinoSerie.Criar(exercicioId, numero, cargaKg, repeticoes);
            var id = await _repository.AdicionarSerieAsync(serie);

            return new TreinoSerieDto
            {
                Id = id,
                Numero = numero,
                CargaKg = serie.CargaKg,
                Repeticoes = serie.Repeticoes
            };
        }

        public async Task AtualizarSerieAsync(int serieId, decimal cargaKg, int repeticoes)
        {
            var serie = await ObterSerieDoTreinoAtivoAsync(serieId);
            if (serie.Concluida)
                throw new InvalidOperationException("Uma série concluída não pode ser alterada.");

            serie.Atualizar(cargaKg, repeticoes);
            await _repository.AtualizarSerieAsync(serie);
        }

        public async Task ConcluirSerieAsync(int serieId)
        {
            var serie = await ObterSerieDoTreinoAtivoAsync(serieId);
            serie.MarcarConcluida();
            await _repository.AtualizarSerieAsync(serie);
        }

        public async Task FinalizarTreinoAsync()
        {
            var sessao = await ObterSessaoAtivaObrigatoriaAsync();
            sessao.Finalizar(DateTime.UtcNow);
            await _repository.FinalizarSessaoAsync(sessao);
        }

        private async Task<TreinoSessao> ObterSessaoAtivaObrigatoriaAsync()
        {
            return await _repository.ObterSessaoAtivaAsync()
                ?? throw new InvalidOperationException("Não há treino ativo.");
        }

        private async Task<TreinoSerie> ObterSerieDoTreinoAtivoAsync(int serieId)
        {
            var sessao = await ObterSessaoAtivaObrigatoriaAsync();
            var serie = await _repository.ObterSerieAsync(serieId)
                ?? throw new InvalidOperationException("Série não encontrada.");
            var exercicios = await _repository.ObterExerciciosAsync(sessao.Id);
            if (!exercicios.Any(exercicio => exercicio.Id == serie.ExercicioId))
                throw new InvalidOperationException("A série não pertence ao treino ativo.");

            return serie;
        }

        private async Task<TreinoSessaoDto> MapearSessaoAsync(TreinoSessao sessao)
        {
            var exercicios = await _repository.ObterExerciciosAsync(sessao.Id);
            var exerciciosDto = new TreinoExercicioDto[exercicios.Count];

            for (var index = 0; index < exercicios.Count; index++)
            {
                var exercicio = exercicios.ElementAt(index);
                var series = await _repository.ObterSeriesAsync(exercicio.Id);
                exerciciosDto[index] = new TreinoExercicioDto
                {
                    Id = exercicio.Id,
                    Nome = exercicio.Nome,
                    Series = series.Select(serie => new TreinoSerieDto
                    {
                        Id = serie.Id,
                        Numero = serie.Numero,
                        CargaKg = serie.CargaKg,
                        Repeticoes = serie.Repeticoes,
                        Concluida = serie.Concluida
                    }).ToArray()
                };
            }

            return new TreinoSessaoDto
            {
                Id = sessao.Id,
                IniciadaEm = sessao.IniciadaEm,
                Exercicios = exerciciosDto
            };
        }
    }
}
