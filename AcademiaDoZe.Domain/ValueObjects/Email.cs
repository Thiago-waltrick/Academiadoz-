// Thiago Augusto Ruskowski Waltrick
using System;
using System.Collections.Generic;
using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Services;

namespace AcademiaDoZe.Domain.ValueObjects
{
    public class Email
    {
        public string Endereco { get; }

        private Email(string endereco)
        {
            Endereco = endereco;
        }

        public static Result<Email> Criar(string? endereco)
        {
            var notifications = new List<Notification>();
            if (string.IsNullOrWhiteSpace(endereco))
                notifications.Add(new Notification(nameof(endereco), "E-mail é obrigatório."));
            else if (!NormalizadoService.EhEmailValido(endereco))
                notifications.Add(new Notification(nameof(endereco), "E-mail em formato inválido."));

            if (notifications.Count > 0)
                return Result<Email>.Failure(notifications);

            var normalizado = (endereco ?? string.Empty).Trim().ToLowerInvariant();
            return Result<Email>.Success(new Email(normalizado));
        }
    }
}
