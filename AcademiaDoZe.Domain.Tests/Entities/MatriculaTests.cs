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

        [Theory(DisplayName = "Matricula: calcula data fim para cada plano")]
        [InlineData(MatriculaPlano.Mensal, 1)]
        [InlineData(MatriculaPlano.Trimestral, 3)]
        [InlineData(MatriculaPlano.Semestral, 6)]
        [InlineData(MatriculaPlano.Anual, 12)]
        public void Deve_Calcular_DataFim_Quando_Plano(MatriculaPlano plano, int meses)
        {
            var inicio = new DateTime(2026, 1, 15);
            var result = Matricula.Criar(1, 1, plano, inicio);
            Assert.True(result.IsSuccess);
            Assert.Equal(inicio.AddMonths(meses), result.Value!.DataFim);
            Assert.Equal((int)plano, (int)result.Value.Plano);
        }

        [Fact]
        public void Deve_Preservar_Multiplas_Restricoes()
        {
            var restricoes = MatriculaRestricoes.Diabetes | MatriculaRestricoes.Alergias;
            var result = Matricula.Criar(1, 1, MatriculaPlano.Mensal, new DateTime(2026, 1, 15), restricoes: restricoes);

            Assert.True(result.IsSuccess);
            Assert.Equal(restricoes, result.Value!.Restricoes);
        }

        [Fact]
        public void Deve_Rejeitar_Codigo_De_Plano_Fora_De_1_A_4()
        {
            var result = Matricula.Criar(1, 1, (MatriculaPlano)0, new DateTime(2026, 1, 15));

            Assert.True(result.IsFailure);
        }
    }
}
