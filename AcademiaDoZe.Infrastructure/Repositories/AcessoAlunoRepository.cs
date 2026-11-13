// Thiago Augusto Ruskowski Waltrick
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Infrastructure.Data;
using AcademiaDoZe.Infrastructure.Exceptions;

namespace AcademiaDoZe.Infrastructure.Repositories
{
    public class AcessoAlunoRepository : BaseRepository, IAcessoAlunoRepository
    {
        public AcessoAlunoRepository(DbProvider provider) : base(provider) { }

        public void Add(AcessoAluno entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var sql = "INSERT INTO dbo.tb_acesso_aluno (aluno_id, entrada, saida) SELECT @aluno_id, @entrada, @saida WHERE NOT EXISTS (SELECT 1 FROM dbo.tb_acesso_aluno WITH (UPDLOCK, HOLDLOCK) WHERE aluno_id = @aluno_id AND saida IS NULL);";
            var p1 = _provider.CreateParameter("@aluno_id", entity.AlunoId);
            var p2 = _provider.CreateParameter("@entrada", entity.Entrada);
            var p3 = _provider.CreateParameter("@saida", entity.Saida ?? (object)DBNull.Value);
            if (ExecuteNonQuery(sql, p1, p2, p3) == 0)
                throw new InvalidOperationException("O aluno já possui uma entrada em aberto.");
        }

        public IReadOnlyCollection<AcessoAluno> GetAll()
        {
            var sql = "SELECT Id, aluno_id, entrada, saida FROM dbo.tb_acesso_aluno";
            var list = Query(sql, Map);
            return list.AsReadOnly();
        }

        public IReadOnlyCollection<(int Id, int AlunoId, DateTime DataAcesso)> GetHistoricoLegado(DateTime inicio, DateTime fim)
        {
            const string sql = "IF OBJECT_ID(N'dbo.tb_acesso', N'U') IS NOT NULL SELECT Id, aluno_id, data_acesso FROM dbo.tb_acesso WHERE aluno_id IS NOT NULL AND data_acesso >= @inicio AND data_acesso < @fim ELSE SELECT CAST(NULL AS INT), CAST(NULL AS INT), CAST(NULL AS DATETIME2) WHERE 1 = 0";
            return Query(sql, r => (r.GetInt32(0), r.GetInt32(1), r.GetDateTime(2)),
                _provider.CreateParameter("@inicio", inicio), _provider.CreateParameter("@fim", fim)).AsReadOnly();
        }

        public AcessoAluno GetById(int id)
        {
            var sql = "SELECT Id, aluno_id, entrada, saida FROM dbo.tb_acesso_aluno WHERE Id = @id";
            var p = _provider.CreateParameter("@id", id);
            var list = Query(sql, Map, p);
            if (list == null || list.Count == 0) throw new InfrastructureException("AcessoAluno não encontrado");
            return list[0];
        }

        public IReadOnlyCollection<AcessoAluno> GetByAlunoId(int alunoId)
        {
            var sql = "SELECT Id, aluno_id, entrada, saida FROM dbo.tb_acesso_aluno WHERE aluno_id = @aluno_id";
            var p = _provider.CreateParameter("@aluno_id", alunoId);
            var list = Query(sql, Map, p);
            return list.AsReadOnly();
        }

        public IReadOnlyCollection<AcessoAluno> GetByAlunoIdAndDate(int alunoId, DateTime date)
        {
            var sql = "SELECT Id, aluno_id, entrada, saida FROM dbo.tb_acesso_aluno WHERE aluno_id = @aluno_id AND CAST(entrada AS date) = @d";
            var p1 = _provider.CreateParameter("@aluno_id", alunoId);
            var p2 = _provider.CreateParameter("@d", date.Date);
            var list = Query(sql, Map, p1, p2);
            return list.AsReadOnly();
        }

        public void Remove(AcessoAluno entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var sql = "DELETE FROM dbo.tb_acesso_aluno WHERE Id = @id";
            var p = _provider.CreateParameter("@id", entity.Id);
            ExecuteNonQuery(sql, p);
        }

        public void Update(AcessoAluno entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var sql = "UPDATE dbo.tb_acesso_aluno SET aluno_id = @aluno_id, entrada = @entrada, saida = @saida WHERE Id = @id AND saida IS NULL";
            var p1 = _provider.CreateParameter("@aluno_id", entity.AlunoId);
            var p2 = _provider.CreateParameter("@entrada", entity.Entrada);
            var p3 = _provider.CreateParameter("@saida", entity.Saida ?? (object)DBNull.Value);
            var p4 = _provider.CreateParameter("@id", entity.Id);
            if (ExecuteNonQuery(sql, p1, p2, p3, p4) == 0)
                throw new InvalidOperationException("A saída já foi registrada ou o acesso não existe.");
        }

        private AcessoAluno Map(DbDataReader r)
        {
            var id = r.GetInt32(0);
            var alunoId = r.GetInt32(1);
            var entrada = r.GetDateTime(2);
            DateTime? saida = r.IsDBNull(3) ? null : r.GetDateTime(3);
            var res = AcessoAluno.Criar(id, alunoId);
            if (res.IsFailure) throw new InfrastructureException("Falha ao mapear AcessoAluno: " + string.Join(',', res.Notifications));
            var ent = res.Value;
            // ajustar datas
            var type = ent.GetType();
            type.GetProperty("Entrada")!.SetValue(ent, entrada);
            type.GetProperty("Saida")!.SetValue(ent, saida);
            return ent;
        }
    }
}
