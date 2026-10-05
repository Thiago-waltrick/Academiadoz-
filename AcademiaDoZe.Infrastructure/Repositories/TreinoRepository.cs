using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Infrastructure.Data;

namespace AcademiaDoZe.Infrastructure.Repositories
{
    public sealed class TreinoRepository : ITreinoRepository
    {
        private readonly DbProvider _provider;

        public TreinoRepository(DbProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public async Task<TreinoSessao?> ObterSessaoAtivaAsync()
        {
            const string sql = "SELECT TOP (1) Id, iniciada_em, concluida_em FROM dbo.tb_treino_sessao WHERE concluida_em IS NULL ORDER BY Id DESC";
            var sessions = await _provider.ExecuteReaderAsync(sql, MapearSessao);
            return sessions.FirstOrDefault();
        }

        public Task<int> IniciarSessaoAsync(TreinoSessao sessao)
        {
            ArgumentNullException.ThrowIfNull(sessao);
            const string sql = "INSERT INTO dbo.tb_treino_sessao (iniciada_em) VALUES (@iniciada_em);";
            return _provider.ExecuteInsertAsync(sql, new[]
            {
                _provider.CreateParameter("@iniciada_em", sessao.IniciadaEm, DbType.DateTime2)
            });
        }

        public async Task<IReadOnlyCollection<TreinoExercicio>> ObterExerciciosAsync(int sessaoId)
        {
            const string sql = "SELECT Id, sessao_id, nome FROM dbo.tb_treino_exercicio WHERE sessao_id = @sessao_id ORDER BY Id";
            var exercises = await _provider.ExecuteReaderAsync(sql, MapearExercicio, new[]
            {
                _provider.CreateParameter("@sessao_id", sessaoId, DbType.Int32)
            });
            return exercises.AsReadOnly();
        }

        public Task<int> AdicionarExercicioAsync(TreinoExercicio exercicio)
        {
            ArgumentNullException.ThrowIfNull(exercicio);
            const string sql = "INSERT INTO dbo.tb_treino_exercicio (sessao_id, nome) VALUES (@sessao_id, @nome);";
            return _provider.ExecuteInsertAsync(sql, new[]
            {
                _provider.CreateParameter("@sessao_id", exercicio.SessaoId, DbType.Int32),
                _provider.CreateParameter("@nome", exercicio.Nome, DbType.String)
            });
        }

        public async Task<IReadOnlyCollection<TreinoSerie>> ObterSeriesAsync(int exercicioId)
        {
            const string sql = "SELECT Id, exercicio_id, numero, carga_kg, repeticoes, concluida FROM dbo.tb_treino_serie WHERE exercicio_id = @exercicio_id ORDER BY numero";
            var sets = await _provider.ExecuteReaderAsync(sql, MapearSerie, new[]
            {
                _provider.CreateParameter("@exercicio_id", exercicioId, DbType.Int32)
            });
            return sets.AsReadOnly();
        }

        public async Task<TreinoSerie?> ObterSerieAsync(int serieId)
        {
            const string sql = "SELECT Id, exercicio_id, numero, carga_kg, repeticoes, concluida FROM dbo.tb_treino_serie WHERE Id = @id";
            var sets = await _provider.ExecuteReaderAsync(sql, MapearSerie, new[]
            {
                _provider.CreateParameter("@id", serieId, DbType.Int32)
            });
            return sets.FirstOrDefault();
        }

        public Task<int> AdicionarSerieAsync(TreinoSerie serie)
        {
            ArgumentNullException.ThrowIfNull(serie);
            const string sql = "INSERT INTO dbo.tb_treino_serie (exercicio_id, numero, carga_kg, repeticoes, concluida) VALUES (@exercicio_id, @numero, @carga_kg, @repeticoes, @concluida);";
            return _provider.ExecuteInsertAsync(sql, CriarParametrosSerie(serie));
        }

        public async Task AtualizarSerieAsync(TreinoSerie serie)
        {
            ArgumentNullException.ThrowIfNull(serie);
            const string sql = "UPDATE dbo.tb_treino_serie SET carga_kg = @carga_kg, repeticoes = @repeticoes, concluida = @concluida WHERE Id = @id";
            var parameters = CriarParametrosSerie(serie).ToList();
            parameters.Add(_provider.CreateParameter("@id", serie.Id, DbType.Int32));
            var changed = await _provider.ExecuteNonQueryAsync(sql, parameters);
            if (changed == 0)
                throw new InvalidOperationException("A série não foi encontrada para atualização.");
        }

        public async Task FinalizarSessaoAsync(TreinoSessao sessao)
        {
            ArgumentNullException.ThrowIfNull(sessao);
            if (!sessao.ConcluidaEm.HasValue)
                throw new InvalidOperationException("A sessão precisa estar concluída antes de ser persistida.");

            const string sql = "UPDATE dbo.tb_treino_sessao SET concluida_em = @concluida_em WHERE Id = @id AND concluida_em IS NULL";
            var changed = await _provider.ExecuteNonQueryAsync(sql, new[]
            {
                _provider.CreateParameter("@concluida_em", sessao.ConcluidaEm.Value, DbType.DateTime2),
                _provider.CreateParameter("@id", sessao.Id, DbType.Int32)
            });
            if (changed == 0)
                throw new InvalidOperationException("O treino ativo não foi encontrado para finalização.");
        }

        private IEnumerable<DbParameter> CriarParametrosSerie(TreinoSerie serie)
        {
            return new[]
            {
                _provider.CreateParameter("@exercicio_id", serie.ExercicioId, DbType.Int32),
                _provider.CreateParameter("@numero", serie.Numero, DbType.Int32),
                _provider.CreateParameter("@carga_kg", serie.CargaKg, DbType.Decimal),
                _provider.CreateParameter("@repeticoes", serie.Repeticoes, DbType.Int32),
                _provider.CreateParameter("@concluida", serie.Concluida, DbType.Boolean)
            };
        }

        private static TreinoSessao MapearSessao(DbDataReader reader)
        {
            var iniciadaEm = DateTime.SpecifyKind(reader.GetDateTime(1), DateTimeKind.Utc);
            var concluidaEm = reader.IsDBNull(2)
                ? (DateTime?)null
                : DateTime.SpecifyKind(reader.GetDateTime(2), DateTimeKind.Utc);
            return TreinoSessao.Reconstituir(reader.GetInt32(0), iniciadaEm, concluidaEm);
        }

        private static TreinoExercicio MapearExercicio(DbDataReader reader) =>
            TreinoExercicio.Reconstituir(reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2));

        private static TreinoSerie MapearSerie(DbDataReader reader) =>
            TreinoSerie.Reconstituir(
                reader.GetInt32(0),
                reader.GetInt32(1),
                reader.GetInt32(2),
                reader.GetDecimal(3),
                reader.GetInt32(4),
                reader.GetBoolean(5));
    }
}
