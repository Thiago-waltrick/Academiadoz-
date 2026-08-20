// Thiago Augusto Ruskowski Waltrick
using AcademiaDoZe.Domain.Entities;
using System.Collections.Generic;

namespace AcademiaDoZe.Domain.Repositories
{
    public interface IAcessoAlunoRepository : IRepository<AcessoAluno>
    {
        IReadOnlyCollection<AcessoAluno> GetByAlunoId(int alunoId);
    }
}
