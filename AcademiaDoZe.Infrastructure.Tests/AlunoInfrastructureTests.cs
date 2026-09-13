// Thiago Augusto Ruskowski Waltrick
using Xunit;
using AcademiaDoZe.Infrastructure.Data;
using AcademiaDoZe.Infrastructure.Repositories;
using FluentAssertions;
using AcademiaDoZe.Domain.Entities;
using System.Linq;
using System;

namespace AcademiaDoZe.Infrastructure.Tests
{
    public class AlunoInfrastructureTests : TestBase
    {
        [Fact]
        public void Crud_Aluno_Operacoes_Basicas()
        {
            var provider = CreateProvider();
            var repo = new AlunoRepository(provider);

            // Garantir existência de tabelas
            var infraAssembly = typeof(AcademiaDoZe.Infrastructure.Data.DbProvider).Assembly;
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
                    try { provider.ExecuteScriptFileAsync(tmp).GetAwaiter().GetResult(); }
                    finally { try { System.IO.File.Delete(tmp); } catch { } }
                }
            }

            // Dados de evidência
            const string evidName = "Thiago Augusto Ruskowski Waltrick";
            const string evidComplemento = "Waltrick";
            var cpfVo = AcademiaDoZe.Domain.ValueObjects.Cpf.Criar("12345678902");

            bool evidExists = false;
            try
            {
                var existing = repo.GetByCpf(cpfVo.Value.Valor);
                if (existing != null)
                {
                    evidExists = true;
                }
            }
            catch
            {
                // Não existe
            }

            if (!evidExists)
            {
                try { provider.ExecuteNonQuery("DELETE FROM dbo.tb_aluno WHERE nome = @nome", new System.Data.Common.DbParameter[] { provider.CreateParameter("@nome", evidName) }); } catch { }
            }
            var emailVo = AcademiaDoZe.Domain.ValueObjects.Email.Criar("aluno@test.com");
            var telVo = AcademiaDoZe.Domain.ValueObjects.Telefone.Criar("11988888888");
            var log = Logradouro.Criar("Rua Teste", "Bairro", "SQLServer", "SP", null).Value;
            var endereco = Domain.ValueObjects.Endereco.Criar(log, "321", "Waltrick").Value;

            var alunoRes = Aluno.Criar(0, "Thiago Augusto Ruskowski Waltrick", cpfVo.Value, emailVo.Value, DateTime.UtcNow.AddYears(-20), telVo.Value, endereco);
            alunoRes.IsSuccess.Should().BeTrue();
            var aluno = alunoRes.Value!;

            if (!evidExists)
            {
                repo.Add(aluno);
            }
            else
            {
                aluno = repo.GetByCpf(cpfVo.Value.Valor);
            }

            var all = repo.GetAll();
            all.Should().NotBeNull();
            all.Any(a => a.Nome == "Thiago Augusto Ruskowski Waltrick").Should().BeTrue();

            var byCpf = repo.GetByCpf(cpfVo.Value.Valor);
            byCpf.Should().NotBeNull();
            byCpf.Nome.Should().Be("Thiago Augusto Ruskowski Waltrick");

            // Obter id inserido
            var ids = provider.ExecuteReader("SELECT TOP 1 Id FROM dbo.tb_aluno WHERE nome = @nome ORDER BY Id DESC", r => r.GetInt32(0), new System.Data.Common.DbParameter[] { provider.CreateParameter("@nome", aluno.Nome) });
            var id = ids.First();

            var byId = repo.GetById(id);
            byId.Should().NotBeNull();
            byId.Nome.Should().Be("Thiago Augusto Ruskowski Waltrick");

            // Atualizar
            var updatedRes = Aluno.Criar(id, "Thiago Augusto Ruskowski Waltrick", cpfVo.Value, emailVo.Value, DateTime.UtcNow.AddYears(-20), telVo.Value, endereco);
            updatedRes.IsSuccess.Should().BeTrue();
            var updated = updatedRes.Value!;
            repo.Update(updated);

            var afterUpdate = repo.GetById(id);
            afterUpdate.Should().NotBeNull();
            ((int)afterUpdate.GetType().GetProperty("Id")!.GetValue(afterUpdate)!).Should().Be(id);

            // Remover: não remover registro de evidência
            if (updated.Nome != evidName || (updated.Cpf?.Valor ?? string.Empty) != (cpfVo.Value.Valor))
            {
                repo.Remove(updated);
            }

            var remaining = repo.GetAll();
            remaining.Any(a => a.Nome == evidName).Should().BeTrue();
        }
    }
}
