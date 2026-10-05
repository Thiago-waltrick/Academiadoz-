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
            var sql = "INSERT INTO dbo.tb_acesso_aluno (aluno_id, entrada, saida) VALUES (@aluno_id, @entrada, @saida);";
            var p1 = _provider.CreateParameter("@aluno_id", entity.AlunoId);
            var p2 = _provider.CreateParameter("@entrada", entity.Entrada);
            var p3 = _provider.CreateParameter("@saida", entity.Saida ?? (object)DBNull.Value);
            ExecuteNonQuery(sql, p1, p2, p3);
        }

        public IReadOnlyCollection<AcessoAluno> GetAll()
        {
            var sql = "SELECT Id, aluno_id, entrada, saida FROM dbo.tb_acesso_aluno";
            var list = Query(sql, Map);
            return list.AsReadOnly();
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
            var sql = "UPDATE dbo.tb_acesso_aluno SET aluno_id = @aluno_id, entrada = @entrada, saida = @saida WHERE Id = @id";
            var p1 = _provider.CreateParameter("@aluno_id", entity.AlunoId);
            var p2 = _provider.CreateParameter("@entrada", entity.Entrada);
            var p3 = _provider.CreateParameter("@saida", entity.Saida ?? (object)DBNull.Value);
            var p4 = _provider.CreateParameter("@id", entity.Id);
            ExecuteNonQuery(sql, p1, p2, p3, p4);
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
