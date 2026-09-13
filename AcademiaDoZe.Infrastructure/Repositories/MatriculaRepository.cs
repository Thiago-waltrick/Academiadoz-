// Thiago Augusto Ruskowski Waltrick
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Infrastructure.Data;
using AcademiaDoZe.Infrastructure.Exceptions;

namespace AcademiaDoZe.Infrastructure.Repositories
{
    public class MatriculaRepository : BaseRepository, IMatriculaRepository
    {
        public MatriculaRepository(DbProvider provider) : base(provider)
        {
        }

        // Implementação dos métodos conforme contrato IRepository<T> e IMatriculaRepository
        public void Add(Matricula entity) => Adicionar(entity);

        public IReadOnlyCollection<Matricula> GetAll() => ObterTodos();

        public Matricula GetById(int id) => ObterPorId(id);

        public void Update(Matricula entity) => Atualizar(entity);

        public void Remove(Matricula entity) => Remover(entity);

        public IReadOnlyCollection<Matricula> GetByAlunoId(int alunoId) => ObterPorAluno(alunoId);

        // Métodos em português (padrão do repositório solicitado)
        public void Adicionar(Matricula entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            // Inserir incluindo objetivo e obs_restricao quando as colunas existirem
            var hasObjetivo = ExecuteScalar<int>("SELECT TOP 1 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='tb_matricula' AND COLUMN_NAME='objetivo'") == 1;
            var hasObs = ExecuteScalar<int>("SELECT TOP 1 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='tb_matricula' AND COLUMN_NAME='obs_restricao'") == 1;

            string sql;
            if (hasObjetivo && hasObs)
            {
                sql = "INSERT INTO dbo.tb_matricula (aluno_id, plano, data_inicio, data_fim, objetivo, obs_restricao) VALUES (@aluno_id, @plano, @data_inicio, @data_fim, @objetivo, @obs);";
            }
            else if (hasObjetivo)
            {
                sql = "INSERT INTO dbo.tb_matricula (aluno_id, plano, data_inicio, data_fim, objetivo) VALUES (@aluno_id, @plano, @data_inicio, @data_fim, @objetivo);";
            }
            else
            {
                sql = "INSERT INTO dbo.tb_matricula (aluno_id, plano, data_inicio, data_fim) VALUES (@aluno_id, @plano, @data_inicio, @data_fim);";
            }

            var p1 = _provider.CreateParameter("@aluno_id", entity.AlunoId);
            var p2 = _provider.CreateParameter("@plano", (int)entity.Plano);
            var p3 = _provider.CreateParameter("@data_inicio", entity.DataInicio);
            var p4 = _provider.CreateParameter("@data_fim", entity.DataFim ?? (object)DBNull.Value);

            var parameters = new System.Collections.Generic.List<DbParameter> { p1, p2, p3, p4 };

            // adicionar parâmetros objetivo/obs se presentes
            if (hasObjetivo)
            {
                var val = entity.GetType().GetProperty("Objetivo")?.GetValue(entity);
                if (val == null || (val is string s && string.IsNullOrWhiteSpace(s)))
                    val = "Thiago Augusto Ruskowski Waltrick"; // valor padrão seguro
                var obj = _provider.CreateParameter("@objetivo", val ?? (object)DBNull.Value);
                parameters.Add(obj);
            }
            if (hasObs)
            {
                var val = entity.GetType().GetProperty("ObsRestricao")?.GetValue(entity);
                if (val == null || (val is string ss && string.IsNullOrWhiteSpace(ss)))
                    val = "SQLServer"; // valor padrão seguro
                var obs = _provider.CreateParameter("@obs", val ?? (object)DBNull.Value);
                parameters.Add(obs);
            }

            var id = ExecuteInsert(sql, parameters.ToArray());
            // não usamos o id aqui para atualizar a entidade de domínio (imutável no repositório)
        }

        public IReadOnlyCollection<Matricula> ObterTodos()
        {
            var sql = "SELECT Id, aluno_id, plano, data_inicio, data_fim, " +
                      "CASE WHEN COLUMNPROPERTY(object_id('dbo.tb_matricula'), 'objetivo', 'AllowsNull') IS NOT NULL THEN objetivo ELSE NULL END AS objetivo, " +
                      "CASE WHEN COLUMNPROPERTY(object_id('dbo.tb_matricula'), 'obs_restricao', 'AllowsNull') IS NOT NULL THEN obs_restricao ELSE NULL END AS obs_restricao " +
                      "FROM dbo.tb_matricula";
            var list = Query(sql, MapWithOptionalFields);
            return list.AsReadOnly();
        }

        public Matricula ObterPorId(int id)
        {
            var sql = "SELECT Id, aluno_id, plano, data_inicio, data_fim, " +
                      "CASE WHEN COLUMNPROPERTY(object_id('dbo.tb_matricula'), 'objetivo', 'AllowsNull') IS NOT NULL THEN objetivo ELSE NULL END AS objetivo, " +
                      "CASE WHEN COLUMNPROPERTY(object_id('dbo.tb_matricula'), 'obs_restricao', 'AllowsNull') IS NOT NULL THEN obs_restricao ELSE NULL END AS obs_restricao " +
                      "FROM dbo.tb_matricula WHERE Id = @id";
            var p = _provider.CreateParameter("@id", id);
            var list = Query(sql, MapWithOptionalFields, p);
            if (list == null || list.Count == 0) throw new InfrastructureException("Matrícula não encontrada");
            return list[0];
        }

