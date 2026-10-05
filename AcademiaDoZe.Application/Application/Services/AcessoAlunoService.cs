// Thiago Augusto Ruskowski Waltrick
using System;
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
    public class AcessoAlunoService : IAcessoAlunoService
    {
        private readonly IAcessoAlunoRepository _repo;
        private readonly IMatriculaRepository _matRepo;

        public AcessoAlunoService(IAcessoAlunoRepository repo, IMatriculaRepository matRepo)
        {
            _repo = repo;
            _matRepo = matRepo;
        }

        public Task<AcessoAlunoDto> RegistrarEntradaAsync(int alunoId, CancellationToken cancellationToken = default)
        {
            // verificar matrícula ativa
            var possui = false;
            try { possui = _matRepo.PossuiMatriculaAtiva(alunoId); } catch { }
            if (!possui) throw new InvalidOperationException("Aluno não possui matrícula ativa");

            var res = Domain.Entities.AcessoAluno.Criar(0, alunoId);
            if (res.IsFailure) throw new InvalidOperationException("Falha ao criar acesso: " + string.Join(',', res.Notifications));
            _repo.Add(res.Value);

            // calcular tempo restante do contrato
            var ativa = _matRepo.ObterMatriculaAtivaPorAluno(alunoId);
            double? minutosRestantes = null;
            if (ativa != null && ativa.DataFim.HasValue)
            {
                minutosRestantes = (ativa.DataFim.Value - DateTime.UtcNow).TotalMinutes;
            }

            var dto = res.Value.ToDto();
            dto.MinutosRestantesContrato = minutosRestantes;
            return Task.FromResult(dto);
        }

        public Task<AcessoAlunoDto> RegistrarSaidaAsync(int acessoId, CancellationToken cancellationToken = default)
        {
            var ent = _repo.GetById(acessoId);
            var res = ent.RegistrarSaida();
            if (res.IsFailure) throw new InvalidOperationException("Falha ao registrar saída: " + string.Join(',', res.Notifications));
            _repo.Update(ent);
            var dto = ent.ToDto();
            dto.DuracaoMinutos = res.Value.TotalMinutes;
            return Task.FromResult(dto);
        }

        public Task<IReadOnlyCollection<AcessoAlunoDto>> ObterPorAlunoIdAsync(int alunoId, CancellationToken cancellationToken = default)
        {
            var list = _repo.GetByAlunoId(alunoId);
            var dtos = list.Select(a => a.ToDto()).ToList().AsReadOnly();
            return Task.FromResult((IReadOnlyCollection<AcessoAlunoDto>)dtos);
        }
    }
}
