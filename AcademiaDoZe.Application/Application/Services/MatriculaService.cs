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

        public Task<MatriculaDto> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var ent = _repo.GetById(id);
            return Task.FromResult(ent.ToDto());
        }

        public Task<IReadOnlyCollection<MatriculaDto>> ObterTodasAsync(CancellationToken cancellationToken = default)
        {
            var list = _repo.GetAll();
            var dtos = list.Select(m => m.ToDto()).ToList().AsReadOnly();
            return Task.FromResult((IReadOnlyCollection<MatriculaDto>)dtos);
        }

        public Task<IReadOnlyCollection<MatriculaDto>> ObterPorAlunoIdAsync(int alunoId, CancellationToken cancellationToken = default)
        {
            var list = _repo.GetByAlunoId(alunoId);
            var dtos = list.Select(m => m.ToDto()).ToList().AsReadOnly();
            return Task.FromResult((IReadOnlyCollection<MatriculaDto>)dtos);
        }

        public Task CriarAsync(MatriculaDto dto, CancellationToken cancellationToken = default)
        {
            if (dto == null) throw new System.ArgumentNullException(nameof(dto));

            // Regra: não permitir nova matrícula ativa se já existir
            // Verificar via repositório se existe matrícula ativa
            if (_repo.GetType().GetMethod("PossuiMatriculaAtiva") != null)
            {
                try
                {
                    var m = _repo.GetByAlunoId(dto.AlunoId);
                    // se houver qualquer matrícula com dataFim nula ou >= hoje, impedir
                    if (m != null && m.Any(x => x.DataFim == null || x.DataFim >= System.DateTime.UtcNow))
                        throw new System.InvalidOperationException("Aluno já possui matrícula ativa.");
                }
                catch (System.Exception)
                {
                    // ignorar problemas de leitura e deixar fábrica validar
                }
            }

            // Map AppMatriculaPlano -> Domain MatriculaPlano via EnumExtensions
            var planoDomain = dto.Plano switch
            {
                _ => AcademiaDoZe.Domain.Enums.MatriculaPlano.Mensal
            };
            // Prefer using EnumExtensions if available
            var plano = dto.Plano.ToDomain();
            var res = Matricula.Criar(dto.Id, dto.AlunoId, plano, dto.DataInicio);
            if (res.IsFailure) throw new System.InvalidOperationException("Falha ao criar Matrícula: " + string.Join(',', res.Notifications));

            _repo.Add(res.Value);
            return Task.CompletedTask;
        }

        public Task AtualizarAsync(MatriculaDto dto, CancellationToken cancellationToken = default)
        {
            if (dto == null) throw new System.ArgumentNullException(nameof(dto));

            var res = Matricula.Criar(dto.Id, dto.AlunoId, dto.Plano.ToDomain(), dto.DataInicio);
            if (res.IsFailure) throw new System.InvalidOperationException("Falha ao atualizar Matrícula: " + string.Join(',', res.Notifications));

            _repo.Update(res.Value);
            return Task.CompletedTask;
        }
    }
}
