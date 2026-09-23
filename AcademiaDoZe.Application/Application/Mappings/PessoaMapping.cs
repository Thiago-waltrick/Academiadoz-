// Thiago Augusto Ruskowski Waltrick
using System;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Application.DTOs;

namespace AcademiaDoZe.Application.Mappings
{
    public static class PessoaMapping
    {
        public static PessoaDto ToDto(this Pessoa src)
        {
            if (src == null) return null!;

            // Dispatch to concrete DTOs based on runtime type
            if (src is AcademiaDoZe.Domain.Entities.Aluno a)
                return a.ToDto();
            if (src is AcademiaDoZe.Domain.Entities.Colaborador c)
                return c.ToDto();

            throw new InvalidOperationException("Unsupported Pessoa subtype for mapping to DTO.");
        }

        public static void MapToEntity(this PessoaDto dto, Pessoa entity)
        {
            throw new InvalidOperationException("Mapping DTO to Pessoa entity must use domain factories and validations.");
        }
    }
}
