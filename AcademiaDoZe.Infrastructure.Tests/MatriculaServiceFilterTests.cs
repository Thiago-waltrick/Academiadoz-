using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AcademiaDoZe.Application.Services;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Repositories;
using Xunit;

namespace AcademiaDoZe.Infrastructure.Tests
{
    public class MatriculaServiceFilterTests
    {
        [Fact]
        public async Task Consultas_Ativas_E_Vencendo_Delegam_Ao_Repositorio()
        {
            var repository = new FakeRepository();
            var service = new MatriculaService(repository);

            var ativas = await service.ObterAtivasAsync();
            var vencendo = await service.ObterVencendoEmDiasAsync(7);

            Assert.Single(ativas);
            Assert.Single(vencendo);
            Assert.Equal(7, repository.Dias);
            Assert.Equal(123, Assert.Single(ativas).Id);
            Assert.Equal(123, Assert.Single(vencendo).Id);
        }

        [Fact]
        public async Task Consultas_Respeitam_Cancelamento_E_Dias_Invalidos()
        {
            var repository = new FakeRepository();
            var service = new MatriculaService(repository);
            using var source = new CancellationTokenSource();
            source.Cancel();

            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => service.ObterAtivasAsync(source.Token));
            Assert.Throws<ArgumentOutOfRangeException>(() => { _ = service.ObterVencendoEmDiasAsync(-1); });
            Assert.Equal(0, repository.Consultas);
        }

        private sealed class FakeRepository : IMatriculaRepository
        {
            private readonly Matricula _matricula = Matricula.Criar(123, 1, MatriculaPlano.Mensal, DateTime.Today).Value;
            public int Dias { get; private set; }
            public int Consultas { get; private set; }
            public IReadOnlyCollection<Matricula> ObterAtivas() { Consultas++; return new[] { _matricula }; }
            public IReadOnlyCollection<Matricula> ObterVencendoEmDias(int dias) { Consultas++; Dias = dias; return new[] { _matricula }; }
            public IReadOnlyCollection<Matricula> GetByAlunoId(int alunoId) => Array.Empty<Matricula>();
            public IReadOnlyCollection<Matricula> Buscar(string termo) => Array.Empty<Matricula>();
            public Matricula ObterMatriculaAtivaPorAluno(int alunoId) => _matricula;
            public bool PossuiMatriculaAtiva(int alunoId) => false;
            public Matricula GetById(int id) => _matricula;
            public IReadOnlyCollection<Matricula> GetAll() => Array.Empty<Matricula>();
            public void Add(Matricula entity) { }
            public void Update(Matricula entity) { }
            public void Remove(Matricula entity) { }
        }
    }
}
