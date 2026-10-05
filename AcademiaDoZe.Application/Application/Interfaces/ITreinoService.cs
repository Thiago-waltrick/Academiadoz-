using System.Threading.Tasks;
using AcademiaDoZe.Application.DTOs;

namespace AcademiaDoZe.Application.Interfaces
{
    public interface ITreinoService
    {
        Task<TreinoSessaoDto?> ObterTreinoAtivoAsync();
        Task<TreinoSessaoDto> IniciarTreinoAsync();
        Task<TreinoExercicioDto> AdicionarExercicioAsync(string nome);
        Task<TreinoSerieDto> AdicionarSerieAsync(int exercicioId, decimal cargaKg, int repeticoes);
        Task AtualizarSerieAsync(int serieId, decimal cargaKg, int repeticoes);
        Task ConcluirSerieAsync(int serieId);
        Task FinalizarTreinoAsync();
    }
}
