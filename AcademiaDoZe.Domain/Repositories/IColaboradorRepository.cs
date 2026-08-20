// Thiago Augusto Ruskowski Waltrick
using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Domain.Repositories
{
    public interface IColaboradorRepository : IRepository<Colaborador>
    {
        Colaborador GetByCpf(string cpf);
    }
}
