using System;
using System.Collections.Generic;

namespace AcademiaDoZe.Application.DTOs
{
    public sealed class TreinoSessaoDto
    {
        public int Id { get; set; }
        public DateTime IniciadaEm { get; set; }
        public IReadOnlyCollection<TreinoExercicioDto> Exercicios { get; set; } = new List<TreinoExercicioDto>();
    }
}
