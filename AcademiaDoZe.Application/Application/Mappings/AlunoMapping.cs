// Thiago Augusto Ruskowski Waltrick
using System;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Application.DTOs;

namespace AcademiaDoZe.Application.Mappings
{
    public static class AlunoMapping
    {
        public static AlunoDto ToDto(this Aluno src)
        {
            if (src == null) return null!;
            var dto = new AlunoDto
            {
                Id = src.Id,
                Nome = src.Nome,
                Cpf = src.Cpf.ToString(),
                Email = src.Email.ToString(),
                DataNascimento = src.DataNascimento,
                Telefone = src.Telefone?.Numero ?? string.Empty,
                Endereco = src.Endereco?.Logradouro?.ToDto() ?? new Application.DTOs.LogradouroDto(),
                FotoConteudo = src.FotoConteudo
            };
            return dto;
        }

        public static void MapToEntity(this AlunoDto dto, Aluno entity)
        {
            throw new InvalidOperationException("Mapping DTO to Aluno entity must use domain factories and validations.");
        }
    }
}
