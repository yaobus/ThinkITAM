using System.Data;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Data.Sqlite;
using MySqlConnector;

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
            Console.WriteLine(sql);

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


        public bool CreateTableFromEntity<T>() where T : class
        {
            try
            {
                var sqliteSql = SqliteTableCreator.GenerateCreateTableScript<T>();


                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var cmd = new MySqlCommand(sqliteSql, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"创建 MySQL 表失败: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> CreateTableFromEntityAsync<T>(CancellationToken ct = default) where T : class
        {
            try
            {
                var sqliteSql = SqliteTableCreator.GenerateCreateTableScript<T>();


                await using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync(ct);
                    await using (var cmd = new MySqlCommand(sqliteSql, connection))
                    {
                        await cmd.ExecuteNonQueryAsync(ct);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"异步创建 MySQL 表失败: {ex.Message}");
                return false;
            }
        }


        public bool CreateTableFromSql(string sqliteSql)
        {

            if (!string.IsNullOrWhiteSpace(sqliteSql))
            {
                using var connection = new MySqlConnection(_connectionString);
                connection.Open();
                using var command = new MySqlCommand(sqliteSql, connection);
                command.ExecuteNonQuery();
                return true;
            }
            else
            {
                return false;
            }




        }

        public async Task<bool> CreateTableFromSqlAsync(string sql, CancellationToken ct = default)
        {
            Console.WriteLine(sql);
            await using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync(ct);
            await using var command = new MySqlCommand(sql, connection);
            await command.ExecuteNonQueryAsync(ct);
            return true;
        }

        #region 字段检测与添加方法

        public bool CheckAndAddColumnIfNotExists(string tableName, string columnName, string columnType)
        {
            if (string.IsNullOrWhiteSpace(tableName)) throw new ArgumentException("表名不能为空", nameof(tableName));
            if (string.IsNullOrWhiteSpace(columnName)) throw new ArgumentException("字段名不能为空", nameof(columnName));
            if (string.IsNullOrWhiteSpace(columnType)) throw new ArgumentException("字段类型不能为空", nameof(columnType));

            using var connection = CreateConnection();
            //判断数据库是否打开，如果没有则打开
            if (!connection.State.Equals(ConnectionState.Open))
            {
                connection.Open();
            }


            bool exists = ColumnExists(tableName, columnName, connection);

            if (!exists)
            {
                AddColumn(tableName, columnName, columnType, connection);
            }

            return exists;
        }

        private static bool ColumnExists(string tableName, string columnName, IDbConnection connection)
        {
            using var command = connection.CreateCommand();
            command.CommandText = $"DESCRIBE {tableName};";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                string name = reader.GetString(reader.GetOrdinal("Field"));
                if (name.Equals(columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static void AddColumn(string tableName, string columnName, string columnType, IDbConnection connection)
        {
            using var command = connection.CreateCommand();
            command.CommandText = $"ALTER TABLE {tableName} ADD COLUMN {columnName} {columnType};";
            command.ExecuteNonQuery();
        }

        #endregion

        #region 增删查改

        // 插入实体
        public long InsertEntity<T>(string tableName, T entity) where T : class
        {
            using var connection = CreateConnection();
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var columns = properties.Select(p => p.Name).ToList();
            var values = properties.Select(p => p.GetValue(entity)).ToList();

            var sql = $"INSERT INTO {tableName} ({string.Join(", ", columns)}) VALUES ({string.Join(", ", columns.Select(c => "@" + c))}); SELECT LAST_INSERT_ID();";
            var result = connection.ExecuteScalar(sql, entity);
            return Convert.ToInt64(result);
        }

        public async Task<long> InsertEntityAsync<T>(string tableName, T entity, CancellationToken cancellationToken = default) where T : class
        {
            using var connection = CreateConnection();
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var columns = properties.Select(p => p.Name).ToList();

            var sql = $"INSERT INTO {tableName} ({string.Join(", ", columns)}) VALUES ({string.Join(", ", columns.Select(c => "@" + c))}); SELECT LAST_INSERT_ID();";
            var result = await connection.ExecuteScalarAsync(sql, entity);
            return Convert.ToInt64(result);
        }

        // 更新实体
        public bool UpdateEntity<T>(string tableName, T entity) where T : class
        {
            using var connection = CreateConnection();
            var keyProperty = typeof(T).GetProperties().FirstOrDefault(p => p.GetCustomAttributes<KeyAttribute>().Any());
            if (keyProperty == null) throw new InvalidOperationException("Entity must have a property marked with [Key] attribute.");

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var updates = properties.Where(p => !p.Equals(keyProperty))
                                    .Select(p => $"{p.Name} = @{p.Name}")
                                    .ToList();

            var sql = $"UPDATE {tableName} SET {string.Join(", ", updates)} WHERE {keyProperty.Name} = @{keyProperty.Name}";
            return connection.Execute(sql, entity) > 0;
        }

        public async Task<bool> UpdateEntityAsync<T>(string tableName, T entity, CancellationToken cancellationToken = default) where T : class
        {
            using var connection = CreateConnection();
            var keyProperty = typeof(T).GetProperties().FirstOrDefault(p => p.GetCustomAttributes<KeyAttribute>().Any());
            if (keyProperty == null) throw new InvalidOperationException("Entity must have a property marked with [Key] attribute.");

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var updates = properties.Where(p => !p.Equals(keyProperty))
                                    .Select(p => $"{p.Name} = @{p.Name}")
                                    .ToList();

            var sql = $"UPDATE {tableName} SET {string.Join(", ", updates)} WHERE {keyProperty.Name} = @{keyProperty.Name}";
            return await connection.ExecuteAsync(sql, entity) > 0;
        }






        // 查询所有实体
        public List<T> GetAllEntities<T>(string tableName) where T : class
        {
            using var connection = CreateConnection();
            var sql = $"SELECT * FROM {tableName}";
            return connection.Query<T>(sql).ToList();
        }

        public async Task<List<T>> GetAllEntitiesAsync<T>(string tableName, CancellationToken cancellationToken = default) where T : class
        {
            using var connection = CreateConnection();
            var sql = $"SELECT * FROM {tableName}";
            return (await connection.QueryAsync<T>(sql)).ToList();
        }

        // 根据主键查询实体
        public T GetEntityById<T>(string tableName, object id) where T : class
        {
            using var connection = CreateConnection();
            var keyProperty = typeof(T).GetProperties().FirstOrDefault(p => p.GetCustomAttributes<KeyAttribute>().Any());
            if (keyProperty == null) throw new InvalidOperationException("Entity must have a property marked with [Key] attribute.");

            var sql = $"SELECT * FROM {tableName} WHERE {keyProperty.Name} = @Id";
            return connection.QuerySingleOrDefault<T>(sql, new { Id = id });
        }

        public async Task<T> GetEntityByIdAsync<T>(string tableName, object id, CancellationToken cancellationToken = default) where T : class
        {
            using var connection = CreateConnection();
            var keyProperty = typeof(T).GetProperties().FirstOrDefault(p => p.GetCustomAttributes<KeyAttribute>().Any());
            if (keyProperty == null) throw new InvalidOperationException("Entity must have a property marked with [Key] attribute.");

            var sql = $"SELECT * FROM {tableName} WHERE {keyProperty.Name} = @Id";
            return await connection.QuerySingleOrDefaultAsync<T>(sql, new { Id = id });
        }


        public bool UpdateEntity<T>(string tableName, T entity, object conditions) where T : class
        {
            using var connection = CreateConnection();

            var entityType = typeof(T);

            // 获取实体中要更新的属性（排除主键）
            var propertiesToUpdate = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                               .Where(p => p.CanRead && p.GetMethod != null && !IsKeyProperty(p))
                                               .Select(p => $"`{p.Name}` = @{p.Name}")
                                               .ToList();

            if (!propertiesToUpdate.Any())
                throw new InvalidOperationException("No updatable properties found in the entity.");

            // 获取条件对象中的属性
            var conditionProps = conditions.GetType().GetProperties();
            var whereClauses = conditionProps.Select(p => $"`{p.Name}` = @{p.Name}_condition").ToList();

            // 构建 SQL
            var sql = $@"
            UPDATE `{tableName}`
            SET {string.Join(", ", propertiesToUpdate)}
            WHERE {string.Join(" AND ", whereClauses)}";

            // 使用 DynamicParameters 来合并参数
            var parameters = new DynamicParameters(entity);
            foreach (var prop in conditionProps)
            {
                parameters.Add($"{prop.Name}_condition", prop.GetValue(conditions));
            }

            return connection.Execute(sql, parameters) > 0;
        }

        public async Task<bool> UpdateEntityAsync<T>(string tableName, T entity, object conditions, CancellationToken cancellationToken = default) where T : class
        {
            using var connection = CreateConnection();

            var entityType = typeof(T);

            var propertiesToUpdate = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                               .Where(p => p.CanRead && p.GetMethod != null && !IsKeyProperty(p))
                                               .Select(p => $"`{p.Name}` = @{p.Name}")
                                               .ToList();

            if (!propertiesToUpdate.Any())
                throw new InvalidOperationException("No updatable properties found in the entity.");

            var conditionProps = conditions.GetType().GetProperties();
            var whereClauses = conditionProps.Select(p => $"`{p.Name}` = @{p.Name}_condition").ToList();

            var sql = $@"
        UPDATE `{tableName}`
        SET {string.Join(", ", propertiesToUpdate)}
        WHERE {string.Join(" AND ", whereClauses)}";

            var parameters = new DynamicParameters(entity);
            foreach (var prop in conditionProps)
            {
                parameters.Add($"{prop.Name}_condition", prop.GetValue(conditions));
            }

            var rowsAffected = await connection.ExecuteAsync(sql, parameters);
            return rowsAffected > 0;
        }

        private bool IsKeyProperty(PropertyInfo property)
        {
            return property.GetCustomAttributes(typeof(KeyAttribute), false).Any();
        }

        #endregion

    }
}
