using System;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Exceptions;
using Xunit;

namespace AcademiaDoZe.Domain.Tests.Entities
{
    public class TreinoTests
    {
        [Fact]
        public void Sessao_Pode_Ser_Finalizada()
        {
            var iniciadaEm = DateTime.UtcNow;
            var sessao = TreinoSessao.Criar(iniciadaEm);

            sessao.Finalizar(iniciadaEm.AddMinutes(45));

            Assert.True(sessao.Concluida);
            Assert.Equal(iniciadaEm.AddMinutes(45), sessao.ConcluidaEm);
        }

        [Fact]
        public void Sessao_Nao_Pode_Ser_Finalizada_Antes_Do_Inicio()
        {
            var iniciadaEm = DateTime.UtcNow;
            var sessao = TreinoSessao.Criar(iniciadaEm);

            Assert.Throws<DomainException>(() => sessao.Finalizar(iniciadaEm.AddSeconds(-1)));
        }

        [Theory]
        [InlineData(-1, 10)]
        [InlineData(20, 0)]
        public void Serie_Rejeita_Carga_Ou_Repeticoes_Invalidas(decimal carga, int repeticoes)
        {
            Assert.Throws<DomainException>(() => TreinoSerie.Criar(1, 1, carga, repeticoes));
        }

        [Fact]
        public void Serie_Pode_Ser_Marcada_Como_Concluida()
        {
            var serie = TreinoSerie.Criar(1, 1, 40, 8);

            serie.MarcarConcluida();

            Assert.True(serie.Concluida);
        }

        [Fact]
        public void Serie_Rejeita_Carga_Com_Mais_De_Duas_Casas_Decimais()
        {
            Assert.Throws<DomainException>(() => TreinoSerie.Criar(1, 1, 12.345m, 10));
        }
    }
}
