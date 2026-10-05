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
using AcademiaDoZe.Domain.Enums;

namespace AcademiaDoZe.Application.Services
{
    public class AcessoColaboradorService : IAcessoColaboradorService
    {
        private readonly IAcessoColaboradorRepository _repo;
        private readonly IColaboradorRepository _colRepo;

        public AcessoColaboradorService(IAcessoColaboradorRepository repo, IColaboradorRepository colRepo)
        {
            _repo = repo;
            _colRepo = colRepo;
        }

        public Task<AcessoColaboradorDto> RegistrarEntradaAsync(int colaboradorId, CancellationToken cancellationToken = default)
        {
            var res = Domain.Entities.AcessoColaborador.Criar(0, colaboradorId);
            if (res.IsFailure) throw new InvalidOperationException("Falha ao criar acesso: " + string.Join(',', res.Notifications));
            _repo.Add(res.Value);
            var dto = res.Value.ToDto();
            return Task.FromResult(dto);
        }

        public Task<AcessoColaboradorDto> RegistrarSaidaAsync(int acessoId, CancellationToken cancellationToken = default)
        {
            var ent = _repo.GetById(acessoId);
            var res = ent.RegistrarSaida();
            if (res.IsFailure) throw new InvalidOperationException("Falha ao registrar saída: " + string.Join(',', res.Notifications));
            _repo.Update(ent);

            // calcular tempo trabalhado no dia
            var dia = ent.Entrada.Date;
            var registros = _repo.GetByColaboradorIdAndDate(ent.ColaboradorId, dia);
            double minutos = 0;
            foreach (var r in registros)
            {
                var fim = r.Saida ?? DateTime.UtcNow;
                minutos += (fim - r.Entrada).TotalMinutes;
            }

            // verificar jornada por vínculo
            var colaborador = _colRepo.GetById(ent.ColaboradorId);
            var limite = colaborador.Vinculo == ColaboradorVinculo.CLT ? 8 * 60 : 6 * 60;
            var excedeu = minutos > limite;

            var dto = ent.ToDto();
            dto.MinutosTrabalhadosDia = minutos;
            dto.ExcedeuJornada = excedeu;
            return Task.FromResult(dto);
        }

        public Task<IReadOnlyCollection<AcessoColaboradorDto>> ObterPorColaboradorIdAsync(int colaboradorId, CancellationToken cancellationToken = default)
        {
            var list = _repo.GetByColaboradorId(colaboradorId);
            var dtos = list.Select(a => a.ToDto()).ToList().AsReadOnly();
            return Task.FromResult((IReadOnlyCollection<AcessoColaboradorDto>)dtos);
        }
    }
}
