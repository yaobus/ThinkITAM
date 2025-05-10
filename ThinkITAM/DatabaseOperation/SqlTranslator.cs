using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ThinkITAM.DatabaseOperation;

public enum TargetDatabaseType
{
    Sqlite,
    MySql,
    SqlServer,
    MariaDB // 新增 MariaDB 类型
}



public static class SqlTranslator
{

    private static readonly HashSet<string> MySqlReservedKeywords = new(StringComparer.OrdinalIgnoreCase)
    {
        "GROUP", "ORDER", "USER", "PASSWORD", "KEY", "INDEX", "LEFT", "RIGHT", "DATE", "LIMIT"
    };

    public static string TranslateCreateTable(string sqliteSql, TargetDatabaseType targetType)
    {
        if (string.IsNullOrWhiteSpace(sqliteSql))
            return sqliteSql;

        // 预处理：移除多余的空白字符和换行符
        var cleanedSql = PreprocessSql(sqliteSql);

        // 移除 IF NOT EXISTS（不同数据库语法不同）
        var result = Regex.Replace(cleanedSql, @"IF\s+NOT\s+EXISTS", "", RegexOptions.IgnoreCase).Trim();

        // 替换关键字和类型
        switch (targetType)
        {
            case TargetDatabaseType.MySql:
                result = TranslateToMySql(result);
                break;
            case TargetDatabaseType.SqlServer:
                result = TranslateToSqlServer(result);
                break;
            case TargetDatabaseType.MariaDB:
                result = TranslateToMariaDB(result);
                break;
            default:
                throw new NotSupportedException($"不支持的目标数据库类型: {targetType}");
        }

        Console.WriteLine(result); // 输出转换后的SQL用于调试

        return result;
    }

    private static string PreprocessSql(string sql)
    {
        // 去除所有换行符(\r\n, \n, \r)，并简化连续的空白字符为单个空格
        var result = Regex.Replace(sql, @"\s+", " ");
        result = result.Replace("\"", ""); // 如果你确定不需要双引号，可以直接移除它们

        return result.Trim();
    }

    private static string TranslateToMySql(string sql)
    {
        var result = sql;

        // 替换特定的 SQL 关键字
        result = Regex.Replace(result, @"\bAUTOINCREMENT\b", "AUTO_INCREMENT", RegexOptions.IgnoreCase);
        result = Regex.Replace(result, @"\bBOOLEAN\b", "TINYINT(1)", RegexOptions.IgnoreCase);
        result = Regex.Replace(result, @"\bREAL\b", "FLOAT", RegexOptions.IgnoreCase);

        // ✅ 将 TEXT 改为 VARCHAR(255)（而不是 LONGTEXT）
        result = Regex.Replace(result, @"\bTEXT\b", "VARCHAR(255)", RegexOptions.IgnoreCase);

        result = Regex.Replace(result, @"\bINTEGER\b", "INT", RegexOptions.IgnoreCase);

        // 替换双引号为反引号
        result = Regex.Replace(result, @"""(\w+)""", "`$1`");

        // 包裹未被包裹的标识符
        result = Regex.Replace(result, @"\b(\w+)\b", match =>
        {
            var word = match.Value;
            if (!Regex.IsMatch(word, @"^`?\w+`?$"))
            {
                return $"`{word}`";
            }
            return word;
        });

        // 将 ENGINE 和 CHARSET 插入到 CREATE TABLE 语句结尾处
        result = Regex.Replace(result, @"(?s)(CREATE\s+TABLE\s+`?\w+`?\s*$$.*?$$)\s*;",
            "$1 ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;", RegexOptions.IgnoreCase);

        return result;
    }


    private static string TranslateToSqlServer(string sql)
    {
        var result = sql;

        // 替换特定的 SQL 关键字
        result = Regex.Replace(result, @"\bAUTOINCREMENT\b", "IDENTITY(1,1)", RegexOptions.IgnoreCase);
        result = Regex.Replace(result, @"\bBOOLEAN\b", "BIT", RegexOptions.IgnoreCase);
        result = Regex.Replace(result, @"\bREAL\b", "FLOAT", RegexOptions.IgnoreCase);
        result = Regex.Replace(result, @"\bTEXT\b", "NVARCHAR(MAX)", RegexOptions.IgnoreCase);
        result = Regex.Replace(result, @"\bINTEGER\b", "INT", RegexOptions.IgnoreCase);

        // 包裹表名和列名
        result = Regex.Replace(result, @"\b(\w+)\b", match =>
        {
            var word = match.Value;
            if (!Regex.IsMatch(word, @"^$?\w+$?$")) // 检查是否已经是包裹的标识符
            {
                return $"[{word}]";
            }
            return word;
        });

        // 包装为 IF NOT EXISTS
        var tableNameMatch = Regex.Match(result, @"CREATE\s+TABLE\s+(?:IF\s+NOT\s+EXISTS\s+)?`?(\w+)`?");
        if (tableNameMatch.Success)
        {
            var tableName = tableNameMatch.Groups[1].Value;
            result = $@"
                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='{tableName}' AND xtype='U')
                    BEGIN
                    {result}
                    END";
        }

        return result;
    }

    private static string TranslateToMariaDB(string sql)
    {
        // MariaDB 的翻译逻辑与 MySQL 基本一致
        return TranslateToMySql(sql);
    }
}



