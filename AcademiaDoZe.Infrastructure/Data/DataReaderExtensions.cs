// Thiago Augusto Ruskowski Waltrick
using System;
using System.Data;

namespace AcademiaDoZe.Infrastructure.Data
{
    public static class DataReaderExtensions
    {
        public static string GetStringOrDefault(this IDataRecord reader, string columnName)
        {
            var idx = reader.GetOrdinal(columnName);
            if (reader.IsDBNull(idx)) return string.Empty;
            return reader.GetString(idx);
        }

        public static int GetInt32OrDefault(this IDataRecord reader, string columnName)
        {
            var idx = reader.GetOrdinal(columnName);
            if (reader.IsDBNull(idx)) return default;
            return reader.GetInt32(idx);
        }

        public static long GetInt64OrDefault(this IDataRecord reader, string columnName)
        {
            var idx = reader.GetOrdinal(columnName);
            if (reader.IsDBNull(idx)) return default;
            return reader.GetInt64(idx);
        }

        public static bool GetBooleanOrDefault(this IDataRecord reader, string columnName)
        {
            var idx = reader.GetOrdinal(columnName);
            if (reader.IsDBNull(idx)) return default;
            return reader.GetBoolean(idx);
        }

        public static DateTime? GetDateTimeOrNull(this IDataRecord reader, string columnName)
        {
            var idx = reader.GetOrdinal(columnName);
            if (reader.IsDBNull(idx)) return null;
            return reader.GetDateTime(idx);
        }

        public static T? GetValueOrDefault<T>(this IDataRecord reader, string columnName)
        {
            var idx = reader.GetOrdinal(columnName);
            if (reader.IsDBNull(idx)) return default;
            return (T)reader.GetValue(idx);
        }
    }
}
