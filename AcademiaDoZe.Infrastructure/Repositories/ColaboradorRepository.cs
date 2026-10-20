// Thiago Augusto Ruskowski Waltrick
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Infrastructure.Data;
using AcademiaDoZe.Infrastructure.Exceptions;

namespace AcademiaDoZe.Infrastructure.Repositories
{
    public class ColaboradorRepository : BaseRepository, IColaboradorRepository
    {
        public ColaboradorRepository(DbProvider provider) : base(provider)
        {
        }

        public void Add(Colaborador entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var sql = "INSERT INTO dbo.tb_colaborador (nome, data_nascimento, data_admissao, tipo, vinculo, cpf, email, telefone, end_numero, end_complemento, logradouro_nome, logradouro_bairro, logradouro_cidade, logradouro_estado, logradouro_cep, foto_conteudo) VALUES (@nome, @data_nascimento, @data_admissao, @tipo, @vinculo, @cpf, @email, @telefone, @end_numero, @end_complemento, @logradouro_nome, @logradouro_bairro, @logradouro_cidade, @logradouro_estado, @logradouro_cep, @foto_conteudo);";

            var p1 = _provider.CreateParameter("@nome", entity.Nome);
            var p2 = _provider.CreateParameter("@data_nascimento", entity.DataNascimento == default ? (object)DBNull.Value : entity.DataNascimento);
            var p3 = _provider.CreateParameter("@data_admissao", entity.DataAdmissao == default ? (object)DBNull.Value : entity.DataAdmissao);
            var p4 = _provider.CreateParameter("@tipo", (int)entity.Tipo);
            var p5 = _provider.CreateParameter("@vinculo", (int)entity.Vinculo);
            var p6 = _provider.CreateParameter("@cpf", entity.Cpf?.Valor ?? (object)DBNull.Value);
            var p7 = _provider.CreateParameter("@email", entity.Email?.Endereco ?? (object)DBNull.Value);
            var p8 = _provider.CreateParameter("@telefone", entity.Telefone?.Numero ?? (object)DBNull.Value);
            var address = entity.Endereco;
            var logradouro = address?.Logradouro;
            var p9 = _provider.CreateParameter("@end_numero", address?.Numero ?? (object)DBNull.Value);
            var p10 = _provider.CreateParameter("@end_complemento", address?.Complemento ?? (object)DBNull.Value);
            var p11 = _provider.CreateParameter("@logradouro_nome", logradouro?.Nome ?? (object)DBNull.Value);
            var p12 = _provider.CreateParameter("@logradouro_bairro", logradouro?.Bairro ?? (object)DBNull.Value);
            var p13 = _provider.CreateParameter("@logradouro_cidade", logradouro?.Cidade ?? (object)DBNull.Value);
            var p14 = _provider.CreateParameter("@logradouro_estado", logradouro?.Estado ?? (object)DBNull.Value);
            var p15 = _provider.CreateParameter("@logradouro_cep", logradouro?.Cep?.Codigo ?? (object)DBNull.Value);
            var p16 = _provider.CreateParameter("@foto_conteudo", entity.FotoConteudo ?? (object)DBNull.Value);

            ExecuteNonQuery(sql, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16);
        }

        public IReadOnlyCollection<Colaborador> GetAll()
        {
            var sql = "SELECT Id, nome, data_nascimento, data_admissao, tipo, vinculo, cpf, email, telefone, end_numero, end_complemento, logradouro_nome, logradouro_bairro, logradouro_cidade, logradouro_estado, logradouro_cep, foto_conteudo FROM dbo.tb_colaborador";
            var list = Query(sql, Map);
            return list.AsReadOnly();
        }

        public Colaborador GetById(int id)
        {
            var sql = "SELECT Id, nome, data_nascimento, data_admissao, tipo, vinculo, cpf, email, telefone, end_numero, end_complemento, logradouro_nome, logradouro_bairro, logradouro_cidade, logradouro_estado, logradouro_cep, foto_conteudo FROM dbo.tb_colaborador WHERE Id = @id";
            var p = _provider.CreateParameter("@id", id);
            var list = Query(sql, Map, p);
            if (list == null || list.Count == 0) throw new InfrastructureException("Colaborador não encontrado");
            return list[0];
        }

        public Colaborador GetByCpf(string cpf)
        {
            var sql = "SELECT Id, nome, data_nascimento, data_admissao, tipo, vinculo, cpf, email, telefone, end_numero, end_complemento, logradouro_nome, logradouro_bairro, logradouro_cidade, logradouro_estado, logradouro_cep, foto_conteudo FROM dbo.tb_colaborador WHERE cpf = @cpf";
            var p = _provider.CreateParameter("@cpf", cpf ?? (object)DBNull.Value);
            var list = Query(sql, Map, p);
            if (list == null || list.Count == 0) throw new InfrastructureException("Colaborador não encontrado");
            return list[0];
        }

        public void Remove(Colaborador entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var sql = "DELETE FROM dbo.tb_colaborador WHERE Id = @id";
            var p = _provider.CreateParameter("@id", entity.Id);
            ExecuteNonQuery(sql, p);
        }

