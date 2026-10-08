// Thiago Augusto Ruskowski Waltrick
using System;

namespace AcademiaDoZe.Application.DTOs
{
    public abstract class PessoaDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DataNascimento { get; set; }
        public string Telefone { get; set; } = string.Empty;
        public LogradouroDto Endereco { get; set; } = new LogradouroDto();
        public byte[]? FotoConteudo { get; set; }
    }
}
