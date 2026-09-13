// Thiago Augusto Ruskowski Waltrick
using System;
using System.Linq;
using Xunit;
using FluentAssertions;
using AcademiaDoZe.Infrastructure.Data;
using AcademiaDoZe.Infrastructure.Repositories;
using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Infrastructure.Tests
{
    public class LogradouroRepositoryEdgeTests : TestBase
    {
        [Fact]
        public void Add_Null_Throws()
        {
            var provider = CreateProvider();
            var repo = new LogradouroRepository(provider);
            Action act = () => repo.Add(null!);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Update_Null_Throws()
        {
            var provider = CreateProvider();
            var repo = new LogradouroRepository(provider);
            Action act = () => repo.Update(null!);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Remove_Null_Throws()
        {
            var provider = CreateProvider();
            var repo = new LogradouroRepository(provider);
            Action act = () => repo.Remove(null!);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetAll_When_Empty_Returns_Empty()
        {
            var provider = CreateProvider();
            var repo = new LogradouroRepository(provider);
            provider.ExecuteNonQuery("DELETE FROM dbo.tb_logradouro");
            var all = repo.GetAll();
            all.Should().NotBeNull();
            all.Count.Should().Be(0);
        }

        [Fact]
        public void BuscarPorCep_With_Null_Returns_Empty()
        {
            var provider = CreateProvider();
            var repo = new LogradouroRepository(provider);
            provider.ExecuteNonQuery("DELETE FROM dbo.tb_logradouro");
            var found = repo.BuscarPorCep(null!);
            found.Should().NotBeNull();
            found.Count.Should().Be(0);
        }

        [Fact]
        public void Multiple_Inserts_GetAll_Returns_All()
        {
            var provider = CreateProvider();
            var repo = new LogradouroRepository(provider);
            provider.ExecuteNonQuery("DELETE FROM dbo.tb_logradouro");

            var cepRes1 = AcademiaDoZe.Domain.ValueObjects.Cep.Criar("11111111");
            var cep1 = cepRes1.IsSuccess ? cepRes1.Value : null;
            var a = Logradouro.Criar("Thiago Augusto Ruskowski Waltrick", "Waltrick", "SQLServer", "SP", cep1).Value;
            var cepRes2 = AcademiaDoZe.Domain.ValueObjects.Cep.Criar("22222222");
            var cep2 = cepRes2.IsSuccess ? cepRes2.Value : null;
            var b = Logradouro.Criar("Thiago Augusto Ruskowski Waltrick", "Waltrick", "SQLServer", "SP", cep2).Value;

            repo.Add(a);
            repo.Add(b);

            var all = repo.GetAll();
            all.Should().NotBeNull();
            all.Count.Should().BeGreaterOrEqualTo(2);
            all.All(l => l.Nome == "Thiago Augusto Ruskowski Waltrick" && l.Bairro == "Waltrick" && l.Cidade == "SQLServer").Should().BeTrue();
        }
    }
}
