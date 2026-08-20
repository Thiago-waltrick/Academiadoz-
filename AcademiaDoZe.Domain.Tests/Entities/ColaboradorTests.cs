// Thiago Augusto Ruskowski Waltrick
using System;
using Xunit;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Domain.Enums;

namespace AcademiaDoZe.Domain.Tests.Entities
{
    public class ColaboradorTests
    {
        private static Logradouro GetValidLogradouro() => Logradouro.Criar("Rua Teste", "Bairro", "Cidade", "SP", null).Value!;
        private static Endereco GetValidEndereco() => Endereco.Criar(GetValidLogradouro(), "123", null).Value!;

        [Theory(DisplayName = "Colaborador: data admissão futura -> falha")]
        [InlineData(1)]
        public void Deve_Falhar_Criacao_Quando_DataAdmissaoFutura(int _)
        {
            var cpf = Cpf.Criar("12345678901").Value!;
            var email = Email.Criar("colab@dominio.com").Value!;
            var telefone = Telefone.Criar("11912345678").Value!;
            var endereco = GetValidEndereco();
            var dataAdmissao = DateTime.UtcNow.AddDays(1);
            var result = Colaborador.Criar(1, "Carlos", cpf, email, DateTime.UtcNow.AddYears(-30), telefone, endereco, ColaboradorTipo.Instrutor, ColaboradorVinculo.CLT, dataAdmissao);
            Assert.True(result.IsFailure);
            Assert.NotEmpty(result.Notifications);
        }

        [Theory(DisplayName = "Colaborador: administrador com vínculo não CLT -> falha")]
        [InlineData(1)]
        public void Deve_Falhar_Criacao_Quando_AdministradorSemCLT(int _)
        {
            var cpf = Cpf.Criar("12345678901").Value!;
            var email = Email.Criar("adm@dominio.com").Value!;
            var telefone = Telefone.Criar("11912345678").Value!;
            var endereco = GetValidEndereco();
            var dataAdmissao = DateTime.UtcNow.AddDays(-1);
            var result = Colaborador.Criar(1, "Admin", cpf, email, DateTime.UtcNow.AddYears(-30), telefone, endereco, ColaboradorTipo.Administrador, ColaboradorVinculo.Estagio, dataAdmissao);
            Assert.True(result.IsFailure);
            Assert.NotEmpty(result.Notifications);
        }

        [Theory(DisplayName = "Colaborador: dados válidos -> sucesso")]
        [InlineData("Mariana")]
        public void Deve_Criar_Colaborador_Quando_DadosValidos(string nome)
        {
            var cpf = Cpf.Criar("12345678901").Value!;
            var email = Email.Criar("colab@dominio.com").Value!;
            var telefone = Telefone.Criar("11912345678").Value!;
            var endereco = GetValidEndereco();
            var dataAdmissao = DateTime.UtcNow.AddDays(-10);
            var result = Colaborador.Criar(1, nome, cpf, email, DateTime.UtcNow.AddYears(-30), telefone, endereco, ColaboradorTipo.Instrutor, ColaboradorVinculo.CLT, dataAdmissao);
            Assert.True(result.IsSuccess);
            Assert.Equal(nome.Trim(), result.Value!.Nome);
        }
    }
}
