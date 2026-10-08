
// Thiago Augusto Ruskowski Waltrick
using System;
using System.Collections.Generic;
using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities
{
    public class Aluno : Pessoa
    {
        public byte[]? FotoConteudo { get; private set; }

        private Aluno(int id, string nome, Cpf cpf, Email email, DateTime dataNascimento, Telefone telefone, Endereco endereco, byte[]? fotoConteudo)
            : base(id, nome, cpf, email, dataNascimento, telefone, endereco)
        {
            FotoConteudo = fotoConteudo;
        }

        public static Result<Aluno> Criar(int id, string nome, Cpf cpf, Email email, DateTime dataNascimento, Telefone telefone, Endereco endereco, byte[]? fotoConteudo = null)
        {
            var notifications = new List<Notification>();
            if (string.IsNullOrWhiteSpace(nome)) notifications.Add(new Notification(nameof(nome), "Nome é obrigatório"));
            if (dataNascimento == default) notifications.Add(new Notification(nameof(dataNascimento), "Data de nascimento inválida"));

            if (notifications.Count > 0) return Result<Aluno>.Failure(notifications);

            var aluno = new Aluno(id, nome.Trim(), cpf, email, dataNascimento, telefone, endereco, fotoConteudo);
            return Result<Aluno>.Success(aluno);
        }
    }
}
