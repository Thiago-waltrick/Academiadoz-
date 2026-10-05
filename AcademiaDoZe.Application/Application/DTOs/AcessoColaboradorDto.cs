// Thiago Augusto Ruskowski Waltrick
using System;

namespace AcademiaDoZe.Application.DTOs
{
    public class AcessoColaboradorDto
    {
        public int Id { get; set; }
        public int ColaboradorId { get; set; }
        public DateTime Entrada { get; set; }
        public DateTime? Saida { get; set; }
        // Tempo trabalhado no dia em minutos (após saída)
        public double? MinutosTrabalhadosDia { get; set; }
        // Excedeu jornada permitida?
        public bool? ExcedeuJornada { get; set; }
    }
}
