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
            // limpar tabela para estado conhecido
            provider.ExecuteNonQuery("DELETE FROM dbo.tb_logradouro");

            var obj = provider.ExecuteScalar("SELECT COUNT(1) FROM dbo.tb_logradouro");
            var count = obj == null || obj == DBNull.Value ? 0 : Convert.ToInt32(obj);
            count.Should().Be(0);
        }

        [Fact]
        public void Deve_Retornar_Erro_Quando_GetById_Nao_Existir()
        {
            var provider = CreateProvider();
            var repo = new LogradouroRepository(provider);
            // garantir tabela limpa
            provider.ExecuteNonQuery("DELETE FROM dbo.tb_logradouro");

            Action act = () => repo.GetById(999999);
            act.Should().Throw<AcademiaDoZe.Infrastructure.Exceptions.InfrastructureException>();
        }

        [Fact]
        public void Deve_Inserir_E_Atualizar_E_Remover_Logradouro()
        {
            var provider = CreateProvider();
            var repo = new LogradouroRepository(provider);

            // limpar antes
            provider.ExecuteNonQuery("DELETE FROM dbo.tb_logradouro");

            var cepRes = AcademiaDoZe.Domain.ValueObjects.Cep.Criar("12345678");
            var cep = cepRes.IsSuccess ? cepRes.Value : null;

            var log = Logradouro.Criar("Thiago Augusto Ruskowski Waltrick", "Waltrick", "SQLServer", "SP", cep).Value;
            repo.Add(log);

            var all = repo.GetAll();
            all.Any(l => l.Nome == "Thiago Augusto Ruskowski Waltrick" && l.Cidade == "SQLServer").Should().BeTrue();

            // atualizar bairro
            var updated = Logradouro.Criar("Thiago Augusto Ruskowski Waltrick", "WaltrickUpdated", "SQLServer", "SP", cep).Value;
            repo.Update(updated);

            var byIdList = provider.ExecuteReader("SELECT TOP 1 bairro FROM dbo.tb_logradouro WHERE nome = @nome AND cidade = @cidade ORDER BY Id DESC",
                r => r.IsDBNull(0) ? string.Empty : r.GetString(0), new System.Data.Common.DbParameter[] { provider.CreateParameter("@nome", updated.Nome), provider.CreateParameter("@cidade", updated.Cidade) });

            byIdList.Should().NotBeNull();
            byIdList.Count.Should().BeGreaterThan(0);
            byIdList[0].Should().Be("WaltrickUpdated");

            // remover
            repo.Remove(updated);
            var remaining = repo.GetAll();
            remaining.Any(l => l.Nome == "Thiago Augusto Ruskowski Waltrick" && l.Cidade == "SQLServer").Should().BeFalse();
        }

        [Fact]
        public void Deve_Buscar_Varios_Por_Cep()
        {
            var provider = CreateProvider();
            var repo = new LogradouroRepository(provider);
            provider.ExecuteNonQuery("DELETE FROM dbo.tb_logradouro");

            var cepRes = AcademiaDoZe.Domain.ValueObjects.Cep.Criar("99999999");
            var cep = cepRes.IsSuccess ? cepRes.Value : null;

            var a = Logradouro.Criar("Thiago Augusto Ruskowski Waltrick", "Waltrick", "SQLite", "SP", cep).Value;
            var b = Logradouro.Criar("Thiago Augusto Ruskowski Waltrick", "Waltrick2", "SQLite", "SP", cep).Value;
            repo.Add(a);
            repo.Add(b);

            var found = repo.BuscarPorCep("99999999");
            found.Should().NotBeNull();
            found.Count.Should().BeGreaterOrEqualTo(2);
            found.All(l => l.Cidade == "SQLite").Should().BeTrue();
        }
    }
}
