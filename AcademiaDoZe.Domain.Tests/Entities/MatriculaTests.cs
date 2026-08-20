// Thiago Augusto Ruskowski Waltrick
using System;
using Xunit;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;

namespace AcademiaDoZe.Domain.Tests.Entities
{
    public class MatriculaTests
    {
        [Theory(DisplayName = "Matricula: aluno inválido -> falha")]
        [InlineData(0)]
        [InlineData(-1)]
        public void Deve_Falhar_Criacao_Quando_AlunoInvalido(int alunoId)
        {
            var result = Matricula.Criar(1, alunoId, MatriculaPlano.Mensal, DateTime.UtcNow);
            Assert.True(result.IsFailure);
            Assert.NotEmpty(result.Notifications);
        }

        [Theory(DisplayName = "Matricula: data inicio inválida -> falha")]
        [InlineData("0001-01-01")]
        public void Deve_Falhar_Criacao_Quando_DataInicioInvalida(string data)
        {
            var dt = DateTime.Parse(data);
            var result = Matricula.Criar(1, 1, MatriculaPlano.Mensal, dt);
            Assert.True(result.IsFailure);
            Assert.NotEmpty(result.Notifications);
        }

        [Theory(DisplayName = "Matricula: calcula data fim por plano -> sucesso")]
        [InlineData(0)]
        public void Deve_Calcular_DataFim_Quando_Plano(int plano)
        {
            var inicio = DateTime.UtcNow.Date;
            var result = Matricula.Criar(1, 1, MatriculaPlano.Mensal, inicio);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value!.DataFim);
        }
    }
}
