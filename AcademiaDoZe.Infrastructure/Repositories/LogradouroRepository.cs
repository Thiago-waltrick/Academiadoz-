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
    public class LogradouroRepository : BaseRepository, ILogradouroRepository
    {
        public LogradouroRepository(DbProvider provider) : base(provider)
        {
        }

        public void Add(Logradouro entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var sql = "INSERT INTO dbo.tb_logradouro (nome, bairro, cidade, estado, cep) VALUES (@nome, @bairro, @cidade, @estado, @cep);";
            var p1 = _provider.CreateParameter("@nome", entity.Nome);
            var p2 = _provider.CreateParameter("@bairro", entity.Bairro ?? (object)DBNull.Value);
            var p3 = _provider.CreateParameter("@cidade", entity.Cidade ?? (object)DBNull.Value);
            var p4 = _provider.CreateParameter("@estado", entity.Estado ?? (object)DBNull.Value);
            var p5 = _provider.CreateParameter("@cep", entity.Cep?.Codigo ?? (object)DBNull.Value);

            // Executa INSERT sem solicitar SCOPE_IDENTITY para evitar dependência de retorno do id
            ExecuteNonQuery(sql, p1, p2, p3, p4, p5);
        }

        public IReadOnlyCollection<Logradouro> GetAll()
        {
            var sql = "SELECT Id, nome, bairro, cidade, estado, cep FROM dbo.tb_logradouro";
            var list = Query(sql, Map);
            return list.AsReadOnly();
        }

        public Logradouro GetById(int id)
        {
            var sql = "SELECT Id, nome, bairro, cidade, estado, cep FROM dbo.tb_logradouro WHERE Id = @id";
            var p = _provider.CreateParameter("@id", id);
            var list = Query(sql, Map, p);
            if (list == null || list.Count == 0) throw new InfrastructureException("Logradouro não encontrado");
            return list[0];
        }

        public void Remove(Logradouro entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            // Remove by matching unique fields: nome + cidade
            var sql = "DELETE FROM dbo.tb_logradouro WHERE nome = @nome AND cidade = @cidade";
            var p1 = _provider.CreateParameter("@nome", entity.Nome);
            var p2 = _provider.CreateParameter("@cidade", entity.Cidade ?? (object)DBNull.Value);
            ExecuteNonQuery(sql, p1, p2);
        }

        public void Update(Logradouro entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            // Update by matching nome + cidade (best-effort since domain Logradouro has no Id)
            var sql = "UPDATE dbo.tb_logradouro SET bairro = @bairro, estado = @estado, cep = @cep WHERE nome = @nome AND cidade = @cidade";
            var p1 = _provider.CreateParameter("@bairro", entity.Bairro ?? (object)DBNull.Value);
            var p2 = _provider.CreateParameter("@estado", entity.Estado ?? (object)DBNull.Value);
            var p3 = _provider.CreateParameter("@cep", entity.Cep?.Codigo ?? (object)DBNull.Value);
            var p4 = _provider.CreateParameter("@nome", entity.Nome);
            var p5 = _provider.CreateParameter("@cidade", entity.Cidade ?? (object)DBNull.Value);
            ExecuteNonQuery(sql, p1, p2, p3, p4, p5);
        }

        public IReadOnlyCollection<Logradouro> BuscarPorCep(string cep)
        {
            var sql = "SELECT Id, nome, bairro, cidade, estado, cep FROM dbo.tb_logradouro WHERE cep = @cep";
            var p = _provider.CreateParameter("@cep", cep ?? (object)DBNull.Value);
            var list = Query(sql, Map, p);
            return list.AsReadOnly();
        }

        private Logradouro Map(DbDataReader r)
        {
            var nome = r.IsDBNull(1) ? string.Empty : r.GetString(1);
            var bairro = r.IsDBNull(2) ? string.Empty : r.GetString(2);
            var cidade = r.IsDBNull(3) ? string.Empty : r.GetString(3);
            var estado = r.IsDBNull(4) ? string.Empty : r.GetString(4);
            var cep = r.IsDBNull(5) ? null : r.GetString(5);

            var cepVo = cep == null ? null : Domain.ValueObjects.Cep.Criar(cep).Value;
            var result = Logradouro.Criar(nome, bairro, cidade, estado, cepVo);
            if (result.IsFailure) throw new InfrastructureException("Falha ao mapear Logradouro: " + string.Join(',', result.Notifications));
            return result.Value!;
        }
    }
}
