using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Domain.Repositories;

namespace AcademiaDoZe.Application.Services
{
    public class AcessoService : IAcessoService
    {
        private readonly IAcessoAlunoRepository _alunos;
        private readonly IAcessoColaboradorRepository _colaboradores;
        private readonly IAcessoAlunoService _alunoService;
        private readonly IAcessoColaboradorService _colaboradorService;

        public AcessoService(IAcessoAlunoRepository alunos, IAcessoColaboradorRepository colaboradores, IAcessoAlunoService alunoService, IAcessoColaboradorService colaboradorService)
        {
            _alunos = alunos;
            _colaboradores = colaboradores;
            _alunoService = alunoService;
            _colaboradorService = colaboradorService;
        }

        public Task<IReadOnlyCollection<AcessoDto>> ObterAcessosHojeAsync(CancellationToken cancellationToken = default) => Task.Run<IReadOnlyCollection<AcessoDto>>(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            var inicio = DateTime.Today.ToUniversalTime();
            var fim = DateTime.Today.AddDays(1).ToUniversalTime();
            var alunos = _alunos.GetAll().Where(a => a.Entrada >= inicio && a.Entrada < fim).Select(a => new AcessoDto { Id = a.Id, AlunoId = a.AlunoId, Entrada = DateTime.SpecifyKind(a.Entrada, DateTimeKind.Utc), Saida = a.Saida.HasValue ? DateTime.SpecifyKind(a.Saida.Value, DateTimeKind.Utc) : null });
            var colaboradores = _colaboradores.GetAll().Where(a => a.Entrada >= inicio && a.Entrada < fim).Select(a => new AcessoDto { Id = a.Id, ColaboradorId = a.ColaboradorId, Entrada = DateTime.SpecifyKind(a.Entrada, DateTimeKind.Utc), Saida = a.Saida.HasValue ? DateTime.SpecifyKind(a.Saida.Value, DateTimeKind.Utc) : null });
            var legado = _alunos.GetHistoricoLegado(DateTime.Today, DateTime.Today.AddDays(1)).Select(a => new AcessoDto { Id = a.Id, AlunoId = a.AlunoId, Entrada = DateTime.SpecifyKind(a.DataAcesso, DateTimeKind.Local), HistoricoLegado = true });
            return alunos.Concat(colaboradores).Concat(legado).OrderByDescending(a => a.Entrada.ToUniversalTime()).ToList().AsReadOnly();
        }, cancellationToken);

        public Task<AcessoDto?> ObterEntradaAbertaAlunoAsync(int id, CancellationToken cancellationToken = default) => Task.Run(() => _alunos.GetByAlunoId(id).Where(a => a.Saida == null).Select(a => new AcessoDto { Id = a.Id, AlunoId = a.AlunoId, Entrada = DateTime.SpecifyKind(a.Entrada, DateTimeKind.Utc) }).OrderByDescending(a => a.Entrada).FirstOrDefault(), cancellationToken);
        public Task<AcessoDto?> ObterEntradaAbertaColaboradorAsync(int id, CancellationToken cancellationToken = default) => Task.Run(() => _colaboradores.GetByColaboradorId(id).Where(a => a.Saida == null).Select(a => new AcessoDto { Id = a.Id, ColaboradorId = a.ColaboradorId, Entrada = DateTime.SpecifyKind(a.Entrada, DateTimeKind.Utc) }).OrderByDescending(a => a.Entrada).FirstOrDefault(), cancellationToken);
        public Task<AcessoAlunoDto> RegistrarEntradaAlunoAsync(int id, CancellationToken cancellationToken = default) => Task.Run(async () => await _alunoService.RegistrarEntradaAsync(id, cancellationToken), cancellationToken);
        public Task<AcessoAlunoDto> RegistrarSaidaAlunoAsync(int id, CancellationToken cancellationToken = default) => Task.Run(async () => await _alunoService.RegistrarSaidaAsync(id, cancellationToken), cancellationToken);
        public Task<AcessoColaboradorDto> RegistrarEntradaColaboradorAsync(int id, CancellationToken cancellationToken = default) => Task.Run(async () => await _colaboradorService.RegistrarEntradaAsync(id, cancellationToken), cancellationToken);
        public Task<AcessoColaboradorDto> RegistrarSaidaColaboradorAsync(int id, CancellationToken cancellationToken = default) => Task.Run(async () => await _colaboradorService.RegistrarSaidaAsync(id, cancellationToken), cancellationToken);
        public Task<TimeSpan> ObterTempoColaboradorHojeAsync(int id, CancellationToken cancellationToken = default) => Task.Run(() => AcessoColaboradorService.CalcularTempoHoje(_colaboradores.GetByColaboradorId(id), DateTime.UtcNow), cancellationToken);
    }
}
