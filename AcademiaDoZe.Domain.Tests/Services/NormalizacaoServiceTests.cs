// Thiago Augusto Ruskowski Waltrick
using Xunit;
using AcademiaDoZe.Domain.Services;

namespace AcademiaDoZe.Domain.Tests.Services
{
    public class NormalizacaoServiceTests
    {
        [Theory(DisplayName = "LimparEspacos: remove espaços -> sucesso")]
        [InlineData("  abc  ", "abc")]
        [InlineData("", "")]
        [InlineData(null, "")]
        public void Deve_Limpar_Espacos_Quando_RecebeTexto(string input, string expected)
        {
            var result = NormalizadoService.LimparEspacos(input);
            Assert.Equal(expected, result);
        }

        [Theory(DisplayName = "ApenasDigitos: mantém somente dígitos -> sucesso")]
        [InlineData("(11) 91234-5678", "11912345678")]
        [InlineData("abc123", "123")]
        [InlineData(null, "")]
        public void Deve_Remover_NaoDigitos_Quando_RecebeTexto(string input, string expected)
        {
            var result = NormalizadoService.ApenasDigitos(input);
            Assert.Equal(expected, result);
        }

        [Theory(DisplayName = "ParaMaiusculo: converte para maiúsculas -> sucesso")]
        [InlineData("teste", "TESTE")]
        [InlineData(null, "")]
        public void Deve_Converter_ParaMaiusculo(string input, string expected)
        {
            var result = NormalizadoService.ParaMaiusculo(input);
            Assert.Equal(expected, result);
        }

        [Theory(DisplayName = "EhEmailValido: valida formatos de e-mail -> resultado esperado")]
        [InlineData("teste@dominio.com", true)]
        [InlineData("invalido@", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void Deve_Validar_Email_Correto(string? input, bool expected)
        {
            var result = NormalizadoService.EhEmailValido(input ?? string.Empty);
            Assert.Equal(expected, result);
        }
    }
}
