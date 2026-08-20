// Thiago Augusto Ruskowski Waltrick
using System;
using System.Collections.Generic;
using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Services;

namespace AcademiaDoZe.Domain.ValueObjects
{
    public class Senha
    {
        public string Valor { get; }

        private Senha(string valor) => Valor = valor;

        public static Result<Senha> Criar(string? valor)
        {
            var notifications = new List<Notification>();
            var v = valor ?? string.Empty;
            if (string.IsNullOrWhiteSpace(v) || v.Length < 6)
                notifications.Add(new Notification(nameof(valor), "Senha inválida. Deve ter ao menos 6 caracteres."));
            if (notifications.Count > 0) return Result<Senha>.Failure(notifications);
            return Result<Senha>.Success(new Senha(v));
        }
    }
}
