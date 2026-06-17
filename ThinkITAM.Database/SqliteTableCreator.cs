using System.Reflection;

namespace ThinkITAM.Database;

public static class SqliteTableCreator
{
    public static string GenerateCreateTableScript<T>() where T : class
    {
        var tableName = typeof(T).Name;
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var columns = new List<string>();

        foreach (var property in properties)
        {
            var columnName = property.Name;
            var columnType = MapClrTypeToSqliteDbType(property.PropertyType);
            var primaryKey = property.GetCustomAttribute<PrimaryKeyAttribute>() != null;
            var autoIncrement = property.GetCustomAttribute<AutoIncrementAttribute>() != null;

            var columnDefinition = $"{columnName} {columnType}";

            if (primaryKey)
            {
                columnDefinition += " PRIMARY KEY";
                if (autoIncrement && columnType == "INTEGER")
                {
                    columnDefinition += " AUTOINCREMENT";
                }
            }

            columns.Add(columnDefinition);
        }

        return $"CREATE TABLE IF NOT EXISTS {tableName} (\n{string.Join(",\n", columns)}\n);";
    }

    private static string MapClrTypeToSqliteDbType(Type clrType)
    {
        if (clrType == typeof(int) || clrType == typeof(int?))
            return "INTEGER";
        else if (clrType == typeof(long) || clrType == typeof(long?))
            return "INTEGER";
        else if (clrType == typeof(string))
            return "TEXT";
        else if (clrType == typeof(DateTime) || clrType == typeof(DateTime?))
            return "TEXT";
        else if (clrType == typeof(bool) || clrType == typeof(bool?))
            return "BOOLEAN";
        else if (clrType == typeof(decimal) || clrType == typeof(decimal?))
            return "REAL";
        else if (clrType == typeof(double) || clrType == typeof(float) ||
                 clrType == typeof(double?) || clrType == typeof(float?))
            return "REAL";
        else if (clrType == typeof(byte[]) || clrType == typeof(byte?[]))
            return "BLOB";

        throw new NotSupportedException($"不支持的数据类型: {clrType.FullName}");
    }
}