        public void Update(Colaborador entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var sql = "UPDATE dbo.tb_colaborador SET nome = @nome, data_nascimento = @data_nascimento, data_admissao = @data_admissao, tipo = @tipo, vinculo = @vinculo, cpf = @cpf, email = @email, telefone = @telefone, end_numero = @end_numero, end_complemento = @end_complemento, logradouro_nome = @logradouro_nome, logradouro_bairro = @logradouro_bairro, logradouro_cidade = @logradouro_cidade, logradouro_estado = @logradouro_estado, logradouro_cep = @logradouro_cep, foto_conteudo = @foto_conteudo WHERE Id = @id";
            var address = entity.Endereco;
            var logradouro = address?.Logradouro;
            var p1 = _provider.CreateParameter("@nome", entity.Nome);
            var p2 = _provider.CreateParameter("@data_nascimento", entity.DataNascimento == default ? (object)DBNull.Value : entity.DataNascimento);
            var p3 = _provider.CreateParameter("@data_admissao", entity.DataAdmissao == default ? (object)DBNull.Value : entity.DataAdmissao);
            var p4 = _provider.CreateParameter("@tipo", (int)entity.Tipo);
            var p5 = _provider.CreateParameter("@vinculo", (int)entity.Vinculo);
            var p6 = _provider.CreateParameter("@cpf", entity.Cpf?.Valor ?? (object)DBNull.Value);
            var p7 = _provider.CreateParameter("@email", entity.Email?.Endereco ?? (object)DBNull.Value);
            var p8 = _provider.CreateParameter("@telefone", entity.Telefone?.Numero ?? (object)DBNull.Value);
            var p9 = _provider.CreateParameter("@end_numero", address?.Numero ?? (object)DBNull.Value);
            var p10 = _provider.CreateParameter("@end_complemento", address?.Complemento ?? (object)DBNull.Value);
            var p11 = _provider.CreateParameter("@logradouro_nome", logradouro?.Nome ?? (object)DBNull.Value);
            var p12 = _provider.CreateParameter("@logradouro_bairro", logradouro?.Bairro ?? (object)DBNull.Value);
            var p13 = _provider.CreateParameter("@logradouro_cidade", logradouro?.Cidade ?? (object)DBNull.Value);
            var p14 = _provider.CreateParameter("@logradouro_estado", logradouro?.Estado ?? (object)DBNull.Value);
            var p15 = _provider.CreateParameter("@logradouro_cep", logradouro?.Cep?.Codigo ?? (object)DBNull.Value);
            var p16 = _provider.CreateParameter("@foto_conteudo", entity.FotoConteudo ?? (object)DBNull.Value);
            var p17 = _provider.CreateParameter("@id", entity.Id);
            ExecuteNonQuery(sql, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17);
        }

        private Colaborador Map(DbDataReader r)
        {
            var id = r.GetInt32(0);
            var nome = r.IsDBNull(1) ? string.Empty : r.GetString(1);
            var data_nasc = r.IsDBNull(2) ? default(DateTime) : r.GetDateTime(2);
            var data_adm = r.IsDBNull(3) ? default(DateTime) : r.GetDateTime(3);
            var tipo = r.IsDBNull(4) ? 0 : r.GetInt32(4);
            var vinculo = r.IsDBNull(5) ? 0 : r.GetInt32(5);
            var cpf = r.IsDBNull(6) ? null : r.GetString(6);
            var email = r.IsDBNull(7) ? null : r.GetString(7);
            var telefone = r.IsDBNull(8) ? null : r.GetString(8);
            var numero = r.IsDBNull(9) ? null : r.GetString(9);
            var complemento = r.IsDBNull(10) ? null : r.GetString(10);
            var logradouroNome = r.IsDBNull(11) ? null : r.GetString(11);
            var bairro = r.IsDBNull(12) ? null : r.GetString(12);
            var cidade = r.IsDBNull(13) ? null : r.GetString(13);
            var estado = r.IsDBNull(14) ? null : r.GetString(14);
            var cep = r.IsDBNull(15) ? null : r.GetString(15);
            var foto = r.IsDBNull(16) ? null : (byte[])r.GetValue(16);

            var cpfVo = cpf == null ? null : Domain.ValueObjects.Cpf.Criar(cpf).Value;
            var emailVo = email == null ? null : Domain.ValueObjects.Email.Criar(email).Value;
            var telVo = telefone == null ? null : Domain.ValueObjects.Telefone.Criar(telefone).Value;

            var logradouro = string.IsNullOrWhiteSpace(logradouroNome) || string.IsNullOrWhiteSpace(cidade) || string.IsNullOrWhiteSpace(estado)
                ? null
                : Logradouro.Criar(logradouroNome, bairro, cidade, estado, string.IsNullOrWhiteSpace(cep) ? null : Domain.ValueObjects.Cep.Criar(cep).Value).Value;
            var endereco = logradouro is null || string.IsNullOrWhiteSpace(numero)
                ? null
                : Domain.ValueObjects.Endereco.Criar(logradouro, numero, complemento).Value;

            var res = Colaborador.Criar(id, nome, cpfVo!, emailVo!, data_nasc, telVo!, endereco!, (Domain.Enums.ColaboradorTipo)tipo, (Domain.Enums.ColaboradorVinculo)vinculo, data_adm, foto);
            if (res.IsFailure) throw new InfrastructureException("Falha ao mapear Colaborador: " + string.Join(',', res.Notifications));
            return res.Value!;
        }
    }
}
