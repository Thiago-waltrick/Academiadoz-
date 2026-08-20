// Thiago Augusto Ruskowski Waltrick
using System;
using Xunit;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Tests.ValueObjects
{
    public class ValueObjectsTests
    {
        [Theory(DisplayName = "Cep: dígitos inválidos -> falha")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("123")]
        public void Deve_Falhar_Criacao_Quando_CepDigitosInvalidos(string input)
        {
            var result = Cep.Criar(input);
            Assert.True(result.IsFailure);
            Assert.NotEmpty(result.Notifications);
        }

        [Theory(DisplayName = "Cep: válido -> sucesso")]
        [InlineData("12345678")]
        [InlineData("12345-678")]
        public void Deve_Criar_Cep_Quando_Valido(string input)
        {
            var result = Cep.Criar(input);
            Assert.True(result.IsSuccess);
            Assert.Equal(8, result.Value!.Codigo.Length);
        }

        [Theory(DisplayName = "Cpf: inválido -> falha")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("123")]
        public void Deve_Falhar_Criacao_Quando_CpfInvalido(string input)
        {
            var result = Cpf.Criar(input);
            Assert.True(result.IsFailure);
            Assert.NotEmpty(result.Notifications);
        }

        [Theory(DisplayName = "Cpf: válido -> sucesso")]
        [InlineData("12345678901")]
        [InlineData("123.456.789-01")]
        public void Deve_Criar_Cpf_Quando_Valido(string input)
        {
            var result = Cpf.Criar(input);
            Assert.True(result.IsSuccess);
            Assert.Equal(11, result.Value!.Valor.Length);
        }

        [Theory(DisplayName = "Telefone: dígitos inválidos -> falha")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("123")]
        public void Deve_Falhar_Criacao_Quando_TelefoneDigitosInvalidos(string input)
        {
            var result = Telefone.Criar(input);
            Assert.True(result.IsFailure);
            Assert.NotEmpty(result.Notifications);
        }

        [Theory(DisplayName = "Telefone: válido -> sucesso")]
        [InlineData("1123456789")]
        [InlineData("11912345678")]
        public void Deve_Criar_Telefone_Quando_Valido(string input)
        {
            var result = Telefone.Criar(input);
            Assert.True(result.IsSuccess);
            Assert.True(result.Value!.Numero.Length == 10 || result.Value.Numero.Length == 11);
        }

        [Theory(DisplayName = "Senha: inválida -> falha")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("123")]
        public void Deve_Falhar_Criacao_Quando_SenhaInvalida(string input)
        {
            var result = Senha.Criar(input);
            Assert.True(result.IsFailure);
            Assert.NotEmpty(result.Notifications);
        }

        [Theory(DisplayName = "Senha: válida -> sucesso")]
        [InlineData("123456")]
        [InlineData("senhaSegura")] 
        public void Deve_Criar_Senha_Quando_Valida(string input)
        {
            var result = Senha.Criar(input);
            Assert.True(result.IsSuccess);
            Assert.Equal(input, result.Value!.Valor);
        }
    }
}
