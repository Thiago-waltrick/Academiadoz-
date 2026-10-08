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
    public class AlunoRepository : BaseRepository, IAlunoRepository
    {
        public AlunoRepository(DbProvider provider) : base(provider)
        {
        }

        public void Add(Aluno entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var sql = "INSERT INTO dbo.tb_aluno (nome, data_nascimento, cpf, email, telefone, end_numero, end_complemento, logradouro_nome, logradouro_bairro, logradouro_cidade, logradouro_estado, logradouro_cep, foto_conteudo) VALUES (@nome, @data_nascimento, @cpf, @email, @telefone, @end_numero, @end_complemento, @logradouro_nome, @logradouro_bairro, @logradouro_cidade, @logradouro_estado, @logradouro_cep, @foto_conteudo);";

            var p1 = _provider.CreateParameter("@nome", entity.Nome);
            var p2 = _provider.CreateParameter("@data_nascimento", entity.DataNascimento == default ? (object)DBNull.Value : entity.DataNascimento);
            var p3 = _provider.CreateParameter("@cpf", entity.Cpf?.Valor ?? (object)DBNull.Value);
            var p4 = _provider.CreateParameter("@email", entity.Email?.Endereco ?? (object)DBNull.Value);
            var p5 = _provider.CreateParameter("@telefone", entity.Telefone?.Numero ?? (object)DBNull.Value);

            var numero = entity.Endereco?.Numero ?? string.Empty;
            var complemento = entity.Endereco?.Complemento ?? string.Empty;
            var log = entity.Endereco?.Logradouro;

            var p6 = _provider.CreateParameter("@end_numero", numero == null ? (object)DBNull.Value : numero);
            var p7 = _provider.CreateParameter("@end_complemento", complemento == null ? (object)DBNull.Value : complemento);
            var p8 = _provider.CreateParameter("@logradouro_nome", log?.Nome ?? (object)DBNull.Value);
            var p9 = _provider.CreateParameter("@logradouro_bairro", log?.Bairro ?? (object)DBNull.Value);
            var p10 = _provider.CreateParameter("@logradouro_cidade", log?.Cidade ?? (object)DBNull.Value);
            var p11 = _provider.CreateParameter("@logradouro_estado", log?.Estado ?? (object)DBNull.Value);
            var p12 = _provider.CreateParameter("@logradouro_cep", log?.Cep?.Codigo ?? (object)DBNull.Value);
            var p13 = _provider.CreateParameter("@foto_conteudo", entity.FotoConteudo ?? (object)DBNull.Value);

            ExecuteNonQuery(sql, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13);
        }

        public IReadOnlyCollection<Aluno> GetAll()
        {
            var sql = "SELECT Id, nome, data_nascimento, cpf, email, telefone, end_numero, end_complemento, logradouro_nome, logradouro_bairro, logradouro_cidade, logradouro_estado, logradouro_cep, foto_conteudo FROM dbo.tb_aluno";
            var list = Query(sql, Map);
            return list.AsReadOnly();
        }

        public Aluno GetById(int id)
        {
            var sql = "SELECT Id, nome, data_nascimento, cpf, email, telefone, end_numero, end_complemento, logradouro_nome, logradouro_bairro, logradouro_cidade, logradouro_estado, logradouro_cep, foto_conteudo FROM dbo.tb_aluno WHERE Id = @id";
            var p = _provider.CreateParameter("@id", id);
            var list = Query(sql, Map, p);
            if (list == null || list.Count == 0) throw new InfrastructureException("Aluno não encontrado");
            return list[0];
        }

        public Aluno GetByCpf(string cpf)
        {
            var sql = "SELECT Id, nome, data_nascimento, cpf, email, telefone, end_numero, end_complemento, logradouro_nome, logradouro_bairro, logradouro_cidade, logradouro_estado, logradouro_cep, foto_conteudo FROM dbo.tb_aluno WHERE cpf = @cpf";
            var p = _provider.CreateParameter("@cpf", cpf ?? (object)DBNull.Value);
            var list = Query(sql, Map, p);
            if (list == null || list.Count == 0) throw new InfrastructureException("Aluno não encontrado");
            return list[0];
        }

        public void Remove(Aluno entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var sql = "DELETE FROM dbo.tb_aluno WHERE nome = @nome";
            var p = _provider.CreateParameter("@nome", entity.Nome);
            ExecuteNonQuery(sql, p);
        }

        public void Update(Aluno entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var sql = "UPDATE dbo.tb_aluno SET data_nascimento = @data_nascimento, cpf = @cpf, email = @email, telefone = @telefone, end_numero = @end_numero, end_complemento = @end_complemento, logradouro_nome = @logradouro_nome, logradouro_bairro = @logradouro_bairro, logradouro_cidade = @logradouro_cidade, logradouro_estado = @logradouro_estado, logradouro_cep = @logradouro_cep, foto_conteudo = @foto_conteudo WHERE Id = @id";

            var p1 = _provider.CreateParameter("@data_nascimento", entity.DataNascimento == default ? (object)DBNull.Value : entity.DataNascimento);
            var p2 = _provider.CreateParameter("@cpf", entity.Cpf?.Valor ?? (object)DBNull.Value);
            var p3 = _provider.CreateParameter("@email", entity.Email?.Endereco ?? (object)DBNull.Value);
            var p4 = _provider.CreateParameter("@telefone", entity.Telefone?.Numero ?? (object)DBNull.Value);

            var numero = entity.Endereco?.Numero ?? string.Empty;
            var complemento = entity.Endereco?.Complemento ?? string.Empty;
            var log = entity.Endereco?.Logradouro;

            var p5 = _provider.CreateParameter("@end_numero", numero == null ? (object)DBNull.Value : numero);
            var p6 = _provider.CreateParameter("@end_complemento", complemento == null ? (object)DBNull.Value : complemento);
            var p7 = _provider.CreateParameter("@logradouro_nome", log?.Nome ?? (object)DBNull.Value);
            var p8 = _provider.CreateParameter("@logradouro_bairro", log?.Bairro ?? (object)DBNull.Value);
            var p9 = _provider.CreateParameter("@logradouro_cidade", log?.Cidade ?? (object)DBNull.Value);
            var p10 = _provider.CreateParameter("@logradouro_estado", log?.Estado ?? (object)DBNull.Value);
            var p11 = _provider.CreateParameter("@logradouro_cep", log?.Cep?.Codigo ?? (object)DBNull.Value);
            var p12 = _provider.CreateParameter("@id", entity.Id);
            var p13 = _provider.CreateParameter("@foto_conteudo", entity.FotoConteudo ?? (object)DBNull.Value);

            ExecuteNonQuery(sql, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13);
        }

        private Aluno Map(DbDataReader r)
        {
            var id = r.GetInt32(0);
            var nome = r.IsDBNull(1) ? string.Empty : r.GetString(1);
            var data_nasc = r.IsDBNull(2) ? default(DateTime) : r.GetDateTime(2);
            var cpf = r.IsDBNull(3) ? null : r.GetString(3);
            var email = r.IsDBNull(4) ? null : r.GetString(4);
            var telefone = r.IsDBNull(5) ? null : r.GetString(5);
            var end_num = r.IsDBNull(6) ? null : r.GetString(6);
            var end_comp = r.IsDBNull(7) ? null : r.GetString(7);
            var log_nome = r.IsDBNull(8) ? null : r.GetString(8);
            var log_bairro = r.IsDBNull(9) ? null : r.GetString(9);
            var log_cidade = r.IsDBNull(10) ? null : r.GetString(10);
            var log_estado = r.IsDBNull(11) ? null : r.GetString(11);
            var log_cep = r.IsDBNull(12) ? null : r.GetString(12);
            var fotoConteudo = r.IsDBNull(13) ? null : (byte[])r.GetValue(13);

            var cpfVo = cpf == null ? null : Domain.ValueObjects.Cpf.Criar(cpf).Value;
            var emailVo = email == null ? null : Domain.ValueObjects.Email.Criar(email).Value;
            var telVo = telefone == null ? null : Domain.ValueObjects.Telefone.Criar(telefone).Value;
            var cepVo = log_cep == null ? null : Domain.ValueObjects.Cep.Criar(log_cep).Value;

            var logradouro = log_nome == null ? null : Logradouro.Criar(log_nome, log_bairro, log_cidade, log_estado, cepVo).Value;
            var endereco = Domain.ValueObjects.Endereco.Criar(logradouro, end_num, end_comp).Value;

            var res = Aluno.Criar(id, nome, cpfVo!, emailVo!, data_nasc, telVo!, endereco, fotoConteudo);
            if (res.IsFailure) throw new InfrastructureException("Falha ao mapear Aluno: " + string.Join(',', res.Notifications));
            return res.Value!;
        }
    }
}
