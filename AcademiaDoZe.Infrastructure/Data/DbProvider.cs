// Thiago Augusto Ruskowski Waltrick
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Threading.Tasks;

namespace AcademiaDoZe.Infrastructure.Data
{
    /// <summary>
    /// Abstrai a fábrica de provedores ADO.NET e fornece métodos utilitários para executar comandos.
    /// Suporta múltiplos provedores via DbProviderFactory.
    /// </summary>
    public enum DatabaseType
    {
        Unknown,
        SqlServer,
        MySql,
        Sqlite
    }

    public class DbProvider
    {
        public DatabaseType DatabaseType { get; }
        public string ProviderInvariantName { get; }
        public string ConnectionString { get; }
        private readonly DbProviderFactory _factory;

        public DbProvider(string providerInvariantName, string connectionString, DatabaseType? databaseType = null)
        {
            ProviderInvariantName = providerInvariantName ?? throw new ArgumentNullException(nameof(providerInvariantName));
            ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));

            // Suporte apenas para SQL Server neste projeto
            if (providerInvariantName.IndexOf("SqlClient", StringComparison.OrdinalIgnoreCase) >= 0 || providerInvariantName.IndexOf("SqlServer", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                _factory = Microsoft.Data.SqlClient.SqlClientFactory.Instance;
            }
            else
            {
                throw new InvalidOperationException($"Provedor não suportado neste projeto: '{providerInvariantName}'. Apenas SQL Server é permitido.");
            }

            if (databaseType.HasValue)
                DatabaseType = databaseType.Value;
            else if (providerInvariantName.IndexOf("SqlClient", StringComparison.OrdinalIgnoreCase) >= 0 || providerInvariantName.IndexOf("SqlServer", StringComparison.OrdinalIgnoreCase) >= 0)
                DatabaseType = DatabaseType.SqlServer;
            else if (providerInvariantName.IndexOf("MySql", StringComparison.OrdinalIgnoreCase) >= 0)
                DatabaseType = DatabaseType.MySql;
            else if (providerInvariantName.IndexOf("Sqlite", StringComparison.OrdinalIgnoreCase) >= 0)
                DatabaseType = DatabaseType.Sqlite;
            else
                DatabaseType = DatabaseType.Unknown;
        }

        public DbConnection CreateConnection()
        {
            var conn = _factory.CreateConnection() ?? throw new InvalidOperationException("Não foi possível criar a conexão a partir da factory");
            conn.ConnectionString = ConnectionString;
            return conn;
        }

        public DbCommand CreateCommand(string sql, DbConnection? connection = null, DbTransaction? transaction = null)
        {
            var cmd = _factory.CreateCommand() ?? throw new InvalidOperationException("Não foi possível criar o comando a partir da factory");
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            if (connection != null) cmd.Connection = connection;
            if (transaction != null) cmd.Transaction = transaction;
            return cmd;
        }

        public DbParameter CreateParameter(string name, object? value, DbType? dbType = null)
        {
            var p = _factory.CreateParameter() ?? throw new InvalidOperationException("Não foi possível criar o parâmetro a partir da factory");
            p.ParameterName = name;
            p.Value = value ?? DBNull.Value;
            if (dbType.HasValue) p.DbType = dbType.Value;
            return p;
        }

        public async Task<int> ExecuteNonQueryAsync(string sql, IEnumerable<DbParameter>? parameters = null)
        {
            using var conn = CreateConnection();
            await conn.OpenAsync();
            using var cmd = CreateCommand(sql, conn);
            AddParameters(cmd, parameters);
            return await cmd.ExecuteNonQueryAsync();
        }

