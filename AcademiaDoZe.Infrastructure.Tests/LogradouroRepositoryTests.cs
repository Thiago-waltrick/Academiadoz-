// Thiago Augusto Ruskowski Waltrick
using System;
using Xunit;
using AcademiaDoZe.Infrastructure.Data;
using AcademiaDoZe.Infrastructure.Repositories;
using FluentAssertions;
using AcademiaDoZe.Domain.Entities;
using System.Linq;

namespace AcademiaDoZe.Infrastructure.Tests
{
    public class LogradouroRepositoryTests : TestBase
    {
        [Fact]
        public void Crud_Logradouro_Operacoes_Basicas()
        {
            var provider = CreateProvider();
            var repo = new LogradouroRepository(provider);

            // Garantir existência de tabela: carregar script do assembly da infraestrutura
            var infraAssembly = typeof(AcademiaDoZe.Infrastructure.Data.DbProvider).Assembly;
            string? resourceName = null;
            foreach (var n in infraAssembly.GetManifestResourceNames())
            {
                if (n.EndsWith("script_sqlserver.sql", System.StringComparison.OrdinalIgnoreCase))
                {
                    resourceName = n;
                    break;
                }
            }

            if (resourceName != null)
            {
                using var scriptStream = infraAssembly.GetManifestResourceStream(resourceName);
                if (scriptStream != null)
                {
                    using var sr = new System.IO.StreamReader(scriptStream);
                    var sql = sr.ReadToEnd();
                    var tmp = System.IO.Path.GetTempFileName();
                    System.IO.File.WriteAllText(tmp, sql);
                    try { provider.ExecuteScriptFileAsync(tmp).GetAwaiter().GetResult(); }
                    finally { try { System.IO.File.Delete(tmp); } catch { } }
                }
            }

            var token = System.Guid.NewGuid().ToString("N");
            var nome = $"Teste {token}";
            var cidade = $"SQLServer-{token}";
            var cepVo = AcademiaDoZe.Domain.ValueObjects.Cep.Criar("12345678");
            var cep = cepVo.IsSuccess ? cepVo.Value : null;
            var log = Logradouro.Criar(nome, "Waltrick", cidade, "SP", cep).Value;
            repo.Add(log);

            var ids = provider.ExecuteReader(
                "SELECT TOP (1) Id FROM dbo.tb_logradouro WHERE nome = @nome AND cidade = @cidade ORDER BY Id DESC",
                r => r.GetInt32(0),
                new System.Data.Common.DbParameter[] { provider.CreateParameter("@nome", nome), provider.CreateParameter("@cidade", cidade) });
            ids.Should().ContainSingle();
            var id = ids[0];
            id.Should().BeGreaterThan(0);

            try
            {
                var all = repo.GetAll();
                all.Should().Contain(logradouro => logradouro.Id == id);

                var byId = repo.GetById(id);
                byId.Nome.Should().Be(nome);
                byId.Bairro.Should().Be("Waltrick");
                byId.Cidade.Should().Be(cidade);

                var byCep = repo.BuscarPorCep("12345678");
                byCep.Should().Contain(logradouro => logradouro.Id == id);

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
    }
}
