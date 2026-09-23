// Thiago Augusto Ruskowski Waltrick
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AcademiaDoZe.Application.DTOs;

namespace AcademiaDoZe.Application.Interfaces
{
    public interface IAlunoService
    {
        Task<IReadOnlyCollection<AlunoDto>> ObterTodosAsync(System.Threading.CancellationToken cancellationToken = default);
        Task<AlunoDto> ObterPorIdAsync(int id, System.Threading.CancellationToken cancellationToken = default);
        Task<AlunoDto> ObterPorCpfAsync(string cpf, System.Threading.CancellationToken cancellationToken = default);
        Task CriarAsync(AlunoDto dto, System.Threading.CancellationToken cancellationToken = default);
        Task AtualizarAsync(AlunoDto dto, System.Threading.CancellationToken cancellationToken = default);
        Task RemoverAsync(int id, System.Threading.CancellationToken cancellationToken = default);
    }
}
