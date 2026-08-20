// Thiago Augusto Ruskowski Waltrick
using System;
using Xunit;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Tests.Entities
{
    public class AlunoTests
    {
        private static Logradouro GetValidLogradouro() => Logradouro.Criar("Rua Teste", "Bairro", "Cidade", "SP", null).Value!;
        private static Endereco GetValidEndereco() => Endereco.Criar(GetValidLogradouro(), "123", null).Value!;

        [Theory(DisplayName = "Aluno: nome vazio -> falha")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Deve_Falhar_Criacao_Quando_NomeVazio(string nome)
        {
            var cpf = Cpf.Criar("12345678901").Value!;
            var email = Email.Criar("teste@dominio.com").Value!;
            var telefone = Telefone.Criar("11912345678").Value!;
            var endereco = GetValidEndereco();
            var result = Aluno.Criar(0, nome, cpf, email, DateTime.UtcNow.AddYears(-20), telefone, endereco);
            Assert.True(result.IsFailure);
            Assert.NotEmpty(result.Notifications);
        }

        [Theory(DisplayName = "Aluno: criação válida -> sucesso")]
        [InlineData("João")] 
        public void Deve_Criar_Aluno_Quando_DadosValidos(string nome)
        {
            var cpf = Cpf.Criar("12345678901").Value!;
            var email = Email.Criar("teste@dominio.com").Value!;
            var telefone = Telefone.Criar("11912345678").Value!;
            var endereco = GetValidEndereco();
            var result = Aluno.Criar(1, nome, cpf, email, DateTime.UtcNow.AddYears(-25), telefone, endereco);
            Assert.True(result.IsSuccess);
            Assert.Equal(nome.Trim(), result.Value!.Nome);
        }
    }
}
