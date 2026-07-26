// Thiago Augusto Ruskowski Waltrick
using System;
using System.Collections.Generic;
using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Services;

namespace AcademiaDoZe.Domain.ValueObjects
{
    public class Cpf
    {
        public string Valor { get; }

        private Cpf(string valor)
        {
            Valor = valor;
        }

        public static Result<Cpf> Criar(string? valor)
        {
            var notifications = new List<Notification>();
            var limpo = NormalizadoService.ApenasDigitos(valor);
            if (string.IsNullOrWhiteSpace(limpo) || limpo.Length != 11)
                notifications.Add(new Notification(nameof(valor), "CPF inválido. Deve conter 11 dígitos."));

            if (notifications.Count > 0)
                return Result<Cpf>.Failure(notifications);

            return Result<Cpf>.Success(new Cpf(limpo));
        }
    }
}
