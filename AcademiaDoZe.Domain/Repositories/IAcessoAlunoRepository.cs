// Thiago Augusto Ruskowski Waltrick
using AcademiaDoZe.Domain.Entities;
using System.Collections.Generic;

namespace AcademiaDoZe.Domain.Repositories
{
    public interface IAcessoAlunoRepository : IRepository<AcessoAluno>
    {
        IReadOnlyCollection<AcessoAluno> GetByAlunoId(int alunoId);
        IReadOnlyCollection<AcessoAluno> GetByAlunoIdAndDate(int alunoId, System.DateTime date);
        IReadOnlyCollection<(int Id, int AlunoId, System.DateTime DataAcesso)> GetHistoricoLegado(System.DateTime inicio, System.DateTime fim);
    }
}
