// Thiago Augusto Ruskowski Waltrick
using Xunit;
using FluentAssertions;
using AcademiaDoZe.Infrastructure.Data;
using AcademiaDoZe.Infrastructure.Repositories;
using System.Data.Common;

namespace AcademiaDoZe.Infrastructure.Tests
{
    public class LogradouroPersistentInsertTest : TestBase
    {
        [Fact]
        public void Insere_Logradouro_Permanente_No_SqlServer()
        {
            var provider = CreateProvider();

            // Garantir que tabelas existem (script embutido no assembly da infraestrutura)
            var infraAssembly = typeof(DbProvider).Assembly;
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

            // Criar e inserir Logradouro usando o repositório (registro permanecerá no banco)
            var repo = new LogradouroRepository(provider);
            var cepRes = AcademiaDoZe.Domain.ValueObjects.Cep.Criar("12345678");
            var cep = cepRes.IsSuccess ? cepRes.Value : null;

            var log = AcademiaDoZe.Domain.Entities.Logradouro.Criar("Thiago Augusto Ruskowski Waltrick", "Waltrick", "SQLServer", "SP", cep).Value;
            repo.Add(log);

            // Verificar existência do registro inserido
            var rows = provider.ExecuteReader("SELECT TOP 1 nome, bairro, cidade FROM dbo.tb_logradouro WHERE nome = @nome AND cidade = @cidade ORDER BY Id DESC",
                r => new { Nome = r.IsDBNull(0) ? string.Empty : r.GetString(0), Bairro = r.IsDBNull(1) ? string.Empty : r.GetString(1), Cidade = r.IsDBNull(2) ? string.Empty : r.GetString(2) },
                new DbParameter[] { provider.CreateParameter("@nome", "Thiago Augusto Ruskowski Waltrick"), provider.CreateParameter("@cidade", "SQLServer") });

            rows.Should().NotBeNull();
            rows.Count.Should().BeGreaterThan(0);
            rows[0].Nome.Should().Be("Thiago Augusto Ruskowski Waltrick");
            rows[0].Bairro.Should().Be("Waltrick");
            rows[0].Cidade.Should().Be("SQLServer");
        }
    }
}
