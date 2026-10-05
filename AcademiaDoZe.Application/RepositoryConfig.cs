namespace AcademiaDoZe.Application
{
    public class RepositoryConfig
    {
        public Enums.AppDatabaseType DatabaseType { get; set; } = Enums.AppDatabaseType.Sqlite;
        public string? ConnectionString { get; set; }
    }
}
