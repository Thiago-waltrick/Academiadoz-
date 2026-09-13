using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace AcademiaDoZe.Infrastructure.Tests
{
    // Arquivo vazio usado apenas para aplicar o atributo de assembly que desabilita
    // a execução paralela de testes. Isso evita deadlocks quando testes de integração
    // que manipulam o mesmo banco são executados simultaneamente.
    internal static class TestCollection { }
}
