using System.Collections.Generic;
using System.Threading.Tasks;
using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Domain.Repositories
{
    public interface ITreinoRepository
    {
        Task<TreinoSessao?> ObterSessaoAtivaAsync();
        Task<int> IniciarSessaoAsync(TreinoSessao sessao);
        Task<IReadOnlyCollection<TreinoExercicio>> ObterExerciciosAsync(int sessaoId);
        Task<int> AdicionarExercicioAsync(TreinoExercicio exercicio);
        Task<IReadOnlyCollection<TreinoSerie>> ObterSeriesAsync(int exercicioId);
        Task<TreinoSerie?> ObterSerieAsync(int serieId);
        Task<int> AdicionarSerieAsync(TreinoSerie serie);
        Task AtualizarSerieAsync(TreinoSerie serie);
        Task FinalizarSessaoAsync(TreinoSessao sessao);
    }
}
