// Thiago Augusto Ruskowski Waltrick
using System;
using System.Collections.Generic;
using AcademiaDoZe.Domain.Common;

namespace AcademiaDoZe.Domain.Entities
{
    public class AcessoAluno
    {
        public int Id { get; private set; }
        public int AlunoId { get; private set; }
        public DateTime DataAcesso { get; private set; }

        private AcessoAluno(int id, int alunoId, DateTime dataAcesso)
        {
            Id = id;
            AlunoId = alunoId;
            DataAcesso = dataAcesso;
        }

        public static Result<AcessoAluno> Criar(int id, int alunoId)
        {
            var notifications = new List<Notification>();
            if (alunoId <= 0) notifications.Add(new Notification(nameof(alunoId), "Aluno inválido"));

            if (notifications.Count > 0) return Result<AcessoAluno>.Failure(notifications);

            var acesso = new AcessoAluno(id, alunoId, DateTime.UtcNow);
            return Result<AcessoAluno>.Success(acesso);
        }
    }
}
