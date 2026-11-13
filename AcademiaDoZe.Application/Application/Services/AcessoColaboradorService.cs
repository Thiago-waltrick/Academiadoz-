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
            var registros = _repo.GetByColaboradorId(colaboradorId);
            if (registros.Any(a => a.Saida == null))
                throw new InvalidOperationException("O colaborador já possui uma entrada em aberto.");
            var colaborador = _colRepo.GetById(colaboradorId);
            var limite = colaborador.Vinculo switch
            {
                ColaboradorVinculo.CLT => TimeSpan.FromHours(8),
                ColaboradorVinculo.Estagio => TimeSpan.FromHours(6),
                _ => throw new InvalidOperationException("Vínculo de colaborador inválido.")
            };
            if (CalcularTempoHoje(registros, DateTime.UtcNow) >= limite)
                throw new InvalidOperationException("Limite diário de horas para o vínculo atingido.");
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
            var registros = _repo.GetByColaboradorId(ent.ColaboradorId);
            double minutos = CalcularTempoHoje(registros, DateTime.UtcNow).TotalMinutes;

            // verificar jornada por vínculo
            var colaborador = _colRepo.GetById(ent.ColaboradorId);
            var limite = colaborador.Vinculo switch { ColaboradorVinculo.CLT => 8 * 60, ColaboradorVinculo.Estagio => 6 * 60, _ => throw new InvalidOperationException("Vínculo de colaborador inválido.") };
            var excedeu = minutos > limite;

            var dto = ent.ToDto();
            dto.MinutosTrabalhadosDia = minutos;
            dto.ExcedeuJornada = excedeu;
            return Task.FromResult(dto);
        }

        public static TimeSpan CalcularTempoHoje(IEnumerable<Domain.Entities.AcessoColaborador> registros, DateTime agoraUtc)
        {
            var inicio = DateTime.Today.ToUniversalTime();
            var fimDia = DateTime.Today.AddDays(1).ToUniversalTime();
            var fim = agoraUtc < fimDia ? agoraUtc : fimDia;
            long ticks = 0;
            foreach (var registro in registros)
            {
                var entrada = DateTime.SpecifyKind(registro.Entrada, DateTimeKind.Utc);
                var saida = registro.Saida.HasValue ? DateTime.SpecifyKind(registro.Saida.Value, DateTimeKind.Utc) : agoraUtc;
                var de = entrada > inicio ? entrada : inicio;
                var ate = saida < fim ? saida : fim;
                if (ate > de) ticks += (ate - de).Ticks;
            }
            return TimeSpan.FromTicks(ticks);
        }

        public Task<IReadOnlyCollection<AcessoColaboradorDto>> ObterPorColaboradorIdAsync(int colaboradorId, CancellationToken cancellationToken = default)
        {
            var list = _repo.GetByColaboradorId(colaboradorId);
            var dtos = list.Select(a => a.ToDto()).ToList().AsReadOnly();
            return Task.FromResult((IReadOnlyCollection<AcessoColaboradorDto>)dtos);
        }
    }
}
