// Thiago Augusto Ruskowski Waltrick
using System;
using System.Collections.Generic;
using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Enums;

namespace AcademiaDoZe.Domain.Entities
{
    public class Matricula
    {
        public int Id { get; private set; }
        public int AlunoId { get; private set; }
        public MatriculaPlano Plano { get; private set; }
        public DateTime DataInicio { get; private set; }
        public DateTime? DataFim { get; private set; }

        private Matricula(int id, int alunoId, MatriculaPlano plano, DateTime dataInicio, DateTime? dataFim)
        {
            Id = id;
            AlunoId = alunoId;
            Plano = plano;
            DataInicio = dataInicio;
            DataFim = dataFim;
        }

        public static Result<Matricula> Criar(int id, int alunoId, MatriculaPlano plano, DateTime dataInicio)
        {
            var notifications = new List<Notification>();
            if (alunoId <= 0) notifications.Add(new Notification(nameof(alunoId), "Aluno inválido"));
            if (dataInicio == default) notifications.Add(new Notification(nameof(dataInicio), "Data de início inválida"));

            if (notifications.Count > 0) return Result<Matricula>.Failure(notifications);

            DateTime? dataFim = plano switch
            {
                MatriculaPlano.Mensal => dataInicio.AddMonths(1),
                MatriculaPlano.Trimestral => dataInicio.AddMonths(3),
                MatriculaPlano.Semestral => dataInicio.AddMonths(6),
                MatriculaPlano.Anual => dataInicio.AddYears(1),
                _ => null
            };

            var matricula = new Matricula(id, alunoId, plano, dataInicio, dataFim);
            return Result<Matricula>.Success(matricula);
        }
    }
}
