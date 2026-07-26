// Thiago Augusto Ruskowski Waltrick
using System;
using System.Collections.Generic;
using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Services;
using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Domain.ValueObjects
{
    public class Endereco
    {
        public Logradouro Logradouro { get; }
        public string Numero { get; }
        public string Complemento { get; }

        private Endereco(Logradouro logradouro, string numero, string complemento)
        {
            Logradouro = logradouro;
            Numero = numero;
            Complemento = complemento;
        }

        public static Result<Endereco> Criar(Logradouro logradouro, string? numero, string? complemento)
        {
            var notifications = new List<Notification>();
            if (logradouro == null)
                notifications.Add(new Notification(nameof(logradouro), "Logradouro é obrigatório."));
            var numeroNorm = NormalizadoService.LimparEspacos(numero);
            if (string.IsNullOrWhiteSpace(numeroNorm))
                notifications.Add(new Notification(nameof(numero), "Número é obrigatório."));

            if (notifications.Count > 0)
                return Result<Endereco>.Failure(notifications);

            return Result<Endereco>.Success(new Endereco(logradouro, numeroNorm, NormalizadoService.LimparEspacos(complemento)));
        }
    }
}
