using AcademiaDoZe.Domain.Exceptions;

namespace AcademiaDoZe.Domain.Entities
{
    public sealed class TreinoSerie : Entity
    {
        public int ExercicioId { get; }
        public int Numero { get; }
        public decimal CargaKg { get; private set; }
        public int Repeticoes { get; private set; }
        public bool Concluida { get; private set; }

        private TreinoSerie(int id, int exercicioId, int numero, decimal cargaKg, int repeticoes, bool concluida) : base(id)
        {
            if (exercicioId <= 0 || numero <= 0)
                throw new DomainException("SERIE_INVALIDA");

            ExercicioId = exercicioId;
            Numero = numero;
            Atualizar(cargaKg, repeticoes);
            Concluida = concluida;
        }

        public static TreinoSerie Criar(int exercicioId, int numero, decimal cargaKg, int repeticoes) =>
            new(0, exercicioId, numero, cargaKg, repeticoes, false);

        public static TreinoSerie Reconstituir(int id, int exercicioId, int numero, decimal cargaKg, int repeticoes, bool concluida) =>
            new(id, exercicioId, numero, cargaKg, repeticoes, concluida);

        public void Atualizar(decimal cargaKg, int repeticoes)
        {
            if (cargaKg < 0 || cargaKg > 999999.99m || decimal.Round(cargaKg, 2) != cargaKg || repeticoes <= 0)
                throw new DomainException("CARGA_OU_REPETICOES_INVALIDAS");

            CargaKg = cargaKg;
            Repeticoes = repeticoes;
        }

        public void MarcarConcluida() => Concluida = true;
    }
}
