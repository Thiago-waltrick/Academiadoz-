// Thiago Augusto Ruskowski Waltrick
using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Domain.Repositories
{
    public interface IAlunoRepository : IRepository<Aluno>
    {
        Aluno GetByCpf(string cpf);
    }
}
