// Thiago Augusto Ruskowski Waltrick
using System;
using Xunit;
using AcademiaDoZe.Infrastructure.Data;
using AcademiaDoZe.Infrastructure.Repositories;
using FluentAssertions;
using System.Data.Common;

namespace AcademiaDoZe.Infrastructure.Tests
{
    public class LogradouroInfrastructureTests : TestBase
    {
        [Fact]
        public void Deve_Criar_Logradouro_SqlServer()
        {
            var provider = CreateProvider();
            // Cria tabela se necessário
            // Tentar carregar o script embutido do assembly da infraestrutura (não do assembly de testes)
            var infraAssembly = typeof(DbProvider).Assembly;
            string? resourceName = null;
            foreach (var n in infraAssembly.GetManifestResourceNames())
            {
                if (n.EndsWith("script_sqlserver.sql", StringComparison.OrdinalIgnoreCase))
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
                    try
                    {
                        provider.ExecuteScriptFileAsync(tmp).GetAwaiter().GetResult();
                    }
                    finally
                    {
                        try { System.IO.File.Delete(tmp); } catch { }
                    }
                }
            }

            // Inserir registro de logradouro via LogradouroRepository (registro ficará permanentemente no banco)
            var repo = new LogradouroRepository(provider);
            var cepRes = AcademiaDoZe.Domain.ValueObjects.Cep.Criar("12345678");
            var cep = cepRes.IsSuccess ? cepRes.Value : null;

            var log = AcademiaDoZe.Domain.Entities.Logradouro.Criar("Thiago Augusto Ruskowski Waltrick", "Waltrick", "SQLServer", "SP", cep).Value;
            // Inserção via repositório — NÃO removeremos este registro no teste
            repo.Add(log);

            // Verificar existência usando consulta simples
            var list = provider.ExecuteReader("SELECT TOP 1 nome, bairro, cidade FROM dbo.tb_logradouro WHERE nome = @nome AND cidade = @cidade ORDER BY Id DESC", r => new { Nome = r.IsDBNull(0) ? string.Empty : r.GetString(0), Bairro = r.IsDBNull(1) ? string.Empty : r.GetString(1), Cidade = r.IsDBNull(2) ? string.Empty : r.GetString(2) }, new DbParameter[] { provider.CreateParameter("@nome", "Thiago Augusto Ruskowski Waltrick"), provider.CreateParameter("@cidade", "SQLServer") });
            list.Should().NotBeNull();
            list.Count.Should().BeGreaterThan(0);
            list[0].Cidade.Should().Be("SQLServer");
        }
    }
}
