// Thiago Augusto Ruskowski Waltrick
using System;
using System.Collections.Generic;
using AcademiaDoZe.Domain.Common;

namespace AcademiaDoZe.Domain.ValueObjects
{
    public class Arquivo
    {
        private const long LIMITE_BYTES = 15L * 1024 * 1024; // 15MB

        public string Nome { get; }
        public string ContentType { get; }
        public long Tamanho { get; }

        private Arquivo(string nome, string contentType, long tamanho)
        {
            Nome = nome;
            ContentType = contentType;
            Tamanho = tamanho;
        }

        public static Result<Arquivo> Criar(string nome, string contentType, long tamanho)
        {
            var notifications = new List<Notification>();
            if (string.IsNullOrWhiteSpace(nome))
                notifications.Add(new Notification(nameof(nome), "Nome do arquivo é obrigatório"));
            if (tamanho <= 0)
                notifications.Add(new Notification(nameof(tamanho), "Tamanho inválido"));
            if (tamanho > LIMITE_BYTES)
                notifications.Add(new Notification(nameof(tamanho), "Arquivo excede o limite de 15MB"));
            if (notifications.Count > 0)
                return Result<Arquivo>.Failure(notifications);

            var arquivo = new Arquivo(nome.Trim(), contentType ?? string.Empty, tamanho);
            return Result<Arquivo>.Success(arquivo);
        }
    }
}
