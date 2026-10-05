// Thiago Augusto Ruskowski Waltrick
using System.Collections.Generic;
using System.Linq;
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
            return Task.Run<IReadOnlyCollection<LogradouroDto>>(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var list = _repo.BuscarPorCep(cep);
                var dtoList = new List<LogradouroDto>();
                foreach (var logradouro in list)
                    dtoList.Add(logradouro.ToDto());
                return dtoList.AsReadOnly();
            }, cancellationToken);
        }

        public Task<IReadOnlyCollection<LogradouroDto>> ObterTodosAsync(CancellationToken cancellationToken = default)
        {
            return Task.Run<IReadOnlyCollection<LogradouroDto>>(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                return _repo.GetAll().Select(item => item.ToDto()).ToList().AsReadOnly();
            }, cancellationToken);
        }

        public Task<LogradouroDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return Task.Run<LogradouroDto?>(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    return _repo.GetById(id).ToDto();
                }
                catch (AcademiaDoZe.Infrastructure.Exceptions.InfrastructureException)
                {
                    return null;
                }
            }, cancellationToken);
        }

        public Task CriarAsync(LogradouroDto dto, CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (dto is null) throw new System.ArgumentNullException(nameof(dto));
                _repo.Add(dto.ToEntity());
            }, cancellationToken);
        }

        public Task AtualizarAsync(LogradouroDto dto, CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (dto is null) throw new System.ArgumentNullException(nameof(dto));
                _repo.Update(dto.ToEntity());
            }, cancellationToken);
        }

        public Task RemoverAsync(int id, CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var entity = _repo.GetById(id);
                _repo.Remove(entity);
            }, cancellationToken);
        }
    }
}
