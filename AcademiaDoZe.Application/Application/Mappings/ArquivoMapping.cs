// Thiago Augusto Ruskowski Waltrick
using System;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Application.DTOs;

namespace AcademiaDoZe.Application.Mappings
{
    public static class ArquivoMapping
    {
        public static ArquivoDto ToDto(this Arquivo src)
        {
            if (src == null) return null!;
            return new ArquivoDto
            {
                Nome = src.Nome,
                ContentType = src.ContentType,
                Tamanho = src.Tamanho
            };
        }

        public static Arquivo ToEntity(this ArquivoDto src)
        {
            if (src == null) return null!;
            throw new InvalidOperationException("Converting ArquivoDto to Arquivo should use domain factories and validations.");
        }
    }
}
