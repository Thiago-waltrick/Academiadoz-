// Thiago Augusto Ruskowski Waltrick
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

            // Limpar possíveis registros anteriores com o mesmo nome/cidade
            var cepVo = AcademiaDoZe.Domain.ValueObjects.Cep.Criar("12345678");
            var toRemoveRes = Logradouro.Criar("Thiago Augusto Ruskowski Waltrick", "Waltrick", "SQLServer", "SP", cepVo.IsSuccess ? cepVo.Value : null);
            if (toRemoveRes.IsSuccess && toRemoveRes.Value != null)
            {
                try { repo.Remove(toRemoveRes.Value); } catch { }
            }

            var log = Logradouro.Criar("Thiago Augusto Ruskowski Waltrick", "Waltrick", "SQLServer", "SP", cepVo.IsSuccess ? cepVo.Value : null).Value;
            repo.Add(log);

            var all = repo.GetAll();
            all.Should().NotBeNull();
            all.Any(l => l.Nome == "Thiago Augusto Ruskowski Waltrick" && l.Cidade == "SQLServer").Should().BeTrue();

            // Obter Id do registro inserido via consulta e testar GetById
            var ids = provider.ExecuteReader("SELECT TOP 1 Id FROM dbo.tb_logradouro WHERE nome = @nome AND cidade = @cidade ORDER BY Id DESC", r => r.GetInt32(0), new System.Data.Common.DbParameter[] { provider.CreateParameter("@nome", log.Nome), provider.CreateParameter("@cidade", log.Cidade) });
            ids.Should().NotBeNull();
            ids.Count.Should().BeGreaterThan(0);
            var id = ids.First();
            id.Should().BeGreaterThan(0);

            var byId = repo.GetById(id);
            byId.Should().NotBeNull();
            byId.Nome.Should().Be("Thiago Augusto Ruskowski Waltrick");
            byId.Bairro.Should().Be("Waltrick");
            byId.Cidade.Should().Be("SQLServer");

            // Buscar por cep (deve retornar ao menos 1, já que inserimos com cep '12345678')
            var byCep = repo.BuscarPorCep("12345678");
            byCep.Should().NotBeNull();
            byCep.Any(l => l.Nome == "Thiago Augusto Ruskowski Waltrick").Should().BeTrue();

            // Atualizar (best-effort)
            var updated = Logradouro.Criar("Thiago Augusto Ruskowski Waltrick", "WaltrickUpdated", "SQLServer", "SP", cepVo.IsSuccess ? cepVo.Value : null).Value;
            repo.Update(updated);

            // Remover
            repo.Remove(updated);

            // Verificar remoção
            var remaining = repo.GetAll();
            remaining.Any(l => l.Nome == "Thiago Augusto Ruskowski Waltrick" && l.Cidade == "SQLServer").Should().BeFalse();
        }
    }
}
