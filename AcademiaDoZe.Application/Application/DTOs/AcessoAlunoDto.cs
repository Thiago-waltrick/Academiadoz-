// Thiago Augusto Ruskowski Waltrick
using System;

namespace AcademiaDoZe.Application.DTOs
{
    public class AcessoAlunoDto
    {
        public int Id { get; set; }
        public int AlunoId { get; set; }
        public DateTime Entrada { get; set; }
        public DateTime? Saida { get; set; }
        // Tempo restante no contrato (em minutos) quando registrar entrada
        public double? MinutosRestantesContrato { get; set; }
        // Duração em minutos quando registrar saída
        public double? DuracaoMinutos { get; set; }
    }
}
