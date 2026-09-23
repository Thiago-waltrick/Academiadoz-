// Thiago Augusto Ruskowski Waltrick
using System;

namespace AcademiaDoZe.Application.DTOs
{
    public class ArquivoDto
    {
        public string Nome { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long Tamanho { get; set; }
    }
}
