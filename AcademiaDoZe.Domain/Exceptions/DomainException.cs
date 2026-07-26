// Thiago Augusto Ruskowski Waltrick
using System;

namespace AcademiaDoZe.Domain.Exceptions
{
    public sealed class DomainException : Exception
    {
        public DomainException(string message) : base(message) { }
    }
}
