// Thiago Augusto Ruskowski Waltrick
using System;
using System.Collections.Generic;
using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Services;

namespace AcademiaDoZe.Domain.ValueObjects
{
    public class Cep
    {
        public string Codigo { get; }

        private Cep(string codigo)
        {
            Codigo = codigo;
        }

        public static Result<Cep> Criar(string? valor)
        {
            var notifications = new List<Notification>();
            var limpo = NormalizadoService.ApenasDigitos(valor);
            if (string.IsNullOrWhiteSpace(limpo) || limpo.Length != 8)
                notifications.Add(new Notification(nameof(valor), "CEP inválido. Deve conter 8 dígitos."));

            if (notifications.Count > 0)
                return Result<Cep>.Failure(notifications);

            return Result<Cep>.Success(new Cep(limpo));
        }
    }
}
