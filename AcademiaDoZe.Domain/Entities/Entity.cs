// Thiago Augusto Ruskowski Waltrick
using System;
using AcademiaDoZe.Domain.Exceptions;

namespace AcademiaDoZe.Domain.Entities
{
    public abstract class Entity
    {
        public int Id { get; protected set; }

        protected Entity(int id)
        {
            if (id < 0)
                throw new DomainException("ID_NEGATIVO");

            Id = id;
        }
    }
}
