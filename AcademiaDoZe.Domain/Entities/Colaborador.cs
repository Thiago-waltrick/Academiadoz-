// Thiago Augusto Ruskowski Waltrick
using System;
using System.Collections.Generic;
using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities
{
    public class Colaborador : Pessoa
    {
        public ColaboradorTipo Tipo { get; private set; }
        public ColaboradorVinculo Vinculo { get; private set; }
        public DateTime DataAdmissao { get; private set; }

        private Colaborador(int id, string nome, Cpf cpf, Email email, DateTime dataNascimento, Telefone telefone, Endereco endereco, ColaboradorTipo tipo, ColaboradorVinculo vinculo, DateTime dataAdmissao)
            : base(id, nome, cpf, email, dataNascimento, telefone, endereco)
        {
            Tipo = tipo;
            Vinculo = vinculo;
            DataAdmissao = dataAdmissao;
        }

        public static Result<Colaborador> Criar(int id, string nome, Cpf cpf, Email email, DateTime dataNascimento, Telefone telefone, Endereco endereco, ColaboradorTipo tipo, ColaboradorVinculo vinculo, DateTime dataAdmissao)
        {
            var notifications = new List<Notification>();
            if (string.IsNullOrWhiteSpace(nome)) notifications.Add(new Notification(nameof(nome), "Nome é obrigatório"));
            if (dataNascimento == default) notifications.Add(new Notification(nameof(dataNascimento), "Data de nascimento inválida"));

            var idade = CalcularIdade(dataNascimento);
            if (idade < 12) notifications.Add(new Notification(nameof(dataNascimento), "Colaborador deve ter ao menos 12 anos"));
            if (dataAdmissao > DateTime.UtcNow) notifications.Add(new Notification(nameof(dataAdmissao), "Data de admissão não pode ser no futuro"));
            if (tipo == ColaboradorTipo.Administrador && vinculo != ColaboradorVinculo.CLT)
                notifications.Add(new Notification(nameof(vinculo), "Administrador deve possuir vínculo CLT"));

            if (notifications.Count > 0) return Result<Colaborador>.Failure(notifications);

            var colaborador = new Colaborador(id, nome.Trim(), cpf, email, dataNascimento, telefone, endereco, tipo, vinculo, dataAdmissao);
            return Result<Colaborador>.Success(colaborador);
        }

        private static int CalcularIdade(DateTime dataNascimento)
        {
            var today = DateTime.UtcNow.Date;
            var age = today.Year - dataNascimento.Year;
            if (dataNascimento.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}
