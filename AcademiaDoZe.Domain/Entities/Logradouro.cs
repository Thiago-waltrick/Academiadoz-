// Thiago Augusto Ruskowski Waltrick
using System;
using System.Collections.Generic;
using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Services;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities
{
    public class Logradouro
    {
        public int Id { get; private set; }
        public string Nome { get; }
        public string Bairro { get; }
        public string Cidade { get; }
        public string Estado { get; }
        public Cep? Cep { get; }

        private Logradouro(int id, string nome, string bairro, string cidade, string estado, Cep? cep)
        {
            Id = id;
            Nome = nome;
            Bairro = bairro;
            Cidade = cidade;
            Estado = estado;
            Cep = cep;
        }

        public static Result<Logradouro> Criar(string? nome, string? bairro, string? cidade, string? estado, Cep? cep, int id = 0)
        {
            var notifications = new List<Notification>();
            var nomeN = NormalizadoService.LimparEspacos(nome);
            if (string.IsNullOrWhiteSpace(nomeN)) notifications.Add(new Notification(nameof(nome), "Nome do logradouro obrigatório"));
            var cidadeN = NormalizadoService.LimparEspacos(cidade);
            if (string.IsNullOrWhiteSpace(cidadeN)) notifications.Add(new Notification(nameof(cidade), "Cidade obrigatória"));
            var estadoN = NormalizadoService.ParaMaiusculo(estado);
            if (string.IsNullOrWhiteSpace(estadoN)) notifications.Add(new Notification(nameof(estado), "Estado obrigatório"));

            if (notifications.Count > 0) return Result<Logradouro>.Failure(notifications);

            return Result<Logradouro>.Success(new Logradouro(id, nomeN, NormalizadoService.LimparEspacos(bairro), cidadeN, estadoN, cep));
        }
    }
}
