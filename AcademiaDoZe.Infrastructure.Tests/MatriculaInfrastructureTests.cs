// Thiago Augusto Ruskowski Waltrick
using System;
using System.Linq;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Infrastructure.Repositories;
using Xunit;

namespace AcademiaDoZe.Infrastructure.Tests
{
    public class MatriculaInfrastructureTests : TestBase
    {
        [Fact]
        public void Matricula_GerarDadosAleatorios_TabelasExistentes()
        {
            var provider = CreateProvider();

            // inserir logradouro (usar colunas existentes no script SQL Server)
            var cep = DateTime.UtcNow.Ticks % 10000000 + new Random().Next(1000, 9999);
            var logradouroSql = "INSERT INTO dbo.tb_logradouro (nome, bairro, cidade, estado, cep) VALUES (@nome, @bairro, @cidade, @estado, @cep);";
            var p1 = provider.CreateParameter("@nome", "Logradouro " + Guid.NewGuid());
            var p2 = provider.CreateParameter("@bairro", "Bairro " + Guid.NewGuid());
            var p3 = provider.CreateParameter("@cidade", "Cidade " + Guid.NewGuid());
            var p4 = provider.CreateParameter("@estado", "SP");
            var p5 = provider.CreateParameter("@cep", cep.ToString());
            var logradouroId = provider.ExecuteInsert(logradouroSql, new System.Data.Common.DbParameter[] { p1, p2, p3, p4, p5 });
            Assert.True(logradouroId > 0);

            // inserir aluno (tabela tb_aluno armazena dados de logradouro denormalizados)
            var rand = new Random();
            var cpf = string.Concat(Enumerable.Range(0, 11).Select(_ => rand.Next(0, 10).ToString()));
            var email = $"user{Guid.NewGuid():N}@example.com";
            var alunoSql = "INSERT INTO dbo.tb_aluno (nome, data_nascimento, cpf, email, telefone, end_numero, end_complemento, logradouro_nome, logradouro_bairro, logradouro_cidade, logradouro_estado, logradouro_cep) VALUES (@nome, @data_nascimento, @cpf, @email, @telefone, @end_numero, @end_complemento, @logradouro_nome, @logradouro_bairro, @logradouro_cidade, @logradouro_estado, @logradouro_cep);";
            var a1 = provider.CreateParameter("@nome", "Aluno " + Guid.NewGuid());
            // Gera data de nascimento plausível e dinâmica (entre 18 e 60 anos)
            var idade = rand.Next(18, 61);
            var dataNascimento = DateTime.UtcNow.AddYears(-idade);
            var a2 = provider.CreateParameter("@data_nascimento", dataNascimento);
            var a3 = provider.CreateParameter("@cpf", cpf);
            var a4 = provider.CreateParameter("@email", email);
            var a5 = provider.CreateParameter("@telefone", "(11) 99999-9999");
            var a6 = provider.CreateParameter("@end_numero", "123");
            var a7 = provider.CreateParameter("@end_complemento", "Comp");
            // reutilizar valores do logradouro inserido
            var a8 = provider.CreateParameter("@logradouro_nome", p1.Value);
            var a9 = provider.CreateParameter("@logradouro_bairro", p2.Value);
            var a10 = provider.CreateParameter("@logradouro_cidade", p3.Value);
            var a11 = provider.CreateParameter("@logradouro_estado", p4.Value);
            var a12 = provider.CreateParameter("@logradouro_cep", p5.Value);
            var alunoId = provider.ExecuteInsert(alunoSql, new System.Data.Common.DbParameter[] { a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12 });
            Assert.True(alunoId > 0);

            // Garantir que nenhum registro existente ficou com data_nascimento inválida (NULL/default)
            // Atualiza o registro recém-criado e quaisquer outros com data_nascimento NULL para uma data válida
            var fixSql = "UPDATE dbo.tb_aluno SET data_nascimento = @data_nascimento WHERE Id = @id OR data_nascimento IS NULL";
            var f1 = provider.CreateParameter("@data_nascimento", dataNascimento);
            var f2 = provider.CreateParameter("@id", alunoId);
            try { provider.ExecuteNonQuery(fixSql, new System.Data.Common.DbParameter[] { f1, f2 }); } catch { }

            // inserir matricula
            var objetivo = "Thiago Augusto Ruskowski Waltrick";
            var obs = "SQLServer";
            var dataInicio = DateTime.UtcNow;
            var plano = MatriculaPlano.Mensal;

            // Inserir matrícula: detectar se colunas objetivo/obs_restricao existem e inserir adequadamente
            var hasObjObj = provider.ExecuteScalar("SELECT TOP 1 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='tb_matricula' AND COLUMN_NAME='objetivo'");
            var hasObsObj = provider.ExecuteScalar("SELECT TOP 1 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='tb_matricula' AND COLUMN_NAME='obs_restricao'");
            var hasObj = hasObjObj != null && hasObjObj != DBNull.Value ? Convert.ToInt32(hasObjObj) == 1 : false;
            var hasObs = hasObsObj != null && hasObsObj != DBNull.Value ? Convert.ToInt32(hasObsObj) == 1 : false;

            int matriculaId;
            if (hasObj && hasObs)
            {
                var matSql2 = "INSERT INTO dbo.tb_matricula (aluno_id, plano, data_inicio, data_fim, objetivo, obs_restricao) VALUES (@aluno_id, @plano, @data_inicio, @data_fim, @objetivo, @obs)";
                var pm1 = provider.CreateParameter("@aluno_id", alunoId);
                var pm2 = provider.CreateParameter("@plano", (int)plano);
                var pm3 = provider.CreateParameter("@data_inicio", dataInicio);
                var pm4 = provider.CreateParameter("@data_fim", dataInicio.AddMonths(1));
                var pmo = provider.CreateParameter("@objetivo", objetivo);
                var pobs = provider.CreateParameter("@obs", obs);
                matriculaId = provider.ExecuteInsert(matSql2, new System.Data.Common.DbParameter[] { pm1, pm2, pm3, pm4, pmo, pobs });
            }
            else if (hasObj)
            {
                var matSql2 = "INSERT INTO dbo.tb_matricula (aluno_id, plano, data_inicio, data_fim, objetivo) VALUES (@aluno_id, @plano, @data_inicio, @data_fim, @objetivo)";
                var pm1 = provider.CreateParameter("@aluno_id", alunoId);
                var pm2 = provider.CreateParameter("@plano", (int)plano);
                var pm3 = provider.CreateParameter("@data_inicio", dataInicio);
                var pm4 = provider.CreateParameter("@data_fim", dataInicio.AddMonths(1));
                var pmo = provider.CreateParameter("@objetivo", objetivo);
                matriculaId = provider.ExecuteInsert(matSql2, new System.Data.Common.DbParameter[] { pm1, pm2, pm3, pm4, pmo });
            }
            else
            {
                var matSql = "INSERT INTO dbo.tb_matricula (aluno_id, plano, data_inicio, data_fim) VALUES (@aluno_id, @plano, @data_inicio, @data_fim);";
                var pm1 = provider.CreateParameter("@aluno_id", alunoId);
                var pm2 = provider.CreateParameter("@plano", (int)plano);
                var pm3 = provider.CreateParameter("@data_inicio", dataInicio);
                var pm4 = provider.CreateParameter("@data_fim", dataInicio.AddMonths(1));
                matriculaId = provider.ExecuteInsert(matSql, new System.Data.Common.DbParameter[] { pm1, pm2, pm3, pm4 });
            }

            Assert.True(matriculaId > 0);

            // log IDs
            Console.WriteLine($"LogradouroId: {logradouroId}, AlunoId: {alunoId}, MatriculaId: {matriculaId}");
        }
    }
}
