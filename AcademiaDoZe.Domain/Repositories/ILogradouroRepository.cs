// Thiago Augusto Ruskowski Waltrick
using AcademiaDoZe.Domain.Entities;
using System.Collections.Generic;

namespace AcademiaDoZe.Domain.Repositories
{
    public interface ILogradouroRepository : IRepository<Logradouro>
    {
        IReadOnlyCollection<Logradouro> BuscarPorCep(string cep);
    }
}
