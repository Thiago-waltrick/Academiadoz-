// Thiago Augusto Ruskowski Waltrick
using System;
using Xunit;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Domain.Services;

namespace AcademiaDoZe.Domain.Tests.Entities
{
    public class LogradouroTests
    {
        [Theory(DisplayName = "Logradouro: nome vazio -> falha")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Deve_Falhar_Criacao_Quando_NomeVazio(string nome)
        {
            var cidade = "Cidade";
            var estado = "sp";
            var result = Logradouro.Criar(nome, "Bairro", cidade, estado, null);
            Assert.True(result.IsFailure);
            Assert.NotEmpty(result.Notifications);
        }

        [Theory(DisplayName = "Logradouro: dados válidos -> sucesso")]
        [InlineData("Rua A", "Bairro", "Cidade", "sp")]
        public void Deve_Criar_Logradouro_Quando_Valido(string nome, string bairro, string cidade, string estado)
        {
            var result = Logradouro.Criar(nome, bairro, cidade, estado, null);
            Assert.True(result.IsSuccess);
            Assert.Equal(NormalizadoService.ParaMaiusculo(estado), result.Value!.Estado);
        }
    }
}
