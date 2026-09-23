// Thiago Augusto Ruskowski Waltrick
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Application.Mappings;
using AcademiaDoZe.Domain.Repositories;

namespace AcademiaDoZe.Application.Services
{
    public class LogradouroService : ILogradouroService
    {
        private readonly ILogradouroRepository _repo;

        public LogradouroService(ILogradouroRepository repo)
        {
            _repo = repo;
        }

        public Task<IReadOnlyCollection<LogradouroDto>> BuscarPorCepAsync(string cep, CancellationToken cancellationToken = default)
        {
            // repository is synchronous; wrap in Task.FromResult to respect async signature
            var list = _repo.BuscarPorCep(cep);
            var dtoList = new List<LogradouroDto>();
            foreach (var l in list)
                dtoList.Add(l.ToDto());
            return Task.FromResult((IReadOnlyCollection<LogradouroDto>)dtoList.AsReadOnly());
        }
    }
}
