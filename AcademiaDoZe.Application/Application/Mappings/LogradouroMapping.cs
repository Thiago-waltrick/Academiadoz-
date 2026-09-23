// Thiago Augusto Ruskowski Waltrick
using System;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Application.DTOs;

namespace AcademiaDoZe.Application.Mappings
{
    public static class LogradouroMapping
    {
        public static LogradouroDto ToDto(this Logradouro src)
        {
            if (src == null) return null!;
            return new LogradouroDto
            {
                Nome = src.Nome,
                Bairro = src.Bairro,
                Cidade = src.Cidade,
                Estado = src.Estado,
                Cep = src.Cep?.Codigo
            };
        }

        public static Logradouro ToEntity(this LogradouroDto src)
        {
            if (src == null) return null!;
            // Domain Logradouro uses factory; cannot set private ctor — return null or throw
            throw new InvalidOperationException("Converting LogradouroDto to Logradouro entity should use domain factories and validations.");
        }
    }
}
