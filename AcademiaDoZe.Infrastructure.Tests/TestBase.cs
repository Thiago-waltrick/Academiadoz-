// Thiago Augusto Ruskowski Waltrick
using System;
using AcademiaDoZe.Infrastructure.Data;

namespace AcademiaDoZe.Infrastructure.Tests
{
    public abstract class TestBase
    {
        // Ambiente fixo: somente SQL Server
        protected static readonly DatabaseType SelectedDatabaseType = DatabaseType.SqlServer;

        // Connection string fixa para os testes de infraestrutura (SQL Server com usuário sa)
        private static string GetConnectionString() => "Server=localhost; Database=db_academia_do_ze; User Id=sa;Password=abcBolinhas12345; TrustServerCertificate=True; Encrypt=True;";

        protected static DbProvider CreateProvider()
        {
            var providerName = "Microsoft.Data.SqlClient";
            var connectionString = GetConnectionString();
            // Sem fallback: usar somente a connection string definida (usuário sa). Se falhar, deixe o erro propagar.
            return new DbProvider(providerName, connectionString, SelectedDatabaseType);
        }
    }
}
