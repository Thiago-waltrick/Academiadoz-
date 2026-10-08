// Thiago Augusto Ruskowski Waltrick
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using AcademiaDoZe.Infrastructure.Data;

namespace AcademiaDoZe.Infrastructure.Repositories
{
    /// <summary>
    /// Repositório base com helpers para execução ADO.NET usando DbProvider.
    /// Fornece métodos auxiliares para consultas, leitura e execução de comandos.
    /// </summary>
    public abstract class BaseRepository
    {
        protected readonly DbProvider _provider;

        protected BaseRepository(DbProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        protected async Task<int> ExecuteNonQueryAsync(string sql, params DbParameter[] parameters)
        {
            try
            {
                return await _provider.ExecuteNonQueryAsync(sql, parameters);
            }
            catch (Exception ex)
            {
                throw new Exceptions.InfrastructureException("Falha ao executar comando no banco.", ex);
            }
        }

        protected async Task<int> ExecuteInsertAsync(string insertSql, params DbParameter[] parameters)
        {
            try
            {
                return await _provider.ExecuteInsertAsync(insertSql, parameters);
            }
            catch (Exception ex)
            {
                throw new Exceptions.InfrastructureException("Falha ao inserir registro no banco.", ex);
            }
        }

        protected int ExecuteNonQuery(string sql, params DbParameter[] parameters)
        {
            try
            {
                return _provider.ExecuteNonQuery(sql, parameters);
            }
            catch (Exception ex)
            {
                throw new Exceptions.InfrastructureException("Falha ao executar comando no banco.", ex);
            }
        }

        protected int ExecuteInsert(string insertSql, params DbParameter[] parameters)
        {
            try
            {
                return _provider.ExecuteInsert(insertSql, parameters);
            }
            catch (Exception ex)
            {
                throw new Exceptions.InfrastructureException("Falha ao inserir registro no banco.", ex);
            }
        }

        protected async Task<T?> ExecuteScalarAsync<T>(string sql, params DbParameter[] parameters)
        {
            try
            {
                var obj = await _provider.ExecuteScalarAsync(sql, parameters);
                if (obj == null || obj == DBNull.Value) return default;
                return (T)Convert.ChangeType(obj, typeof(T));
            }
            catch (Exception ex)
            {
                throw new Exceptions.InfrastructureException("Falha ao executar scalar no banco.", ex);
            }
        }

        protected T? ExecuteScalar<T>(string sql, params DbParameter[] parameters)
        {
            try
            {
                var obj = _provider.ExecuteScalar(sql, parameters);
                if (obj == null || obj == DBNull.Value) return default;
                return (T)Convert.ChangeType(obj, typeof(T));
            }
            catch (Exception ex)
            {
                throw new Exceptions.InfrastructureException("Falha ao executar scalar no banco.", ex);
            }
        }

        protected async Task<List<T>> QueryAsync<T>(string sql, Func<DbDataReader, T> map, params DbParameter[] parameters)
        {
            try
            {
                return await _provider.ExecuteReaderAsync(sql, map, parameters);
            }
            catch (Exception ex)
            {
                throw new Exceptions.InfrastructureException("Falha ao consultar dados no banco.", ex);
            }
        }

        protected List<T> Query<T>(string sql, Func<DbDataReader, T> map, params DbParameter[] parameters)
        {
            try
            {
                return _provider.ExecuteReader(sql, map, parameters);
            }
            catch (Exception ex)
            {
                throw new Exceptions.InfrastructureException("Falha ao consultar dados no banco.", ex);
            }
        }
    }
}