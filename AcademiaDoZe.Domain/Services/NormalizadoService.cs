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

        public static string ParaMaiusculo(string? input) => (input ?? string.Empty).ToUpperInvariant();

        public static bool EhEmailValido(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            try
            {
                var trimmed = email.Trim();
                // Simple regex for email
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
