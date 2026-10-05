using System;
using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Infrastructure.Data;

namespace AcademiaDoZe.Application.Mappings;

public static class DatabaseTypeEnumMappingExtensions
{
    public static DatabaseType ToInfrastructure(this AppDatabaseType appDatabaseType)
    {
        return appDatabaseType switch
        {
            AppDatabaseType.SqlServer => DatabaseType.SqlServer,
            AppDatabaseType.MySql => DatabaseType.MySql,
            AppDatabaseType.Sqlite => DatabaseType.Sqlite,
            _ => throw new ArgumentOutOfRangeException(nameof(appDatabaseType), appDatabaseType, "Tipo de banco desconhecido.")
        };
    }

    public static AppDatabaseType ToApplication(this DatabaseType databaseType)
    {
        return databaseType switch
        {
            DatabaseType.SqlServer => AppDatabaseType.SqlServer,
            DatabaseType.MySql => AppDatabaseType.MySql,
            DatabaseType.Sqlite => AppDatabaseType.Sqlite,
            _ => throw new ArgumentOutOfRangeException(nameof(databaseType), databaseType, "Tipo de banco desconhecido.")
        };
    }
}
