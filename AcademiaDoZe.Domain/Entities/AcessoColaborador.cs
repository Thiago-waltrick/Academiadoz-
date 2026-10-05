// Thiago Augusto Ruskowski Waltrick
using System;
using System.Collections.Generic;
using AcademiaDoZe.Domain.Common;

namespace AcademiaDoZe.Domain.Entities
{
    public class AcessoColaborador
    {
        public int Id { get; private set; }
        public int ColaboradorId { get; private set; }
        public DateTime Entrada { get; private set; }
        // Compatibilidade com testes/versões anteriores: DataCriacao mapeia para Entrada
        public DateTime DataCriacao
        {
            get => Entrada;
            // permite que testes alterem a data via reflexão
            set => Entrada = value;
        }
        public DateTime? Saida { get; private set; }

        // Construtor legado que aceitava Senha - mantido para compatibilidade de testes
        private AcessoColaborador(int id, int colaboradorId, Domain.ValueObjects.Senha senha)
            : this(id, colaboradorId, DateTime.UtcNow, null)
        {
            // senha era usada em versões anteriores para autenticação; atualmente não armazenamos
        }

        private AcessoColaborador(int id, int colaboradorId, DateTime entrada, DateTime? saida)
        {
            Id = id;
            ColaboradorId = colaboradorId;
            Entrada = entrada;
            Saida = saida;
        }

        // Sobrecarga compatível com versões que forneciam Senha
        public static Result<AcessoColaborador> Criar(int id, int colaboradorId, Domain.ValueObjects.Senha senha)
        {
            // simplesmente delega para a validação comum
            return Criar(id, colaboradorId);
        }

        public static Result<AcessoColaborador> Criar(int id, int colaboradorId)
        {
            var notifications = new List<Notification>();
            if (colaboradorId <= 0) notifications.Add(new Notification(nameof(colaboradorId), "Colaborador inválido"));

            if (notifications.Count > 0) return Result<AcessoColaborador>.Failure(notifications);

            var acesso = new AcessoColaborador(id, colaboradorId, DateTime.UtcNow, null);
            return Result<AcessoColaborador>.Success(acesso);
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
    }
}
