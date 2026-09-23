// Thiago Augusto Ruskowski Waltrick
using System.Collections.Generic;
using AcademiaDoZe.Application.DTOs;

namespace AcademiaDoZe.Application.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IColaboradorService
    {
        Task<IReadOnlyCollection<ColaboradorDto>> ObterTodosAsync(CancellationToken cancellationToken = default);
        Task<ColaboradorDto> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ColaboradorDto> ObterPorCpfAsync(string cpf, CancellationToken cancellationToken = default);
        Task CriarAsync(ColaboradorDto dto, CancellationToken cancellationToken = default);
        Task AtualizarAsync(ColaboradorDto dto, CancellationToken cancellationToken = default);
        Task RemoverAsync(int id, CancellationToken cancellationToken = default);
    }
}
