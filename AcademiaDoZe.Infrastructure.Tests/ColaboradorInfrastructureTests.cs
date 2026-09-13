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
    public class ColaboradorInfrastructureTests : TestBase
    {
        [Fact]
        public void Crud_Colaborador_Operacoes_Basicas()
        {
            var provider = CreateProvider();
            var repo = new ColaboradorRepository(provider);

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
            // CPF usado para identificação única do registro de evidência
            var cpfVo = AcademiaDoZe.Domain.ValueObjects.Cpf.Criar("12345678901");
            // Antes de inserir, verificar se já existe um registro com esse CPF (evita duplicação).
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
                // Não encontrou: será criado abaixo
            }

            // Se não existir evidência, limpar registros antigos com o mesmo nome para manter idempotência
            if (!evidExists)
            {
                try { provider.ExecuteNonQuery("DELETE FROM dbo.tb_colaborador WHERE nome = @nome", new System.Data.Common.DbParameter[] { provider.CreateParameter("@nome", evidName) }); } catch { }
            }























            var emailVo = AcademiaDoZe.Domain.ValueObjects.Email.Criar("colab@test.com");
            var telVo = AcademiaDoZe.Domain.ValueObjects.Telefone.Criar("11999999999");
            var log = Logradouro.Criar("Rua Teste", "Bairro", "SQLServer", "SP", null).Value;
            var endereco = Domain.ValueObjects.Endereco.Criar(log, "123", "Waltrick").Value;

            var colRes = Colaborador.Criar(0, "Thiago Augusto Ruskowski Waltrick", cpfVo.Value, emailVo.Value, DateTime.UtcNow.AddYears(-25), telVo.Value, endereco, Domain.Enums.ColaboradorTipo.Instrutor, Domain.Enums.ColaboradorVinculo.CLT, DateTime.UtcNow.AddDays(-10));
            colRes.IsSuccess.Should().BeTrue();
            var col = colRes.Value!;

            if (!evidExists)
            {
                repo.Add(col);
            }
            else
            {
                // reutilizar registro existente; recarregar via GetByCpf para obter Id
                col = repo.GetByCpf(cpfVo.Value.Valor);
            }

            var all = repo.GetAll();
            all.Should().NotBeNull();
            all.Any(c => c.Nome == "Thiago Augusto Ruskowski Waltrick").Should().BeTrue();

            var byCpf = repo.GetByCpf(cpfVo.Value.Valor);
            byCpf.Should().NotBeNull();
            byCpf.Nome.Should().Be("Thiago Augusto Ruskowski Waltrick");

            // Obter id inserido
            var ids = provider.ExecuteReader("SELECT TOP 1 Id FROM dbo.tb_colaborador WHERE nome = @nome ORDER BY Id DESC", r => r.GetInt32(0), new System.Data.Common.DbParameter[] { provider.CreateParameter("@nome", col.Nome) });
            var id = ids.First();

            var byId = repo.GetById(id);
            byId.Should().NotBeNull();
            byId.Nome.Should().Be("Thiago Augusto Ruskowski Waltrick");

            // Atualizar
            var updatedRes = Colaborador.Criar(id, "Thiago Augusto Ruskowski Waltrick", cpfVo.Value, emailVo.Value, DateTime.UtcNow.AddYears(-25), telVo.Value, endereco, Domain.Enums.ColaboradorTipo.Administrador, Domain.Enums.ColaboradorVinculo.CLT, DateTime.UtcNow.AddDays(-10));
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
            // O registro de evidência deve permanecer
            remaining.Any(c => c.Nome == evidName).Should().BeTrue();
        }
    }
}
