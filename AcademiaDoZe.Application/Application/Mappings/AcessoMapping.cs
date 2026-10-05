// Thiago Augusto Ruskowski Waltrick
using System;
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Application.Mappings
{
    public static class AcessoMapping
    {
        public static AcessoAlunoDto ToDto(this AcessoAluno src)
        {
            if (src == null) return null!;
            return new AcessoAlunoDto
            {
                Id = src.Id,
                AlunoId = src.AlunoId,
                Entrada = src.Entrada,
                Saida = src.Saida,
                MinutosRestantesContrato = null,
                DuracaoMinutos = src.Saida != null ? (src.Saida.Value - src.Entrada).TotalMinutes : (double?)null
            };
        }

        public static AcessoColaboradorDto ToDto(this AcessoColaborador src)
        {
            if (src == null) return null!;
            return new AcessoColaboradorDto
            {
                Id = src.Id,
                ColaboradorId = src.ColaboradorId,
                Entrada = src.Entrada,
                Saida = src.Saida,
                MinutosTrabalhadosDia = src.Saida != null ? (src.Saida.Value - src.Entrada).TotalMinutes : (double?)null,
                ExcedeuJornada = null
            };
        }
    }
}
