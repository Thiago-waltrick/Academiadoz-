// Thiago Augusto Ruskowski Waltrick
using System.Collections.Generic;
using AcademiaDoZe.Application.DTOs;

namespace AcademiaDoZe.Application.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface ILogradouroService
    {
        Task<IReadOnlyCollection<LogradouroDto>> BuscarPorCepAsync(string cep, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<LogradouroDto>> ObterTodosAsync(CancellationToken cancellationToken = default);
        Task<LogradouroDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
        Task CriarAsync(LogradouroDto dto, CancellationToken cancellationToken = default);
        Task AtualizarAsync(LogradouroDto dto, CancellationToken cancellationToken = default);
        Task RemoverAsync(int id, CancellationToken cancellationToken = default);
    }
}
