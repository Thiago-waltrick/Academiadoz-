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
        private const string SelectMatricula =
            "SELECT m.Id, m.aluno_id, m.plano, m.data_inicio, m.data_fim, m.objetivo, m.restricoes, " +
            "m.obs_restricao, m.laudo_nome, m.laudo_content_type, m.laudo_conteudo, " +
            "a.nome, a.cpf, a.data_nascimento, a.foto_conteudo " +
            "FROM dbo.tb_matricula m INNER JOIN dbo.tb_aluno a ON a.Id = m.aluno_id ";

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
            const string sql = "INSERT INTO dbo.tb_matricula " +
                "(aluno_id, plano, data_inicio, data_fim, objetivo, restricoes, obs_restricao, laudo_nome, laudo_content_type, laudo_conteudo) " +
                "VALUES (@aluno_id, @plano, @data_inicio, @data_fim, @objetivo, @restricoes, @obs_restricao, @laudo_nome, @laudo_content_type, @laudo_conteudo);";
            ExecuteInsert(sql, CreateParameters(entity));
        }

        public IReadOnlyCollection<Matricula> ObterTodos()
        {
            var list = Query(SelectMatricula + "ORDER BY m.Id DESC", Map);
            return list.AsReadOnly();
        }

        public Matricula ObterPorId(int id)
        {
            var sql = SelectMatricula + "WHERE m.Id = @id";
            var p = _provider.CreateParameter("@id", id);
            var list = Query(sql, Map, p);
            if (list == null || list.Count == 0) throw new InfrastructureException("Matrícula não encontrada");
            return list[0];
        }

        public void Atualizar(Matricula entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            const string sql = "UPDATE dbo.tb_matricula SET aluno_id = @aluno_id, plano = @plano, data_inicio = @data_inicio, " +
                "data_fim = @data_fim, objetivo = @objetivo, restricoes = @restricoes, obs_restricao = @obs_restricao, " +
                "laudo_nome = @laudo_nome, laudo_content_type = @laudo_content_type, laudo_conteudo = @laudo_conteudo WHERE Id = @id";
            var parameters = CreateParameters(entity).ToList();
            parameters.Add(_provider.CreateParameter("@id", entity.Id));
            ExecuteNonQuery(sql, parameters.ToArray());
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
            var sql = SelectMatricula + "WHERE m.aluno_id = @aluno_id ORDER BY m.Id DESC";
            var p = _provider.CreateParameter("@aluno_id", alunoId);
            var list = Query(sql, Map, p);
            return list.AsReadOnly();
        }

        public IReadOnlyCollection<Matricula> Buscar(string termo)
        {
            if (string.IsNullOrWhiteSpace(termo)) return ObterTodos();

            var filtro = termo.Trim();
            var cpf = new string(filtro.Where(char.IsDigit).ToArray());
            const string sql = "SELECT m.Id, m.aluno_id, m.plano, m.data_inicio, m.data_fim, m.objetivo, m.restricoes, " +
                "m.obs_restricao, m.laudo_nome, m.laudo_content_type, m.laudo_conteudo, a.nome, a.cpf, a.data_nascimento, a.foto_conteudo " +
                "FROM dbo.tb_matricula m INNER JOIN dbo.tb_aluno a ON a.Id = m.aluno_id " +
                "WHERE a.nome LIKE @termo OR a.cpf LIKE @cpf OR CONVERT(VARCHAR(20), m.Id) LIKE @termo ORDER BY m.Id DESC";
            var list = Query(sql, Map,
                _provider.CreateParameter("@termo", $"%{filtro}%"),
                _provider.CreateParameter("@cpf", $"%{cpf}%"));
            return list.AsReadOnly();
        }

        public Matricula ObterMatriculaAtivaPorAluno(int alunoId)
        {
            var sql = SelectMatricula + "WHERE m.aluno_id = @aluno_id AND m.data_fim >= CONVERT(DATE, GETDATE()) ORDER BY m.data_inicio DESC";
            var p = _provider.CreateParameter("@aluno_id", alunoId);
            var list = Query(sql, Map, p);
            if (list == null || list.Count == 0) return null!;
            return list[0];
        }

        public bool PossuiMatriculaAtiva(int alunoId)
        {
            var sql = "SELECT COUNT(1) FROM dbo.tb_matricula WHERE aluno_id = @aluno_id AND data_fim >= CONVERT(DATE, GETDATE())";
            var p = _provider.CreateParameter("@aluno_id", alunoId);
            var count = ExecuteScalar<int>(sql, p);
            return count > 0;
        }

        public IReadOnlyCollection<Matricula> ObterAtivas()
        {
            var sql = SelectMatricula + "WHERE m.data_fim >= CONVERT(DATE, GETDATE())";
            var list = Query(sql, Map);
            return list.AsReadOnly();
        }

        public IReadOnlyCollection<Matricula> ObterVencendoEmDias(int dias)
        {
            var sql = SelectMatricula + "WHERE DATEDIFF(day, CONVERT(DATE, GETDATE()), m.data_fim) BETWEEN 0 AND @dias";
            var p = _provider.CreateParameter("@dias", dias);
            var list = Query(sql, Map, p);
            return list.AsReadOnly();
        }

        public IReadOnlyCollection<Matricula> ObterPorPlano(MatriculaPlano plano)
        {
            var sql = SelectMatricula + "WHERE m.plano = @plano";
            var p = _provider.CreateParameter("@plano", (int)plano);
            var list = Query(sql, Map, p);
            return list.AsReadOnly();
        }

        private DbParameter[] CreateParameters(Matricula entity) => new[]
        {
            _provider.CreateParameter("@aluno_id", entity.AlunoId),
            _provider.CreateParameter("@plano", (int)entity.Plano),
            _provider.CreateParameter("@data_inicio", entity.DataInicio.Date),
            _provider.CreateParameter("@data_fim", entity.DataFim?.Date ?? (object)DBNull.Value),
            _provider.CreateParameter("@objetivo", entity.Objetivo),
            _provider.CreateParameter("@restricoes", (int)entity.Restricoes),
            _provider.CreateParameter("@obs_restricao", entity.ObsRestricao),
            _provider.CreateParameter("@laudo_nome", entity.LaudoNome),
            _provider.CreateParameter("@laudo_content_type", entity.LaudoContentType),
            _provider.CreateParameter("@laudo_conteudo", entity.LaudoConteudo ?? (object)DBNull.Value)
        };

        private Matricula Map(DbDataReader r)
        {
            var id = r.GetInt32(0);
            var alunoId = r.GetInt32(1);
            var plano = (MatriculaPlano)r.GetInt32(2);
            var dataInicio = r.GetDateTime(3);
            var objetivo = r.IsDBNull(5) ? null : r.GetString(5);
            var restricoes = (MatriculaRestricoes)r.GetInt32(6);
            var obsRestricao = r.IsDBNull(7) ? null : r.GetString(7);
            var laudoNome = r.IsDBNull(8) ? null : r.GetString(8);
            var laudoContentType = r.IsDBNull(9) ? null : r.GetString(9);
            var laudoConteudo = r.IsDBNull(10) ? null : (byte[])r.GetValue(10);
            var alunoNome = r.IsDBNull(11) ? string.Empty : r.GetString(11);
            var alunoCpf = r.IsDBNull(12) ? null : r.GetString(12);
            DateTime? alunoDataNascimento = r.IsDBNull(13) ? null : r.GetDateTime(13);
            var alunoFoto = r.IsDBNull(14) ? null : (byte[])r.GetValue(14);

            var res = Matricula.Criar(id, alunoId, plano, dataInicio, objetivo, restricoes, obsRestricao,
                laudoNome, laudoContentType, laudoConteudo, alunoNome, alunoCpf, alunoDataNascimento, alunoFoto);
            if (res.IsFailure) throw new InfrastructureException("Falha ao mapear Matrícula: " + string.Join(',', res.Notifications));
            return res.Value!;
        }
    }
}
