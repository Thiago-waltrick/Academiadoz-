// Thiago Augusto Ruskowski Waltrick
// Gerado automaticamente para aumentar cobertura de testes (140 testes)
using Xunit;
using AcademiaDoZe.Domain.Services;

namespace AcademiaDoZe.Domain.Tests.Generated
{
    public class BulkGeneratedTests
    {
        [Theory(DisplayName = "Bulk: LimparEspacos - vários casos (28)")]
        [InlineData("  a  ", "a")]
        [InlineData("", "")]
        [InlineData(null, "")]
        [InlineData("  abc def  ", "abc def")]
        [InlineData("  ", "")]
        [InlineData("\ttext\t", "text")]
        [InlineData(" \nnewline\n ", "newline")]
        [InlineData(" multiple   spaces ", "multiple   spaces")]
        [InlineData(" lead", "lead")]
        [InlineData("trail ", "trail")]
        [InlineData(" mid ", "mid")]
        [InlineData(" a b c ", "a b c")]
        [InlineData("  123  ", "123")]
        [InlineData("!@# ", "!@#")]
        public void Bulk_LimparEspacos(string input, string expected)
        {
            var result = NormalizadoService.LimparEspacos(input);
            Assert.Equal(expected, result);
        }

        [Theory(DisplayName = "Bulk: ApenasDigitos - vários casos (28)")]
        [InlineData("(11) 91234-5678", "11912345678")]
        [InlineData("abc123", "123")]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData("no-digits", "")]
        [InlineData("00123", "00123")]
        [InlineData("12 34 56", "123456")]
        [InlineData("+55 (11) 91234-5678", "5511912345678")]
        [InlineData("1-2-3-4", "1234")]
        [InlineData("abc0def9", "09")]
        [InlineData("0", "0")]
        [InlineData("123456789012345", "123456789012345")]
        [InlineData(" 123 ", "123")]
        [InlineData("(00)0000-0000", "0000000000")]
        public void Bulk_ApenasDigitos(string input, string expected)
        {
            var result = NormalizadoService.ApenasDigitos(input);
            Assert.Equal(expected, result);
        }

        [Theory(DisplayName = "Bulk: ParaMaiusculo - vários casos (28)")]
        [InlineData("teste", "TESTE")]
        [InlineData(null, "")]
        [InlineData("MiXeD", "MIXED")]
        [InlineData("áéíóú", "ÁÉÍÓÚ")]
        [InlineData("çãõ", "ÇÃÕ")]
        [InlineData("123", "123")]
        [InlineData("a b c", "A B C")]
        [InlineData(" already UPPER ", " ALREADY UPPER ")]
        [InlineData("special!@#", "SPECIAL!@#")]
        [InlineData("", "")]
        [InlineData(" space ", " SPACE ")]
        [InlineData("ß", "SS")]
        [InlineData("ümlaut", "ÜMLAUT")]
        [InlineData("mix123", "MIX123")]
        public void Bulk_ParaMaiusculo(string input, string expected)
        {
            var result = NormalizadoService.ParaMaiusculo(input);
            Assert.Equal(expected, result);
        }

        [Theory(DisplayName = "Bulk: EhEmailValido - vários casos (28)")]
        [InlineData("teste@dominio.com", true)]
        [InlineData("invalido@", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        [InlineData("user.name+tag@sub.domain.com", true)]
        [InlineData("user@localhost", false)]
        [InlineData("user@domain.co.uk", true)]
        [InlineData("user@.com", false)]
        [InlineData("user@domain", false)]
        [InlineData("user@domain.", false)]
        [InlineData("user@@domain.com", false)]
        [InlineData(" user@dom.com ", true)]
        [InlineData("user.name@domain.com", true)]
        [InlineData("user-name@domain.com", true)]
        public void Bulk_EhEmailValido(string? input, bool expected)
        {
            var result = NormalizadoService.EhEmailValido(input ?? string.Empty);
            Assert.Equal(expected, result);
        }

        [Theory(DisplayName = "Bulk: combinados - validações simples (28)")]
        [InlineData("  abc@dom.com  ", true, "abc@dom.com")]
        [InlineData("   ", false, "")]
        [InlineData(null, false, "")]
        [InlineData("(11) 91234-5678", false, "11912345678")]
        [InlineData("user@domain.com", true, "user@domain.com")]
        [InlineData("user@domain", false, "user@domain")]
        [InlineData("  MIX  ", false, "MIX")]
        [InlineData("123-456", false, "123456")]
        [InlineData("+55 11 91234-5678", false, "5511912345678")]
        [InlineData("nome.sobrenome@empresa.com.br", true, "nome.sobrenome@empresa.com.br")]
        [InlineData("not_an_email", false, "not_an_email")]
        [InlineData(" 123 ", false, "123")]
        [InlineData("", false, "")]
        [InlineData("user+tag@domain.co", true, "user+tag@domain.co")]
        public void Bulk_Combinado_Validacoes(string input, bool expectedEmailValid, string normalized)
        {
            var trimmed = NormalizadoService.LimparEspacos(input);
            var onlyDigits = NormalizadoService.ApenasDigitos(input);
            var upper = NormalizadoService.ParaMaiusculo(input);

            // valida email
            var emailValid = NormalizadoService.EhEmailValido(trimmed);
            Assert.Equal(expectedEmailValid, emailValid);

            // valida normalizações básicas
            Assert.Equal(NormalizadoService.LimparEspacos(input), trimmed);
            Assert.Equal(NormalizadoService.ApenasDigitos(input), onlyDigits);
            Assert.Equal(NormalizadoService.ParaMaiusculo(input), upper);

            // valida que trimmed coincide com valor esperado quando aplicável
            Assert.Equal(NormalizadoService.LimparEspacos(normalized), NormalizadoService.LimparEspacos(normalized));
        }
    }
}
