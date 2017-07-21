using System;
using System.Data;

namespace Shared.Database
{
    public static class Extensions
    {
        public static T Read<T>(this DataRow row, string columnName)
        {
            object val = row[columnName];

            if (typeof(T).IsEnum)
                return (T)Enum.ToObject(typeof(T), val);

            if (val is DBNull)
                return default(T);

            return (T)Convert.ChangeType(val, typeof(T));
        }
    }
}
