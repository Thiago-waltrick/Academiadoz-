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
        public DateTime Entrada { get; private set; }
        public DateTime? Saida { get; private set; }

        private AcessoAluno(int id, int alunoId, DateTime entrada, DateTime? saida)
        {
            Id = id;
            AlunoId = alunoId;
            Entrada = entrada;
            Saida = saida;
        }

        public static Result<AcessoAluno> Criar(int id, int alunoId)
        {
            var notifications = new List<Notification>();
            if (alunoId <= 0) notifications.Add(new Notification(nameof(alunoId), "Aluno inválido"));

            if (notifications.Count > 0) return Result<AcessoAluno>.Failure(notifications);

            var acesso = new AcessoAluno(id, alunoId, DateTime.UtcNow, null);
            return Result<AcessoAluno>.Success(acesso);
        }

        public Result<TimeSpan> RegistrarSaida()
        {
            var notifications = new List<Notification>();
            if (Saida != null) notifications.Add(new Notification(nameof(Saida), "Saída já registrada"));
            if (notifications.Count > 0) return Result<TimeSpan>.Failure(notifications);

            Saida = DateTime.UtcNow;
            var duracao = Saida.Value - Entrada;
            return Result<TimeSpan>.Success(duracao);
        }

        public TimeSpan TempoPermanencia()
        {
            return (Saida ?? DateTime.UtcNow) - Entrada;
        }

        // Compatibilidade: propriedade DataAcesso esperada em testes
        public DateTime DataAcesso
        {
            get => Entrada;
            set => Entrada = value;
        }
    }
}
