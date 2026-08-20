// Thiago Augusto Ruskowski Waltrick
using System.Collections.Generic;

namespace AcademiaDoZe.Domain.Repositories
{
    public interface IRepository<T> where T : class
    {
        T GetById(int id);
        IReadOnlyCollection<T> GetAll();
        void Add(T entity);
        void Update(T entity);
        void Remove(T entity);
    }
}
