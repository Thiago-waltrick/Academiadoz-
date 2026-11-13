using System;

namespace AcademiaDoZe.Application.DTOs
{
    public class AcessoDto
    {
        public int Id { get; set; }
        public int? AlunoId { get; set; }
        public int? ColaboradorId { get; set; }
        public DateTime Entrada { get; set; }
        public DateTime? Saida { get; set; }
        public bool HistoricoLegado { get; set; }
    }
}
