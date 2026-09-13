// Thiago Augusto Ruskowski Waltrick
using System;
using System.Linq;
using FluentAssertions;
using Xunit;
using AcademiaDoZe.Infrastructure.Data;
using AcademiaDoZe.Infrastructure.Repositories;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;

namespace AcademiaDoZe.Infrastructure.Tests
{
    public class MatriculaRepositoryTests : TestBase
    {
        private void EnsureScript(DbProvider provider)
        {
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
                    try { provider.ExecuteScriptFileAsync(tmp).GetAwaiter().GetResult(); }
                    finally { try { System.IO.File.Delete(tmp); } catch { } }
                }
            }
        }

        [Fact]
        public void Matricula_Crud_Operacoes_Basicas_Repository()
        {
            var provider = CreateProvider();
            EnsureScript(provider);

            // inserir logradouro e aluno
            var cep = DateTime.UtcNow.Ticks % 10000000 + new Random().Next(1000, 9999);
            var logSql = "INSERT INTO dbo.tb_logradouro (nome, bairro, cidade, estado, cep) VALUES (@nome, @bairro, @cidade, @estado, @cep);";
            var p1 = provider.CreateParameter("@nome", "LogR " + Guid.NewGuid());
            var p2 = provider.CreateParameter("@bairro", "B " + Guid.NewGuid());
            var p3 = provider.CreateParameter("@cidade", "C " + Guid.NewGuid());
            var p4 = provider.CreateParameter("@estado", "SP");
            var p5 = provider.CreateParameter("@cep", cep.ToString());
            var logId = provider.ExecuteInsert(logSql, new System.Data.Common.DbParameter[] { p1, p2, p3, p4, p5 });
            logId.Should().BeGreaterThan(0);

            var rand = new Random();
            var cpf = string.Concat(Enumerable.Range(0, 11).Select(_ => rand.Next(0, 10).ToString()));
            var email = $"user{Guid.NewGuid():N}@example.com";
            var idade = rand.Next(18, 61);
            var dataNascimento = DateTime.UtcNow.AddYears(-idade);

            var alunoSql = "INSERT INTO dbo.tb_aluno (nome, data_nascimento, cpf, email, telefone, end_numero, end_complemento, logradouro_nome, logradouro_bairro, logradouro_cidade, logradouro_estado, logradouro_cep) VALUES (@nome, @data_nascimento, @cpf, @email, @telefone, @end_numero, @end_complemento, @logradouro_nome, @logradouro_bairro, @logradouro_cidade, @logradouro_estado, @logradouro_cep);";
            var a1 = provider.CreateParameter("@nome", "AlunoR " + Guid.NewGuid());
            var a2 = provider.CreateParameter("@data_nascimento", dataNascimento);
            var a3 = provider.CreateParameter("@cpf", cpf);
            var a4 = provider.CreateParameter("@email", email);
            var a5 = provider.CreateParameter("@telefone", "(11) 99999-9999");
            var a6 = provider.CreateParameter("@end_numero", "10");
            var a7 = provider.CreateParameter("@end_complemento", "Comp");
            var a8 = provider.CreateParameter("@logradouro_nome", p1.Value);
            var a9 = provider.CreateParameter("@logradouro_bairro", p2.Value);
            var a10 = provider.CreateParameter("@logradouro_cidade", p3.Value);
            var a11 = provider.CreateParameter("@logradouro_estado", p4.Value);
            var a12 = provider.CreateParameter("@logradouro_cep", p5.Value);
            var alunoId = provider.ExecuteInsert(alunoSql, new System.Data.Common.DbParameter[] { a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12 });
            alunoId.Should().BeGreaterThan(0);

            // criar e adicionar via repositório
            var repo = new MatriculaRepository(provider);
            var dataInicio = DateTime.UtcNow;
            var cria = Matricula.Criar(0, alunoId, MatriculaPlano.Mensal, dataInicio);
            cria.IsSuccess.Should().BeTrue();
            var matricula = cria.Value!;

            repo.Add(matricula);

            // obter por aluno
            var porAluno = repo.ObterPorAluno(alunoId);
            porAluno.Should().NotBeNull();
            porAluno.Count.Should().BeGreaterThan(0);

            var ativo = repo.ObterMatriculaAtivaPorAluno(alunoId);
            ativo.Should().NotBeNull();

            repo.PossuiMatriculaAtiva(alunoId).Should().BeTrue();

            // localizar id inserido
            var ids = provider.ExecuteReader("SELECT TOP 1 Id FROM dbo.tb_matricula WHERE aluno_id = @id ORDER BY Id DESC", r => r.GetInt32(0), new System.Data.Common.DbParameter[] { provider.CreateParameter("@id", alunoId) });
            var id = ids.First();

            // atualizar
            var novoInicio = dataInicio.AddDays(1);
            var updRes = Matricula.Criar(id, alunoId, MatriculaPlano.Mensal, novoInicio);
            updRes.IsSuccess.Should().BeTrue();
            repo.Update(updRes.Value!);

            var byId = repo.ObterPorId(id);
            byId.Should().NotBeNull();
            byId.DataInicio.Should().BeCloseTo(novoInicio, TimeSpan.FromSeconds(2));

            // remover
            repo.Remove(byId);

            // após remoção, pode haver histórico; verificar que não possui matrícula ativa
            repo.PossuiMatriculaAtiva(alunoId).Should().BeFalse();
        }

        [Fact]
        public void Matricula_Consultas_Extras()
        {
            var provider = CreateProvider();
            EnsureScript(provider);

            // inserir logradouro e aluno
            var cep = DateTime.UtcNow.Ticks % 10000000 + new Random().Next(1000, 9999);
            var logSql = "INSERT INTO dbo.tb_logradouro (nome, bairro, cidade, estado, cep) VALUES (@nome, @bairro, @cidade, @estado, @cep);";
            var p1 = provider.CreateParameter("@nome", "LogX " + Guid.NewGuid());
            var p2 = provider.CreateParameter("@bairro", "B " + Guid.NewGuid());
            var p3 = provider.CreateParameter("@cidade", "C " + Guid.NewGuid());
            var p4 = provider.CreateParameter("@estado", "SP");
            var p5 = provider.CreateParameter("@cep", cep.ToString());
            var logId = provider.ExecuteInsert(logSql, new System.Data.Common.DbParameter[] { p1, p2, p3, p4, p5 });

            var rand = new Random();
            var cpf = string.Concat(Enumerable.Range(0, 11).Select(_ => rand.Next(0, 10).ToString()));
            var email = $"user{Guid.NewGuid():N}@example.com";
            var idade = rand.Next(18, 61);
            var dataNascimento = DateTime.UtcNow.AddYears(-idade);

            var alunoSql = "INSERT INTO dbo.tb_aluno (nome, data_nascimento, cpf, email, telefone, end_numero, end_complemento, logradouro_nome, logradouro_bairro, logradouro_cidade, logradouro_estado, logradouro_cep) VALUES (@nome, @data_nascimento, @cpf, @email, @telefone, @end_numero, @end_complemento, @logradouro_nome, @logradouro_bairro, @logradouro_cidade, @logradouro_estado, @logradouro_cep);";
            var a1 = provider.CreateParameter("@nome", "AlunoX " + Guid.NewGuid());
            var a2 = provider.CreateParameter("@data_nascimento", dataNascimento);
            var a3 = provider.CreateParameter("@cpf", cpf);
            var a4 = provider.CreateParameter("@email", email);
            var a5 = provider.CreateParameter("@telefone", "(11) 99999-9999");
            var a6 = provider.CreateParameter("@end_numero", "10");
            var a7 = provider.CreateParameter("@end_complemento", "Comp");
            var a8 = provider.CreateParameter("@logradouro_nome", p1.Value);
            var a9 = provider.CreateParameter("@logradouro_bairro", p2.Value);
            var a10 = provider.CreateParameter("@logradouro_cidade", p3.Value);
            var a11 = provider.CreateParameter("@logradouro_estado", p4.Value);
            var a12 = provider.CreateParameter("@logradouro_cep", p5.Value);
            var alunoId = provider.ExecuteInsert(alunoSql, new System.Data.Common.DbParameter[] { a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12 });

            var repo = new MatriculaRepository(provider);

            // criar matrículas com planos e datas diversas
            var now = DateTime.UtcNow;
            var m1 = Matricula.Criar(0, alunoId, MatriculaPlano.Mensal, now).Value!;
            var m2 = Matricula.Criar(0, alunoId, MatriculaPlano.Trimestral, now.AddMonths(-2)).Value!; // vencendo em próximo mês
            var m3 = Matricula.Criar(0, alunoId, MatriculaPlano.Anual, now.AddMonths(-11)).Value!; // vencendo em 1 mês

            repo.Add(m1);
            repo.Add(m2);
            repo.Add(m3);

            // Obter por plano
            var mensais = repo.ObterPorPlano(MatriculaPlano.Mensal);
            mensais.Should().NotBeNull();

            // Obter ativas
            var ativas = repo.ObterAtivas();
            ativas.Should().NotBeNull();
            ativas.Count.Should().BeGreaterThan(0);

            // Obter vencendo em 40 dias
            var vencendo = repo.ObterVencendoEmDias(40);
            vencendo.Should().NotBeNull();

            // limpeza: não remover registros (persistência desejada)
        }
    }
}
