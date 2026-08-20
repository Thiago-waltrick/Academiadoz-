// Thiago Augusto Ruskowski Waltrick
using System;
using Xunit;
using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Domain.Tests.Entities
{
    public class AcessoAlunoTests
    {
        [Theory(DisplayName = "AcessoAluno: aluno inválido -> falha")]
        [InlineData(0)]
        [InlineData(-5)]
        public void Deve_Falhar_Criacao_Quando_AlunoInvalido(int alunoId)
        {
            var result = AcessoAluno.Criar(1, alunoId);
            Assert.True(result.IsFailure);
            Assert.NotEmpty(result.Notifications);
        }

        [Theory(DisplayName = "AcessoAluno: criação válida -> sucesso")]
        [InlineData(1)]
        public void Deve_Criar_AcessoAluno_Quando_Valido(int alunoId)
        {
            var result = AcessoAluno.Criar(1, alunoId);
            Assert.True(result.IsSuccess);
            Assert.Equal(alunoId, result.Value!.AlunoId);
            Assert.True(result.Value.DataAcesso <= DateTime.UtcNow);
        }
    }
}
