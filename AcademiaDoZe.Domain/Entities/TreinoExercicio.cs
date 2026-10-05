using AcademiaDoZe.Domain.Exceptions;

namespace AcademiaDoZe.Domain.Entities
{
    public sealed class TreinoExercicio : Entity
    {
        public int SessaoId { get; }
        public string Nome { get; }

        private TreinoExercicio(int id, int sessaoId, string nome) : base(id)
        {
            var nomeNormalizado = nome?.Trim();
            if (sessaoId <= 0 || string.IsNullOrWhiteSpace(nomeNormalizado) || nomeNormalizado.Length > 200)
                throw new DomainException("EXERCICIO_INVALIDO");

            SessaoId = sessaoId;
            Nome = nomeNormalizado;
        }

        public static TreinoExercicio Criar(int sessaoId, string nome) => new(0, sessaoId, nome);

        public static TreinoExercicio Reconstituir(int id, int sessaoId, string nome) => new(id, sessaoId, nome);
    }
}
