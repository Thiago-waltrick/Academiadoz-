using System;
using AcademiaDoZe.Application.Services;
using AcademiaDoZe.Domain.Entities;
using Xunit;

namespace AcademiaDoZe.Infrastructure.Tests
{
    public class AcessoJornadaTests
    {
        [Fact]
        public void SomaSomentePermanenciaNoDiaLocal()
        {
            var inicio = DateTime.Today.ToUniversalTime();
            var primeiro = Criar(inicio.AddHours(-2), inicio.AddHours(1));
            var segundo = Criar(inicio.AddHours(2), inicio.AddHours(4));
            var anterior = Criar(inicio.AddHours(-5), inicio.AddHours(-3));
            var total = AcessoColaboradorService.CalcularTempoHoje(new[] { primeiro, segundo, anterior }, inicio.AddHours(5));
            Assert.Equal(TimeSpan.FromHours(3), total);
        }

        [Fact]
        public void IncluiEntradaAbertaAteAgora()
        {
            var inicio = DateTime.Today.ToUniversalTime();
            var aberto = Criar(inicio.AddHours(1), null);
            var total = AcessoColaboradorService.CalcularTempoHoje(new[] { aberto }, inicio.AddHours(3));
            Assert.Equal(TimeSpan.FromHours(2), total);
        }

        private static AcessoColaborador Criar(DateTime entrada, DateTime? saida)
        {
            var acesso = AcessoColaborador.Criar(1, 1).Value;
            acesso.DataCriacao = entrada;
            typeof(AcessoColaborador).GetProperty(nameof(AcessoColaborador.Saida))!.SetValue(acesso, saida);
            return acesso;
        }
    }
}
