// Thiago Augusto Ruskowski Waltrick
using AcademiaDoZe.Domain.Entities;
using System.Collections.Generic;

namespace AcademiaDoZe.Domain.Repositories
{
    public interface IMatriculaRepository : IRepository<Matricula>
    {
        IReadOnlyCollection<Matricula> GetByAlunoId(int alunoId);
        Matricula ObterMatriculaAtivaPorAluno(int alunoId);
        bool PossuiMatriculaAtiva(int alunoId);
    }
}
