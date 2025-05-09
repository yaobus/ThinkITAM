using Microsoft.Data.Sqlite;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ThinkITAM.DatabaseOperation
{
    public class MySqlDatabaseService : IDatabaseService
    {
        private readonly string _connectionString;

        public MySqlDatabaseService(string connectionString)
        {
            _connectionString = connectionString;
        }

        private MySqlConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        private MySqlCommand CreateCommand(MySqlConnection connection, string sql, object? param)
        {
            var command = connection.CreateCommand();
            command.CommandText = sql;

            if (param != null)
            {
                foreach (var prop in param.GetType().GetProperties())
                {
                    var name = "@" + prop.Name;
                    var value = prop.GetValue(param) ?? DBNull.Value;
                    command.Parameters.AddWithValue(name, value);
                }
            }

            return command;
        }

        //private List<Dictionary<string, object>> ReadToDictionaryList(MySqlDataReader reader)
        //{
        //    var results = new List<Dictionary<string, object>>();
        //    while (reader.Read())
        //    {
        //        var row = new Dictionary<string, object>();
        //        for (var i = 0; i < reader.FieldCount; i++)
        //        {
        //            row[reader.GetName(i)] = reader.IsDBNull(i) ? null! : reader.GetValue(i);
        //        }
        //        results.Add(row);
        //    }
        //    return results;
        //}


        private List<Dictionary<string, object>> ReadToDictionaryList(MySqlDataReader reader)
        {
            var results = new List<Dictionary<string, object>>();

            while (reader.Read())
            {
                var row = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase); // 忽略大小写

                for (var i = 0; i < reader.FieldCount; i++)
                {
                    string columnName = reader.GetName(i);

                    if (reader.IsDBNull(i))
                    {
                        row[columnName] = string.Empty; // 替换 null 为 string.Empty
                    }
                    else
                    {
                        // 可选：统一处理常见类型，提升健壮性
                        var value = reader.GetValue(i);
                        if (value is DBNull)
                            row[columnName] = string.Empty;
                        else if (value is byte[] byteArray)
                            row[columnName] = Encoding.UTF8.GetString(byteArray); // 如果需要转字符串
                        else
                            row[columnName] = value;
                    }
                }

                results.Add(row);
            }

            return results;
        }



        public bool TestConnection()
        {
            try
            {
                using var connection = CreateConnection();
                connection.Open();
                return connection.State == ConnectionState.Open;
            }
            catch
            {
                return false;
            }
        }

        public List<Dictionary<string, object>> ExecuteQuery(string sql, object? param = null)
        {
            using var connection = CreateConnection();
            connection.Open();
            using var command = CreateCommand(connection, sql, param);
            using var reader = command.ExecuteReader();
            return ReadToDictionaryList(reader);
        }

        public async Task<List<Dictionary<string, object>>> ExecuteQueryAsync(string sql, object? param = null, CancellationToken cancellationToken = default)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync(cancellationToken);
            await using var command = CreateCommand(connection, sql, param);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            return ReadToDictionaryList(reader);
        }

        public List<T> ExecuteQuery<T>(string sql, object? param = null) where T : class
        {
            var rows = ExecuteQuery(sql, param);
            return ConvertRowsToObjects<T>(rows);
        }

        public async Task<List<T>> ExecuteQueryAsync<T>(string sql, object? param = null, CancellationToken cancellationToken = default) where T : class
        {
            var rows = await ExecuteQueryAsync(sql, param, cancellationToken);
            return ConvertRowsToObjects<T>(rows);
        }

        private List<T> ConvertRowsToObjects<T>(List<Dictionary<string, object>> rows) where T : class
        {
            var list = new List<T>();
            foreach (var row in rows)
            {
                var json = JsonSerializer.Serialize(row);
                var obj = JsonSerializer.Deserialize<T>(json);
                if (obj != null)
                    list.Add(obj);
            }
            return list;
        }

        public object? ExecuteScalar(string sql)
        {
            using var connection = CreateConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            return command.ExecuteScalar();
        }

        public async Task<object?> ExecuteScalarAsync(string sql, CancellationToken cancellationToken = default)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync(cancellationToken);
            await using var command = connection.CreateCommand();
            command.CommandText = sql;
            return await command.ExecuteScalarAsync(cancellationToken);
        }

        public int ExecuteNonQuery(string sql, object? param = null)
        {
            using var connection = CreateConnection();
            connection.Open();
            using var command = CreateCommand(connection, sql, param);
            return command.ExecuteNonQuery();
        }

        public async Task<int> ExecuteNonQueryAsync(string sql, object? param = null, CancellationToken cancellationToken = default)
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync(cancellationToken);
            await using var command = CreateCommand(connection, sql, param);
            return await command.ExecuteNonQueryAsync(cancellationToken);
        }

        public bool IsTableExists(string tableName)
        {
            const string sql = "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = DATABASE() AND table_name = @TableName";
            using var connection = CreateConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.Parameters.AddWithValue("@TableName", tableName);
            var result = command.ExecuteScalar();
            return Convert.ToInt32(result) > 0;
        }

        public async Task<bool> IsTableExistsAsync(string tableName, CancellationToken cancellationToken = default)
        {
            const string sql = "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = DATABASE() AND table_name = @TableName";
            await using var connection = CreateConnection();
            await connection.OpenAsync(cancellationToken);
            await using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.Parameters.AddWithValue("@TableName", tableName);
            var result = await command.ExecuteScalarAsync(cancellationToken);
            return Convert.ToInt32(result) > 0;
        }
    }
}
