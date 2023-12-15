using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_6
{
    public static class DataReaderExtensions
    {
        private static bool TryGetOrdinal(this IDataReader reader, string column, out int order)
        {
            order = -1;
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i) == column)
                {
                    order = i;
                    return true;
                }
            }
            return false;
        }

        public static string? GetString(this IDataReader reader, string columnName)
        {
            if (reader.TryGetOrdinal(columnName, out int order))
                return reader.GetString(order);
            return default;
        }

        public static int GetInt32(this IDataReader reader, string columnName)
        {
            if (reader.TryGetOrdinal(columnName, out int order))
                return reader.GetInt32(order);
            return default;
        }

        public static int? GetInt32Nullable(this IDataReader reader, string columnName)
        {
            if (reader.TryGetOrdinal(columnName, out int order))
            {
                if (!reader.IsDBNull(order))
                {
                    return reader.GetInt32(order);
                }
            }
            return null;
        }
        public static DateTime GetDateTime(IDataReader reader, string columnName)
        {
            if (reader.TryGetOrdinal(columnName, out int order))
                return reader.GetDateTime(order);
            return default;
        }
        public static DateTime? GetNullableDateTime(IDataReader reader, string columnName)
        {
            if (reader.TryGetOrdinal(columnName, out int order))
            {
                if (reader.IsDBNull(order))
                    return null;
                return reader.GetDateTime(order);
            }
            return null;
        }

    }
}
