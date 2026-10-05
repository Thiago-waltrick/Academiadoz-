namespace AcademiaDoZe.Application.DTOs
{
    public sealed class TreinoSerieDto
    {
        public int Id { get; set; }
        public int Numero { get; set; }
        public decimal CargaKg { get; set; }
        public int Repeticoes { get; set; }
        public bool Concluida { get; set; }
    }
}
