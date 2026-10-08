// Thiago Augusto Ruskowski Waltrick
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Application.Mappings;
using AcademiaDoZe.Application.Extensions;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;

namespace AcademiaDoZe.Application.Services
{
    public class MatriculaService : IMatriculaService
    {
        private readonly IMatriculaRepository _repo;

        public MatriculaService(IMatriculaRepository repo)
        {
            _repo = repo;
        }

        public Task<IReadOnlyCollection<MatriculaDto>> BuscarAsync(string termo, CancellationToken cancellationToken = default)
        {
            return Task.Run<IReadOnlyCollection<MatriculaDto>>(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                return _repo.Buscar(termo).Select(m => m.ToDto()).ToList().AsReadOnly();
            }, cancellationToken);
        }

        public Task<MatriculaDto> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var ent = _repo.GetById(id);
            return Task.FromResult(ent.ToDto());
        }

        public Task<IReadOnlyCollection<MatriculaDto>> ObterTodasAsync(CancellationToken cancellationToken = default)
        {
            return Task.Run<IReadOnlyCollection<MatriculaDto>>(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var list = _repo.GetAll();
                return (IReadOnlyCollection<MatriculaDto>)list.Select(m => m.ToDto()).ToList().AsReadOnly();
            }, cancellationToken);
        }

        public Task<IReadOnlyCollection<MatriculaDto>> ObterPorAlunoIdAsync(int alunoId, CancellationToken cancellationToken = default)
        {
            return Task.Run<IReadOnlyCollection<MatriculaDto>>(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var list = _repo.GetByAlunoId(alunoId);
                return (IReadOnlyCollection<MatriculaDto>)list.Select(m => m.ToDto()).ToList().AsReadOnly();
            }, cancellationToken);
        }

        public Task CriarAsync(MatriculaDto dto, CancellationToken cancellationToken = default)
        {
            if (dto == null) throw new System.ArgumentNullException(nameof(dto));

            _repo.Add(dto.ToEntity());
            return Task.CompletedTask;
        }

        public Task AtualizarAsync(MatriculaDto dto, CancellationToken cancellationToken = default)
        {
            if (dto == null) throw new System.ArgumentNullException(nameof(dto));

            _repo.Update(dto.ToEntity());
            return Task.CompletedTask;
        }

        public Task RemoverAsync(int id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _repo.Remove(_repo.GetById(id));
            return Task.CompletedTask;
        }
    }
}
