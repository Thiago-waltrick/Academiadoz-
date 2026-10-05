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
    public class AcessoColaboradorRepository : BaseRepository, IAcessoColaboradorRepository
    {
        public AcessoColaboradorRepository(DbProvider provider) : base(provider) { }

        public void Add(AcessoColaborador entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var sql = "INSERT INTO dbo.tb_acesso_colaborador (colaborador_id, entrada, saida) VALUES (@colaborador_id, @entrada, @saida);";
            var p1 = _provider.CreateParameter("@colaborador_id", entity.ColaboradorId);
            var p2 = _provider.CreateParameter("@entrada", entity.Entrada);
            var p3 = _provider.CreateParameter("@saida", entity.Saida ?? (object)DBNull.Value);
            ExecuteNonQuery(sql, p1, p2, p3);
        }

        public IReadOnlyCollection<AcessoColaborador> GetAll()
        {
            var sql = "SELECT Id, colaborador_id, entrada, saida FROM dbo.tb_acesso_colaborador";
            var list = Query(sql, Map);
            return list.AsReadOnly();
        }

        public AcessoColaborador GetById(int id)
        {
            var sql = "SELECT Id, colaborador_id, entrada, saida FROM dbo.tb_acesso_colaborador WHERE Id = @id";
            var p = _provider.CreateParameter("@id", id);
            var list = Query(sql, Map, p);
            if (list == null || list.Count == 0) throw new InfrastructureException("AcessoColaborador não encontrado");
            return list[0];
        }

        public IReadOnlyCollection<AcessoColaborador> GetByColaboradorId(int colaboradorId)
        {
            var sql = "SELECT Id, colaborador_id, entrada, saida FROM dbo.tb_acesso_colaborador WHERE colaborador_id = @colaborador_id";
            var p = _provider.CreateParameter("@colaborador_id", colaboradorId);
            var list = Query(sql, Map, p);
            return list.AsReadOnly();
        }

        public IReadOnlyCollection<AcessoColaborador> GetByColaboradorIdAndDate(int colaboradorId, DateTime date)
        {
            var sql = "SELECT Id, colaborador_id, entrada, saida FROM dbo.tb_acesso_colaborador WHERE colaborador_id = @colaborador_id AND CAST(entrada AS date) = @d";
            var p1 = _provider.CreateParameter("@colaborador_id", colaboradorId);
            var p2 = _provider.CreateParameter("@d", date.Date);
            var list = Query(sql, Map, p1, p2);
            return list.AsReadOnly();
        }

        public void Remove(AcessoColaborador entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var sql = "DELETE FROM dbo.tb_acesso_colaborador WHERE Id = @id";
            var p = _provider.CreateParameter("@id", entity.Id);
            ExecuteNonQuery(sql, p);
        }

        public void Update(AcessoColaborador entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var sql = "UPDATE dbo.tb_acesso_colaborador SET colaborador_id = @colaborador_id, entrada = @entrada, saida = @saida WHERE Id = @id";
            var p1 = _provider.CreateParameter("@colaborador_id", entity.ColaboradorId);
            var p2 = _provider.CreateParameter("@entrada", entity.Entrada);
            var p3 = _provider.CreateParameter("@saida", entity.Saida ?? (object)DBNull.Value);
            var p4 = _provider.CreateParameter("@id", entity.Id);
            ExecuteNonQuery(sql, p1, p2, p3, p4);
        }

        private AcessoColaborador Map(DbDataReader r)
        {
            var id = r.GetInt32(0);
            var colaboradorId = r.GetInt32(1);
            var entrada = r.GetDateTime(2);
            DateTime? saida = r.IsDBNull(3) ? null : r.GetDateTime(3);
            var res = AcessoColaborador.Criar(id, colaboradorId);
            if (res.IsFailure) throw new InfrastructureException("Falha ao mapear AcessoColaborador: " + string.Join(',', res.Notifications));
            var ent = res.Value;
            var type = ent.GetType();
            type.GetProperty("Entrada")!.SetValue(ent, entrada);
            type.GetProperty("Saida")!.SetValue(ent, saida);
            return ent;
        }
    }
}
