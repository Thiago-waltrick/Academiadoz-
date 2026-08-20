// Thiago Augusto Ruskowski Waltrick
using System;
using System.Collections.Generic;
using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Services;

namespace AcademiaDoZe.Domain.ValueObjects
{
    public class Telefone
    {
        public string Numero { get; }

        private Telefone(string numero) => Numero = numero;

        public static Result<Telefone> Criar(string? valor)
        {
            var notifications = new List<Notification>();
            var limpo = NormalizadoService.ApenasDigitos(valor);
            if (string.IsNullOrWhiteSpace(limpo) || (limpo.Length != 10 && limpo.Length != 11))
                notifications.Add(new Notification(nameof(valor), "Telefone inválido. Deve ter 10 ou 11 dígitos."));
            if (notifications.Count > 0) return Result<Telefone>.Failure(notifications);
            return Result<Telefone>.Success(new Telefone(limpo));
        }
    }
}
