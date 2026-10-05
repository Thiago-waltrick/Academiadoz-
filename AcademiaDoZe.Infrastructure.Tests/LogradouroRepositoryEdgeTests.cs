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
        public void GetAll_Returns_Inserted_Row()
        {
            var provider = CreateProvider();
            var repo = new LogradouroRepository(provider);
            var token = Guid.NewGuid().ToString("N");
            var item = Logradouro.Criar($"Teste {token}", "Bairro", $"SQLServer-{token}", "SP", null).Value;
            var id = 0;

            try
            {
                repo.Add(item);
                id = Convert.ToInt32(provider.ExecuteScalar(
                    "SELECT TOP (1) Id FROM dbo.tb_logradouro WHERE nome = @nome AND cidade = @cidade ORDER BY Id DESC",
                    new[] { provider.CreateParameter("@nome", item.Nome), provider.CreateParameter("@cidade", item.Cidade) }));

                repo.GetAll().Should().Contain(logradouro => logradouro.Id == id);
            }
            finally
            {
                if (id > 0)
                    repo.Remove(Logradouro.Criar(item.Nome, item.Bairro, item.Cidade, item.Estado, item.Cep, id).Value);
            }
        }

        [Fact]
        public void BuscarPorCep_With_Null_Returns_Empty()
        {
            var provider = CreateProvider();
            var repo = new LogradouroRepository(provider);
            var found = repo.BuscarPorCep(null!);
            found.Should().NotBeNull();
            found.Count.Should().Be(0);
        }

        [Fact]
        public void Multiple_Inserts_GetAll_Returns_All()
        {
            var provider = CreateProvider();
            var repo = new LogradouroRepository(provider);
            var token = Guid.NewGuid().ToString("N");
            var cidade = $"SQLServer-{token}";

            var cepRes1 = AcademiaDoZe.Domain.ValueObjects.Cep.Criar("11111111");
            var cep1 = cepRes1.IsSuccess ? cepRes1.Value : null;
            var a = Logradouro.Criar($"Teste A {token}", "Waltrick", cidade, "SP", cep1).Value;
            var cepRes2 = AcademiaDoZe.Domain.ValueObjects.Cep.Criar("22222222");
            var cep2 = cepRes2.IsSuccess ? cepRes2.Value : null;
            var b = Logradouro.Criar($"Teste B {token}", "Waltrick", cidade, "SP", cep2).Value;

            var ids = new System.Collections.Generic.List<int>();

            try
            {
                repo.Add(a);
                repo.Add(b);
                ids = provider.ExecuteReader(
                    "SELECT Id FROM dbo.tb_logradouro WHERE cidade = @cidade AND nome IN (@nomeA, @nomeB)",
                    r => r.GetInt32(0),
                    new System.Data.Common.DbParameter[]
                    {
                        provider.CreateParameter("@cidade", cidade),
                        provider.CreateParameter("@nomeA", a.Nome),
                        provider.CreateParameter("@nomeB", b.Nome)
                    });

                ids.Should().HaveCount(2);
                repo.GetAll().Where(logradouro => ids.Contains(logradouro.Id)).Should().HaveCount(2);
            }
            finally
            {
                foreach (var id in ids)
                    repo.Remove(Logradouro.Criar("Registro de teste", "Waltrick", cidade, "SP", null, id).Value);
            }
        }
    }
}
