using System;
using AcademiaDoZe.Domain.Exceptions;

namespace AcademiaDoZe.Domain.Entities
{
    public sealed class TreinoSessao : Entity
    {
        public DateTime IniciadaEm { get; }
        public DateTime? ConcluidaEm { get; private set; }
        public bool Concluida => ConcluidaEm.HasValue;

        private TreinoSessao(int id, DateTime iniciadaEm, DateTime? concluidaEm) : base(id)
        {
            if (iniciadaEm == default || (concluidaEm.HasValue && concluidaEm.Value < iniciadaEm))
                throw new DomainException("DATAS_TREINO_INVALIDAS");

            IniciadaEm = iniciadaEm;
            ConcluidaEm = concluidaEm;
        }

        public static TreinoSessao Criar(DateTime iniciadaEm) => new(0, iniciadaEm, null);

        public static TreinoSessao Reconstituir(int id, DateTime iniciadaEm, DateTime? concluidaEm) =>
            new(id, iniciadaEm, concluidaEm);

        public void Finalizar(DateTime concluidaEm)
        {
            if (ConcluidaEm.HasValue)
                throw new DomainException("TREINO_JA_CONCLUIDO");
            if (concluidaEm < IniciadaEm)
                throw new DomainException("DATA_CONCLUSAO_INVALIDA");

            ConcluidaEm = concluidaEm;
        }
    }
}
