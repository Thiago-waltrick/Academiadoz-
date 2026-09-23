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
    }
}
