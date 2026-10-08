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
        public string? Objetivo { get; private set; }
        public MatriculaRestricoes Restricoes { get; private set; }
        public string? ObsRestricao { get; private set; }
        public string? LaudoNome { get; private set; }
        public string? LaudoContentType { get; private set; }
        public byte[]? LaudoConteudo { get; private set; }
        public string AlunoNome { get; private set; }
        public string? AlunoCpf { get; private set; }
        public DateTime? AlunoDataNascimento { get; private set; }
        public byte[]? AlunoFotoConteudo { get; private set; }

        private Matricula(
            int id,
            int alunoId,
            MatriculaPlano plano,
            DateTime dataInicio,
            DateTime? dataFim,
            string? objetivo,
            MatriculaRestricoes restricoes,
            string? obsRestricao,
            string? laudoNome,
            string? laudoContentType,
            byte[]? laudoConteudo,
            string alunoNome,
            string? alunoCpf,
            DateTime? alunoDataNascimento,
            byte[]? alunoFotoConteudo)
        {
            Id = id;
            AlunoId = alunoId;
            Plano = plano;
            DataInicio = dataInicio;
            DataFim = dataFim;
            Objetivo = objetivo;
            Restricoes = restricoes;
            ObsRestricao = obsRestricao;
            LaudoNome = laudoNome;
            LaudoContentType = laudoContentType;
            LaudoConteudo = laudoConteudo;
            AlunoNome = alunoNome;
            AlunoCpf = alunoCpf;
            AlunoDataNascimento = alunoDataNascimento;
            AlunoFotoConteudo = alunoFotoConteudo;
        }

        public static Result<Matricula> Criar(
            int id,
            int alunoId,
            MatriculaPlano plano,
            DateTime dataInicio,
            string? objetivo = null,
            MatriculaRestricoes restricoes = MatriculaRestricoes.Nenhuma,
            string? obsRestricao = null,
            string? laudoNome = null,
            string? laudoContentType = null,
            byte[]? laudoConteudo = null,
            string alunoNome = "",
            string? alunoCpf = null,
            DateTime? alunoDataNascimento = null,
            byte[]? alunoFotoConteudo = null)
        {
            var notifications = new List<Notification>();
            if (alunoId <= 0) notifications.Add(new Notification(nameof(alunoId), "Aluno inválido"));
            if (dataInicio == default) notifications.Add(new Notification(nameof(dataInicio), "Data de início inválida"));
            if (!Enum.IsDefined(plano)) notifications.Add(new Notification(nameof(plano), "Plano inválido"));

            if (notifications.Count > 0) return Result<Matricula>.Failure(notifications);

            DateTime? dataFim = plano switch
            {
                MatriculaPlano.Mensal => dataInicio.AddMonths(1),
                MatriculaPlano.Trimestral => dataInicio.AddMonths(3),
                MatriculaPlano.Semestral => dataInicio.AddMonths(6),
                MatriculaPlano.Anual => dataInicio.AddYears(1),
                _ => null
            };

            var matricula = new Matricula(id, alunoId, plano, dataInicio, dataFim, objetivo, restricoes, obsRestricao,
                laudoNome, laudoContentType, laudoConteudo, alunoNome, alunoCpf, alunoDataNascimento, alunoFotoConteudo);
            return Result<Matricula>.Success(matricula);
        }
    }
}
