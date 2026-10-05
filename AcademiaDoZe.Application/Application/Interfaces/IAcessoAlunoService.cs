// Thiago Augusto Ruskowski Waltrick
// Thiago Augusto Ruskowski Waltrick
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AcademiaDoZe.Application.DTOs;

namespace AcademiaDoZe.Application.Interfaces
{
    public interface IAcessoAlunoService
    {
        Task<AcessoAlunoDto> RegistrarEntradaAsync(int alunoId, CancellationToken cancellationToken = default);
        Task<AcessoAlunoDto> RegistrarSaidaAsync(int acessoId, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<AcessoAlunoDto>> ObterPorAlunoIdAsync(int alunoId, CancellationToken cancellationToken = default);
    }
}
