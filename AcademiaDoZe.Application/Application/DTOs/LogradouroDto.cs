// Thiago Augusto Ruskowski Waltrick
using System;

namespace AcademiaDoZe.Application.DTOs
{
    public class LogradouroDto
    {
        public string Nome { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string? Cep { get; set; }
    }
}
