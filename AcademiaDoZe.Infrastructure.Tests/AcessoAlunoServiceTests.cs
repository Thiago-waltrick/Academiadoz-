using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AcademiaDoZe.Application.Services;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;
using Xunit;

namespace AcademiaDoZe.Infrastructure.Tests
{
    public class AcessoAlunoServiceTests
    {
        [Fact]
        public async Task EntradaSemMatriculaERecusada()
        {
            var acessos = new AcessosFake();
            var service = new AcessoAlunoService(acessos, new MatriculasFake());
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.RegistrarEntradaAsync(1));
            Assert.Equal(0, acessos.Insercoes);
        }

        [Fact]
        public async Task FalhaDeConsultaNaoEConfundidaComMatriculaAusente()
        {
            var acessos = new AcessosFake();
            var service = new AcessoAlunoService(acessos, new MatriculasFake { Falhar = true });
            var erro = await Assert.ThrowsAsync<InvalidOperationException>(() => service.RegistrarEntradaAsync(1));
            Assert.Equal("Falha na consulta", erro.Message);
            Assert.Equal(0, acessos.Insercoes);
        }

        [Fact]
        public async Task EntradaAbertaNaoPermiteNovaEntrada()
        {
            var acessos = new AcessosFake { Aberto = true };
            var service = new AcessoAlunoService(acessos, new MatriculasFake { Ativa = true });
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.RegistrarEntradaAsync(1));
            Assert.Equal(0, acessos.Insercoes);
        }

        private sealed class AcessosFake : IAcessoAlunoRepository
        {
            public bool Aberto { get; set; }
            public int Insercoes { get; private set; }
            public void Add(AcessoAluno entity) => Insercoes++;
            public IReadOnlyCollection<AcessoAluno> GetByAlunoId(int id) => Aberto ? new[] { AcessoAluno.Criar(1, id).Value } : Array.Empty<AcessoAluno>();
            public IReadOnlyCollection<AcessoAluno> GetByAlunoIdAndDate(int id, DateTime date) => Array.Empty<AcessoAluno>();
            public IReadOnlyCollection<(int Id, int AlunoId, DateTime DataAcesso)> GetHistoricoLegado(DateTime inicio, DateTime fim) => Array.Empty<(int, int, DateTime)>();
            public IReadOnlyCollection<AcessoAluno> GetAll() => Array.Empty<AcessoAluno>();
            public AcessoAluno GetById(int id) => throw new NotImplementedException();
            public void Update(AcessoAluno entity) => throw new NotImplementedException();
            public void Remove(AcessoAluno entity) => throw new NotImplementedException();
        }

        private sealed class MatriculasFake : IMatriculaRepository
        {
            public bool Ativa { get; set; }
            public bool Falhar { get; set; }
            public bool PossuiMatriculaAtiva(int id) => Falhar ? throw new InvalidOperationException("Falha na consulta") : Ativa;
            public Matricula ObterMatriculaAtivaPorAluno(int id) => throw new NotImplementedException();
            public IReadOnlyCollection<Matricula> GetByAlunoId(int id) => Array.Empty<Matricula>();
            public IReadOnlyCollection<Matricula> ObterAtivas() => Array.Empty<Matricula>();
            public IReadOnlyCollection<Matricula> ObterVencendoEmDias(int dias) => Array.Empty<Matricula>();
            public IReadOnlyCollection<Matricula> Buscar(string termo) => Array.Empty<Matricula>();
            public IReadOnlyCollection<Matricula> GetAll() => Array.Empty<Matricula>();
            public Matricula GetById(int id) => throw new NotImplementedException();
            public void Add(Matricula entity) => throw new NotImplementedException();
            public void Update(Matricula entity) => throw new NotImplementedException();
            public void Remove(Matricula entity) => throw new NotImplementedException();
        }
    }
}
