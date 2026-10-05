// Thiago Augusto Ruskowski Waltrick
// Thiago Augusto Ruskowski Waltrick
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AcademiaDoZe.Application.DTOs;

namespace AcademiaDoZe.Application.Interfaces
{
    public interface IAcessoColaboradorService
    {
        Task<AcessoColaboradorDto> RegistrarEntradaAsync(int colaboradorId, CancellationToken cancellationToken = default);
        Task<AcessoColaboradorDto> RegistrarSaidaAsync(int acessoId, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<AcessoColaboradorDto>> ObterPorColaboradorIdAsync(int colaboradorId, CancellationToken cancellationToken = default);
    }
}
