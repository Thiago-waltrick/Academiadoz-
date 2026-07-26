using System;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities
{
    public abstract class Pessoa : Entity
    {
        public string Nome { get; protected set; }
        public Cpf Cpf { get; protected set; }
        public Email Email { get; protected set; }
        public DateTime DataNascimento { get; protected set; }
        public ValueObjects.Telefone Telefone { get; protected set; }
        public ValueObjects.Endereco Endereco { get; protected set; }

        protected Pessoa(int id, string nome, Cpf cpf, Email email, DateTime dataNascimento, ValueObjects.Telefone telefone, ValueObjects.Endereco endereco)
            : base(id)
        {
            Nome = nome;
            Cpf = cpf;
            Email = email;
            DataNascimento = dataNascimento;
            Telefone = telefone;
            Endereco = endereco;
        }
    }
}