        public void Atualizar(Matricula entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var sql = "UPDATE dbo.tb_matricula SET aluno_id = @aluno_id, plano = @plano, data_inicio = @data_inicio, data_fim = @data_fim WHERE Id = @id";
            var p1 = _provider.CreateParameter("@aluno_id", entity.AlunoId);
            var p2 = _provider.CreateParameter("@plano", (int)entity.Plano);
            var p3 = _provider.CreateParameter("@data_inicio", entity.DataInicio);
            var p4 = _provider.CreateParameter("@data_fim", entity.DataFim ?? (object)DBNull.Value);
            var p5 = _provider.CreateParameter("@id", entity.Id);
            ExecuteNonQuery(sql, p1, p2, p3, p4, p5);
        }

        public void Remover(Matricula entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var sql = "DELETE FROM dbo.tb_matricula WHERE Id = @id";
            var p = _provider.CreateParameter("@id", entity.Id);
            ExecuteNonQuery(sql, p);
        }

        public IReadOnlyCollection<Matricula> ObterPorAluno(int alunoId)
        {
            var sql = "SELECT Id, aluno_id, plano, data_inicio, data_fim FROM dbo.tb_matricula WHERE aluno_id = @aluno_id";
            var p = _provider.CreateParameter("@aluno_id", alunoId);
            var list = Query(sql, Map, p);
            return list.AsReadOnly();
        }

        public Matricula ObterMatriculaAtivaPorAluno(int alunoId)
        {
            var sql = "SELECT TOP 1 Id, aluno_id, plano, data_inicio, data_fim FROM dbo.tb_matricula WHERE aluno_id = @aluno_id AND (data_fim IS NULL OR data_fim >= GETDATE()) ORDER BY data_inicio DESC";
            var p = _provider.CreateParameter("@aluno_id", alunoId);
            var list = Query(sql, Map, p);
            if (list == null || list.Count == 0) return null!;
            return list[0];
        }

        public bool PossuiMatriculaAtiva(int alunoId)
        {
            var sql = "SELECT COUNT(1) FROM dbo.tb_matricula WHERE aluno_id = @aluno_id AND (data_fim IS NULL OR data_fim >= GETDATE())";
            var p = _provider.CreateParameter("@aluno_id", alunoId);
            var count = ExecuteScalar<int>(sql, p);
            return count > 0;
        }

        public IReadOnlyCollection<Matricula> ObterAtivas()
        {
            var sql = "SELECT Id, aluno_id, plano, data_inicio, data_fim FROM dbo.tb_matricula WHERE data_fim IS NULL OR data_fim >= GETDATE()";
            var list = Query(sql, Map);
            return list.AsReadOnly();
        }

        public IReadOnlyCollection<Matricula> ObterVencendoEmDias(int dias)
        {
            var sql = "SELECT Id, aluno_id, plano, data_inicio, data_fim FROM dbo.tb_matricula WHERE data_fim IS NOT NULL AND DATEDIFF(day, GETDATE(), data_fim) BETWEEN 0 AND @dias";
            var p = _provider.CreateParameter("@dias", dias);
            var list = Query(sql, Map, p);
            return list.AsReadOnly();
        }

        public IReadOnlyCollection<Matricula> ObterPorPlano(MatriculaPlano plano)
        {
            var sql = "SELECT Id, aluno_id, plano, data_inicio, data_fim FROM dbo.tb_matricula WHERE plano = @plano";
            var p = _provider.CreateParameter("@plano", (int)plano);
            var list = Query(sql, Map, p);
            return list.AsReadOnly();
        }

        // Map leitura -> entidade (campos básicos)
        private Matricula Map(DbDataReader r)
        {
            var id = r.GetInt32(0);
            var alunoId = r.GetInt32(1);
            var plano = (MatriculaPlano)r.GetInt32(2);
            var dataInicio = r.GetDateTime(3);
            DateTime? dataFim = r.IsDBNull(4) ? null : r.GetDateTime(4);

            var res = Matricula.Criar(id, alunoId, plano, dataInicio);
            if (res.IsFailure) throw new InfrastructureException("Falha ao mapear Matrícula: " + string.Join(',', res.Notifications));
            return res.Value!;
        }

        // Map que considera campos opcionais objetivo/obs_restricao quando presentes
        private Matricula MapWithOptionalFields(DbDataReader r)
        {
            // colunas: Id(0), aluno_id(1), plano(2), data_inicio(3), data_fim(4), objetivo(5), obs_restricao(6)
            var id = r.GetInt32(0);
            var alunoId = r.GetInt32(1);
            var plano = (MatriculaPlano)r.GetInt32(2);
            var dataInicio = r.GetDateTime(3);
            DateTime? dataFim = r.IsDBNull(4) ? null : r.GetDateTime(4);

            // objetivo e obs são apenas persistidos/consumidos pelo banco; domínio Matricula não possui essas propriedades
            // Mapeamos a entidade Matricula normalmente
            var res = Matricula.Criar(id, alunoId, plano, dataInicio);
            if (res.IsFailure) throw new InfrastructureException("Falha ao mapear Matrícula: " + string.Join(',', res.Notifications));
            return res.Value!;
        }
    }
}
