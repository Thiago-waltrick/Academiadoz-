// Thiago Augusto Ruskowski Waltrick
using System;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Extensions;

namespace AcademiaDoZe.Application.Mappings
{
    public static class ColaboradorMapping
    {
        public static ColaboradorDto ToDto(this Colaborador src)
        {
            if (src == null) return null!;
            return new ColaboradorDto
            {
                Id = src.Id,
                Nome = src.Nome,
                Cpf = src.Cpf.ToString(),
                Email = src.Email.ToString(),
                DataNascimento = src.DataNascimento,
                Telefone = src.Telefone?.Numero ?? string.Empty,
                Endereco = src.Endereco?.Logradouro?.ToDto() ?? new Application.DTOs.LogradouroDto(),
                Tipo = src.Tipo.ToApp(),
                Vinculo = src.Vinculo.ToApp(),
                DataAdmissao = src.DataAdmissao
            };
        }

        public static void MapToEntity(this ColaboradorDto dto, Colaborador entity)
        {
            throw new InvalidOperationException("Mapping DTO to Colaborador entity must use domain factories and validations.");
        }
    }
}
