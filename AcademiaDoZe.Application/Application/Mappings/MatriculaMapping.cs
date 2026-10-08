// Thiago Augusto Ruskowski Waltrick
using System;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Extensions;

namespace AcademiaDoZe.Application.Mappings
{
    public static class MatriculaMapping
    {
        public static MatriculaDto ToDto(this Matricula src)
        {
            if (src == null) return null!;
            return new MatriculaDto
            {
                Id = src.Id,
                AlunoId = src.AlunoId,
                Aluno = new AlunoDto
                {
                    Id = src.AlunoId,
                    Nome = src.AlunoNome,
                    Cpf = src.AlunoCpf ?? string.Empty,
                    DataNascimento = src.AlunoDataNascimento ?? default,
                    FotoConteudo = src.AlunoFotoConteudo
                },
                Plano = src.Plano.ToApp(),
                DataInicio = src.DataInicio,
                DataFim = src.DataFim,
                Objetivo = src.Objetivo ?? string.Empty,
                Restricoes = src.Restricoes.ToApp(),
                ObsRestricao = src.ObsRestricao,
                LaudoMedico = src.LaudoNome is null ? null : new ArquivoDto
                {
                    Nome = src.LaudoNome,
                    ContentType = src.LaudoContentType ?? string.Empty,
                    Conteudo = src.LaudoConteudo,
                    Tamanho = src.LaudoConteudo?.LongLength ?? 0
                }
            };
        }

        public static Matricula ToEntity(this MatriculaDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            // Use domain factory Criar
            var result = Matricula.Criar(dto.Id, dto.AlunoId, dto.Plano.ToDomain(), dto.DataInicio,
                dto.Objetivo, dto.Restricoes.ToDomain(), dto.ObsRestricao,
                dto.LaudoMedico?.Nome, dto.LaudoMedico?.ContentType, dto.LaudoMedico?.Conteudo);
            if (!result.IsSuccess) throw new InvalidOperationException("Falha ao criar Matricula a partir do DTO: " + string.Join(", ", result.Notifications));
            return result.Value;
        }
    }
}
