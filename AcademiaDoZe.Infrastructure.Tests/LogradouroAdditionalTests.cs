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
    public class LogradouroAdditionalTests : TestBase
    {
        [Fact]
        public void Deve_Limpar_Tabela_Logradouro()
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

                repo.GetAll().Any(logradouro => logradouro.Id == id).Should().BeTrue();
            }
            finally
            {
                if (id > 0)
                    repo.Remove(Logradouro.Criar(item.Nome, item.Bairro, item.Cidade, item.Estado, item.Cep, id).Value);
            }
        }

        [Fact]
        public void Deve_Retornar_Erro_Quando_GetById_Nao_Existir()
        {
            var provider = CreateProvider();
            var repo = new LogradouroRepository(provider);

            Action act = () => repo.GetById(-1);
            act.Should().Throw<AcademiaDoZe.Infrastructure.Exceptions.InfrastructureException>();
        }

        [Fact]
        public void Deve_Inserir_E_Atualizar_E_Remover_Logradouro()
        {
            var provider = CreateProvider();
            var repo = new LogradouroRepository(provider);

            var token = Guid.NewGuid().ToString("N");
            var nome = $"Teste {token}";
            var cidade = $"SQLServer-{token}";
            var cepRes = AcademiaDoZe.Domain.ValueObjects.Cep.Criar("12345678");
            var cep = cepRes.IsSuccess ? cepRes.Value : null;

            var log = Logradouro.Criar(nome, "Waltrick", cidade, "SP", cep).Value;
            repo.Add(log);
            var ids = provider.ExecuteReader(
                "SELECT TOP (1) Id FROM dbo.tb_logradouro WHERE nome = @nome AND cidade = @cidade ORDER BY Id DESC",
                r => r.GetInt32(0),
                new System.Data.Common.DbParameter[] { provider.CreateParameter("@nome", nome), provider.CreateParameter("@cidade", cidade) });
            ids.Should().ContainSingle();
            var id = ids[0];

            try
            {
                var updated = Logradouro.Criar(nome, "WaltrickUpdated", cidade, "SP", cep, id).Value;
                repo.Update(updated);

                repo.GetById(id).Bairro.Should().Be("WaltrickUpdated");

                repo.Remove(updated);
                Action getRemoved = () => repo.GetById(id);
                getRemoved.Should().Throw<AcademiaDoZe.Infrastructure.Exceptions.InfrastructureException>();
            }
            finally
            {
                repo.Remove(Logradouro.Criar(nome, "Waltrick", cidade, "SP", cep, id).Value);
            }
        }

        [Fact]
        public void Deve_Buscar_Varios_Por_Cep()
        {
            var provider = CreateProvider();
            var repo = new LogradouroRepository(provider);
            var token = Guid.NewGuid().ToString("N");
            var cidade = $"SQLServer-{token}";
            var cepRes = AcademiaDoZe.Domain.ValueObjects.Cep.Criar("99999999");
            var cep = cepRes.IsSuccess ? cepRes.Value : null;

            var a = Logradouro.Criar($"Teste A {token}", "Bairro A", cidade, "SP", cep).Value;
            var b = Logradouro.Criar($"Teste B {token}", "Bairro B", cidade, "SP", cep).Value;
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
                var found = repo.BuscarPorCep("99999999");
                found.Where(logradouro => ids.Contains(logradouro.Id)).Should().HaveCount(2);
                found.Where(logradouro => ids.Contains(logradouro.Id)).All(logradouro => logradouro.Cidade == cidade).Should().BeTrue();
            }
            finally
            {
                foreach (var id in ids)
                    repo.Remove(Logradouro.Criar("Registro de teste", "Bairro", cidade, "SP", cep, id).Value);
            }
        }
    }
}
