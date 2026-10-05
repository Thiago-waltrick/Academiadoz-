using System.Collections.Generic;

namespace AcademiaDoZe.Application.DTOs
{
    public sealed class TreinoExercicioDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public IReadOnlyCollection<TreinoSerieDto> Series { get; set; } = new List<TreinoSerieDto>();
    }
}
