using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AcademiaDoZe.Application.DTOs;

namespace AcademiaDoZe.Application.Interfaces
{
    public interface IAcessoService
    {
        Task<IReadOnlyCollection<AcessoDto>> ObterAcessosHojeAsync(CancellationToken cancellationToken = default);
        Task<AcessoDto?> ObterEntradaAbertaAlunoAsync(int alunoId, CancellationToken cancellationToken = default);
        Task<AcessoDto?> ObterEntradaAbertaColaboradorAsync(int colaboradorId, CancellationToken cancellationToken = default);
        Task<AcessoAlunoDto> RegistrarEntradaAlunoAsync(int alunoId, CancellationToken cancellationToken = default);
        Task<AcessoAlunoDto> RegistrarSaidaAlunoAsync(int acessoId, CancellationToken cancellationToken = default);
        Task<AcessoColaboradorDto> RegistrarEntradaColaboradorAsync(int colaboradorId, CancellationToken cancellationToken = default);
        Task<AcessoColaboradorDto> RegistrarSaidaColaboradorAsync(int acessoId, CancellationToken cancellationToken = default);
        Task<TimeSpan> ObterTempoColaboradorHojeAsync(int colaboradorId, CancellationToken cancellationToken = default);
    }
}
