// Thiago Augusto Ruskowski Waltrick
using System.Collections.Generic;
using System.Threading.Tasks;
using AcademiaDoZe.Application.DTOs;

namespace AcademiaDoZe.Application.Interfaces
{
    public interface IMatriculaService
    {
        Task<MatriculaDto> ObterPorIdAsync(int id, System.Threading.CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<MatriculaDto>> ObterTodasAsync(System.Threading.CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<MatriculaDto>> ObterPorAlunoIdAsync(int alunoId, System.Threading.CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<MatriculaDto>> BuscarAsync(string termo, System.Threading.CancellationToken cancellationToken = default);
        Task CriarAsync(MatriculaDto dto, System.Threading.CancellationToken cancellationToken = default);
        Task AtualizarAsync(MatriculaDto dto, System.Threading.CancellationToken cancellationToken = default);
        Task RemoverAsync(int id, System.Threading.CancellationToken cancellationToken = default);
    }
}
