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
            if (src == null)
                return null!;
            return new LogradouroDto
            {
                Id = src.Id,
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
            var cep = string.IsNullOrWhiteSpace(src.Cep) ? null : Domain.ValueObjects.Cep.Criar(src.Cep);
            if (cep is { IsFailure: true })
                throw new ArgumentException(string.Join("; ", cep.Notifications), nameof(src));

            var result = Logradouro.Criar(src.Nome, src.Bairro, src.Cidade, src.Estado, cep?.Value, src.Id);
            if (result.IsFailure)
                throw new ArgumentException(string.Join("; ", result.Notifications), nameof(src));

            return result.Value!;
        }
    }
}
