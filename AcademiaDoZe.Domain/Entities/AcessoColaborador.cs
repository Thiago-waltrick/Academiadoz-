// Thiago Augusto Ruskowski Waltrick
using System;
using System.Collections.Generic;
using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities
{
    public class AcessoColaborador
    {
        public int Id { get; private set; }
        public int ColaboradorId { get; private set; }
        public Senha Senha { get; private set; }
        public DateTime DataCriacao { get; private set; }

        private AcessoColaborador(int id, int colaboradorId, Senha senha)
        {
            Id = id;
            ColaboradorId = colaboradorId;
            Senha = senha;
            DataCriacao = DateTime.UtcNow;
        }

        public static Result<AcessoColaborador> Criar(int id, int colaboradorId, Senha senha)
        {
            var notifications = new List<Notification>();
            if (colaboradorId <= 0) notifications.Add(new Notification(nameof(colaboradorId), "Colaborador inválido"));
            if (senha == null) notifications.Add(new Notification(nameof(senha), "Senha obrigatória"));

            if (notifications.Count > 0) return Result<AcessoColaborador>.Failure(notifications);

            var acesso = new AcessoColaborador(id, colaboradorId, senha);
            return Result<AcessoColaborador>.Success(acesso);
        }
    }
}
