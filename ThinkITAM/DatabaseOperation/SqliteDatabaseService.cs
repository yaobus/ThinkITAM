using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Data.Sqlite;

namespace ThinkITAM.DatabaseOperation
{
    public class SqliteDatabaseService : IDatabaseService
    {
        private readonly string _connectionString;

        public SqliteDatabaseService(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        private IDbConnection CreateConnection()
        {
            var connection = new SqliteConnection(_connectionString);
            connection.Open();
            return connection;
        }

        public bool TestConnection()
        {
            try
            {
                using var connection = CreateConnection();
                return connection.State == ConnectionState.Open;
            }
            catch
            {
                return false;
            }
        }

        public List<Dictionary<string, object>> ExecuteQuery(string sql, object? param = null)
        {
            Console.WriteLine("ExecuteQuery:\r" + sql);

            using var connection = CreateConnection();
            var result = connection.Query(sql, param).ToList();
            return ConvertDynamicToDictionaryList(result);
        }

        public async Task<List<Dictionary<string, object>>> ExecuteQueryAsync(string sql, object? param = null, CancellationToken cancellationToken = default)
        {
            using var connection = CreateConnection(); // 使用 'using' 而不是 'await using'
            var result = (await connection.QueryAsync(sql, param)).ToList();
            return ConvertDynamicToDictionaryList(result);
        }

        public List<T> ExecuteQuery<T>(string sql, object? param = null) where T : class
        {
            using var connection = CreateConnection();
            return connection.Query<T>(sql, param).ToList();
        }

        public async Task<List<T>> ExecuteQueryAsync<T>(string sql, object? param = null, CancellationToken cancellationToken = default) where T : class
        {
            using var connection = CreateConnection(); // 使用 'using' 而不是 'await using'
            return (await connection.QueryAsync<T>(sql, param)).ToList();
        }

        public object? ExecuteScalar(string sql)
        {
            using var connection = CreateConnection();
            return connection.ExecuteScalar(sql);
        }

        public async Task<object?> ExecuteScalarAsync(string sql, CancellationToken cancellationToken = default)
        {
            using var connection = CreateConnection(); // 使用 'using' 而不是 'await using'
            return await connection.ExecuteScalarAsync(sql);
        }

        public int ExecuteNonQuery(string sql, object? param = null)
        {
            Console.WriteLine("ExecuteNonQuery:\r" + sql);
            using var connection = CreateConnection();
            return connection.Execute(sql, param);
        }

        public async Task<int> ExecuteNonQueryAsync(string sql, object? param = null, CancellationToken cancellationToken = default)
        {
            using var connection = CreateConnection(); // 使用 'using' 而不是 'await using'
            return await connection.ExecuteAsync(sql, param);
        }

        public bool IsTableExists(string tableName)
        {
            var sql = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name=@TableName";
            using var connection = CreateConnection();
            var count = connection.ExecuteScalar<int>(sql, new { TableName = tableName });
            return count > 0;
        }

        public async Task<bool> IsTableExistsAsync(string tableName, CancellationToken cancellationToken = default)
        {
            var sql = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name=@TableName";
            using var connection = CreateConnection(); // 使用 'using' 而不是 'await using'
            var count = await connection.ExecuteScalarAsync<int>(sql, new { TableName = tableName });
            return count > 0;
        }

        public bool CreateTableFromEntity<T>() where T : class
        {
            try
            {
                var sql = SqliteTableCreator.GenerateCreateTableScript<T>();
                if (string.IsNullOrWhiteSpace(sql))
                    return false;

                using var connection = CreateConnection();
                connection.Execute(sql);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"创建 SQLite 表失败: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> CreateTableFromEntityAsync<T>(CancellationToken ct = default) where T : class
        {
            try
            {
                var sql = SqliteTableCreator.GenerateCreateTableScript<T>();
                if (string.IsNullOrWhiteSpace(sql))
                    return false;

                using var connection = CreateConnection(); // 使用 'using' 而不是 'await using'
                await connection.ExecuteAsync(sql);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"异步创建 SQLite 表失败: {ex.Message}");
                return false;
            }
        }

        public bool CreateTableFromSql(string sqliteSql)
        {
            using var connection = CreateConnection();
            connection.Execute(sqliteSql);
            return true;
        }

        public async Task<bool> CreateTableFromSqlAsync(string sqliteSql, CancellationToken ct = default)
        {
            using var connection = CreateConnection(); // 使用 'using' 而不是 'await using'
            await connection.ExecuteAsync(sqliteSql);
            return true;
        }

        // Helper method: 将 dynamic 转换为 Dictionary<string, object>
        private static List<Dictionary<string, object>> ConvertDynamicToDictionaryList(IEnumerable<dynamic> rows)
        {
            var result = new List<Dictionary<string, object>>();
            foreach (var row in rows)
            {
                var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                foreach (var prop in ((IDictionary<string, object>)row))
                {
                    // 处理 DBNull 和 null 值
                    var value = prop.Value;
                    if (value is DBNull || value == null)
                    {
                        dict[prop.Key] = string.Empty;
                    }
                    else
                    {
                        dict[prop.Key] = value;
                    }
                }
                result.Add(dict);
            }
            return result;
        }



        #region 字段检测与添加方法

        public bool CheckAndAddColumnIfNotExists(string tableName, string columnName, string columnType)
        {
            if (string.IsNullOrEmpty(tableName)) throw new ArgumentException("表名不能为空", nameof(tableName));
            if (string.IsNullOrEmpty(columnName)) throw new ArgumentException("字段名不能为空", nameof(columnName));
            if (string.IsNullOrEmpty(columnType)) throw new ArgumentException("字段类型不能为空", nameof(columnType));

            using var connection = CreateConnection();

            bool exists = ColumnExists(tableName, columnName, connection);

            if (!exists)
            {
                AddColumn(tableName, columnName, columnType, connection);
            }

            return exists;
        }


        private static bool ColumnExists(string tableName, string columnName, IDbConnection connection)
        {
            var cmd = (SqliteConnection)connection;
            using var command = connection.CreateCommand();
            command.CommandText = $"PRAGMA table_info({tableName});";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                string name = reader.GetString(reader.GetOrdinal("name"));
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

            var sql = $"INSERT INTO {tableName} ({string.Join(", ", columns)}) VALUES ({string.Join(", ", columns.Select(c => "@" + c))})";
            return connection.Execute(sql, entity);
        }

        public async Task<long> InsertEntityAsync<T>(string tableName, T entity, CancellationToken cancellationToken = default) where T : class
        {
            using var connection = CreateConnection();
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var columns = properties.Select(p => p.Name).ToList();
            var values = properties.Select(p => p.GetValue(entity)).ToList();

            var sql = $"INSERT INTO {tableName} ({string.Join(", ", columns)}) VALUES ({string.Join(", ", columns.Select(c => "@" + c))})";
            return await connection.ExecuteAsync(sql, entity);
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



        // 根据自定义条件更新实体
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