        public int ExecuteNonQuery(string sql, IEnumerable<DbParameter>? parameters = null)
        {
            using var conn = CreateConnection();
            conn.Open();
            using var cmd = CreateCommand(sql, conn);
            AddParameters(cmd, parameters);
            return cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Executa um INSERT e retorna o id gerado quando suportado pelo banco.
        /// Para SQL Server utiliza SELECT CAST(SCOPE_IDENTITY() AS int).
        /// </summary>
        public async Task<int> ExecuteInsertAsync(string insertSql, IEnumerable<DbParameter>? parameters = null)
        {
            if (DatabaseType == DatabaseType.SqlServer)
            {
                var sql = insertSql.TrimEnd();
                // garante que haja retorno do id
                if (!sql.EndsWith(";")) sql += ";";
                sql += " SELECT CAST(SCOPE_IDENTITY() AS int);";
                var obj = await ExecuteScalarAsync(sql, parameters);
                if (obj == null) return 0;
                return Convert.ToInt32(obj);
            }
            else
            {
                var obj = await ExecuteScalarAsync(insertSql, parameters);
                if (obj == null) return 0;
                return Convert.ToInt32(obj);
            }
        }

        public int ExecuteInsert(string insertSql, IEnumerable<DbParameter>? parameters = null)
        {
            if (DatabaseType == DatabaseType.SqlServer)
            {
                var sql = insertSql.TrimEnd();
                if (!sql.EndsWith(";")) sql += ";";
                sql += " SELECT CAST(SCOPE_IDENTITY() AS int);";
                var obj = ExecuteScalar(sql, parameters);
                if (obj == null) return 0;
                return Convert.ToInt32(obj);
            }
            else
            {
                var obj = ExecuteScalar(insertSql, parameters);
                if (obj == null) return 0;
                return Convert.ToInt32(obj);
            }
        }

        public async Task<object?> ExecuteScalarAsync(string sql, IEnumerable<DbParameter>? parameters = null)
        {
            using var conn = CreateConnection();
            await conn.OpenAsync();
            using var cmd = CreateCommand(sql, conn);
            AddParameters(cmd, parameters);
            return await cmd.ExecuteScalarAsync();
        }

        public object? ExecuteScalar(string sql, IEnumerable<DbParameter>? parameters = null)
        {
            using var conn = CreateConnection();
            conn.Open();
            using var cmd = CreateCommand(sql, conn);
            AddParameters(cmd, parameters);
            return cmd.ExecuteScalar();
        }

        public async Task<List<T>> ExecuteReaderAsync<T>(string sql, Func<DbDataReader, T> map, IEnumerable<DbParameter>? parameters = null)
        {
            var results = new List<T>();
            using var conn = CreateConnection();
            await conn.OpenAsync();
            using var cmd = CreateCommand(sql, conn);
            AddParameters(cmd, parameters);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(map(reader));
            }
            return results;
        }

        public List<T> ExecuteReader<T>(string sql, Func<DbDataReader, T> map, IEnumerable<DbParameter>? parameters = null)
        {
            var results = new List<T>();
            using var conn = CreateConnection();
            conn.Open();
            using var cmd = CreateCommand(sql, conn);
            AddParameters(cmd, parameters);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                results.Add(map((DbDataReader)reader));
            }
            return results;
        }

        private static void AddParameters(DbCommand cmd, IEnumerable<DbParameter>? parameters)
        {
            if (parameters == null) return;
            foreach (var p in parameters)
            {
                cmd.Parameters.Add(p);
            }
        }

        /// <summary>
        /// Executa o script SQL completo do arquivo. Para provedores que não suportam múltiplas instruções em um único ExecuteNonQuery,
        /// o script é dividido por delimitadores simples.
        /// </summary>
        public async Task ExecuteScriptFileAsync(string scriptPath)
        {
            if (!File.Exists(scriptPath)) throw new FileNotFoundException("Script SQL não encontrado", scriptPath);
            var content = await File.ReadAllTextAsync(scriptPath);

            // Para SQL Server, respeitar 'GO' como separador
            if (ProviderInvariantName.Contains("SqlClient", StringComparison.OrdinalIgnoreCase) || ProviderInvariantName.Contains("SqlServer", StringComparison.OrdinalIgnoreCase))
            {
                var batches = content.Split(new[] { "\r\nGO\r\n", "\nGO\n", "\r\nGO\n", "\nGO\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var batch in batches)
                {
                    var sql = batch.Trim();
                    if (string.IsNullOrWhiteSpace(sql)) continue;
                    await ExecuteNonQueryAsync(sql);
                }
            }
            else
            {
                // Para MySQL/SQLite, dividir por ';' simples
                var statements = content.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var stmt in statements)
                {
                    var sql = stmt.Trim();
                    if (string.IsNullOrWhiteSpace(sql)) continue;
                    await ExecuteNonQueryAsync(sql);
                }
            }
        }
    }
}
