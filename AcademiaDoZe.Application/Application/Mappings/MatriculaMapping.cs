// Thiago Augusto Ruskowski Waltrick
using System;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Extensions;
using AcademiaDoZe.Domain.Enums;

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
                Aluno = src.AlunoId > 0 ? new AlunoDto { Id = src.AlunoId } : new AlunoDto(),
                Plano = src.Plano.ToApp(),
                DataInicio = src.DataInicio,
                DataFim = src.DataFim,
                // objetivo/obs/laudo não existem no domínio Matricula; left as defaults
            };
        }

        public static Matricula ToEntity(this MatriculaDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            // Use domain factory Criar
            var result = Matricula.Criar(dto.Id, dto.AlunoId, dto.Plano.ToDomain(), dto.DataInicio);
            if (!result.IsSuccess) throw new InvalidOperationException("Falha ao criar Matricula a partir do DTO: " + string.Join(", ", result.Notifications));
            return result.Value;
        }
    }
}
