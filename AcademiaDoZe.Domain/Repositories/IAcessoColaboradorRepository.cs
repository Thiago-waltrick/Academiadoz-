// Thiago Augusto Ruskowski Waltrick
using AcademiaDoZe.Domain.Entities;
using System.Collections.Generic;

namespace AcademiaDoZe.Domain.Repositories
{
    public interface IAcessoColaboradorRepository : IRepository<AcessoColaborador>
    {
        IReadOnlyCollection<AcessoColaborador> GetByColaboradorId(int colaboradorId);
        IReadOnlyCollection<AcessoColaborador> GetByColaboradorIdAndDate(int colaboradorId, System.DateTime date);
    }
}
