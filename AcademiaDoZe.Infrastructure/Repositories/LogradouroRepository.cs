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

            var sql = $"INSERT INTO {TableName} (nome, bairro, cidade, estado, cep) VALUES (@nome, @bairro, @cidade, @estado, @cep);";
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
            var sql = $"SELECT Id, nome, bairro, cidade, estado, cep FROM {TableName}";
            var list = Query(sql, Map);
            return list.AsReadOnly();
        }

        public Logradouro GetById(int id)
        {
            var sql = $"SELECT Id, nome, bairro, cidade, estado, cep FROM {TableName} WHERE Id = @id";
            var p = _provider.CreateParameter("@id", id);
            var list = Query(sql, Map, p);
            if (list == null || list.Count == 0) throw new InfrastructureException("Logradouro não encontrado");
            return list[0];
        }

        public void Remove(Logradouro entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var sql = $"DELETE FROM {TableName} WHERE Id = @id";
            ExecuteNonQuery(sql, _provider.CreateParameter("@id", entity.Id));
        }

        public void Update(Logradouro entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var sql = $"UPDATE {TableName} SET nome = @nome, bairro = @bairro, cidade = @cidade, estado = @estado, cep = @cep WHERE Id = @id";
            var p1 = _provider.CreateParameter("@nome", entity.Nome);
            var p2 = _provider.CreateParameter("@bairro", entity.Bairro);
            var p3 = _provider.CreateParameter("@cidade", entity.Cidade);
            var p4 = _provider.CreateParameter("@estado", entity.Estado);
            var p5 = _provider.CreateParameter("@cep", entity.Cep?.Codigo);
            var p6 = _provider.CreateParameter("@id", entity.Id);
            ExecuteNonQuery(sql, p1, p2, p3, p4, p5, p6);
        }

        public IReadOnlyCollection<Logradouro> BuscarPorCep(string cep)
        {
            var sql = $"SELECT Id, nome, bairro, cidade, estado, cep FROM {TableName} WHERE cep = @cep";
            var p = _provider.CreateParameter("@cep", cep ?? (object)DBNull.Value);
            var list = Query(sql, Map, p);
            return list.AsReadOnly();
        }

        private const string TableName = "dbo.tb_logradouro";

        private Logradouro Map(DbDataReader r)
        {
            var nome = r.IsDBNull(1) ? string.Empty : r.GetString(1);
            var bairro = r.IsDBNull(2) ? string.Empty : r.GetString(2);
            var cidade = r.IsDBNull(3) ? string.Empty : r.GetString(3);
            var estado = r.IsDBNull(4) ? string.Empty : r.GetString(4);
            var cep = r.IsDBNull(5) ? null : r.GetString(5);

            var cepVo = cep == null ? null : Domain.ValueObjects.Cep.Criar(cep).Value;
            var id = r.IsDBNull(0) ? 0 : r.GetInt32(0);
            var result = Logradouro.Criar(nome, bairro, cidade, estado, cepVo, id);
            if (result.IsFailure) throw new InfrastructureException("Falha ao mapear Logradouro: " + string.Join(',', result.Notifications));
            return result.Value!;
        }
    }
}
