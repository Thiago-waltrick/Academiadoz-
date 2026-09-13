// Thiago Augusto Ruskowski Waltrick
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace AcademiaDoZe.Domain.Services
{
    public static class NormalizadoService
    {
        public static string LimparEspacos(string? input) => (input ?? string.Empty).Trim();

        public static string ApenasDigitos(string? input) => input is null ? string.Empty : new string((input).Where(char.IsDigit).ToArray());

        public static string ParaMaiusculo(string? input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            // Tratamento explícito para o caractere alemão 'ß' (eszett) que deve mapear para "SS"
            // Substituímos por "ss" antes de aplicar ToUpperInvariant para garantir comportamento consistente
            var temp = (input ?? string.Empty).Replace("ß", "ss");
            return temp.ToUpperInvariant();
        }

        public static bool EhEmailValido(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            try
            {
                var trimmed = email.Trim();
                var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return regex.IsMatch(trimmed);
            }
            catch
            {
                return false;
            }
        }
    }
}
