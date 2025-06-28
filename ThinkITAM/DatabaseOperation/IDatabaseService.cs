namespace ThinkITAM.DatabaseOperation
{
    /// <summary>
    /// 统一数据库访问接口，屏蔽底层差异，提升易用性与安全性
    /// </summary>
    public interface IDatabaseService
    {
        /// <summary>
        /// 测试数据库连接是否成功
        /// </summary>
        /// <returns>是否连接成功</returns>
        bool TestConnection();

        /// <summary>
        /// 同步执行 SQL 查询并返回数据行列表（字典形式）
        /// </summary>
        /// <param name="sql">SQL 查询语句</param>
        /// <param name="param">参数对象</param>
        /// <returns>查询结果列表</returns>
        List<Dictionary<string, object>> ExecuteQuery(string sql, object? param = null);

        /// <summary>
        /// 异步执行 SQL 查询并返回数据行列表（字典形式）
        /// </summary>
        /// <param name="sql">SQL 查询语句</param>
        /// <param name="param">参数对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>查询结果列表</returns>
        Task<List<Dictionary<string, object>>> ExecuteQueryAsync(string sql, object? param = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// 同步执行 SQL 查询并返回指定类型的对象列表
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="sql">SQL 查询语句</param>
        /// <param name="param">参数对象</param>
        /// <returns>泛型对象列表</returns>
        List<T> ExecuteQuery<T>(string sql, object? param = null) where T : class;

        /// <summary>
        /// 异步执行 SQL 查询并返回指定类型的对象列表
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="sql">SQL 查询语句</param>
        /// <param name="param">参数对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>泛型对象列表</returns>
        Task<List<T>> ExecuteQueryAsync<T>(string sql, object? param = null, CancellationToken cancellationToken = default) where T : class;


        /// <summary>
        /// 执行 SQL 并返回第一行第一列的结果（如 COUNT 查询）
        /// </summary>
        /// <param name="sql">SQL 查询语句</param>
        /// <returns>结果值</returns>
        object? ExecuteScalar(string sql);

        /// <summary>
        /// 异步执行 SQL 并返回第一行第一列的结果
        /// </summary>
        /// <param name="sql">SQL 查询语句</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>结果值</returns>
        Task<object?> ExecuteScalarAsync(string sql, CancellationToken cancellationToken = default);

        /// <summary>
        /// 插入、更新或删除数据（无返回结果集）
        /// </summary>
        /// <param name="sql">SQL 语句</param>
        /// <param name="param">参数对象</param>
        int ExecuteNonQuery(string sql, object? param = null);

        /// <summary>
        /// 异步插入、更新或删除数据
        /// </summary>
        /// <param name="sql">SQL 语句</param>
        /// <param name="param">参数对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>受影响的行数</returns>
        Task<int> ExecuteNonQueryAsync(string sql, object? param = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// 判断表是否存在
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns>是否存在</returns>
        bool IsTableExists(string tableName);

        /// <summary>
        /// 异步判断表是否存在
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>是否存在</returns>
        Task<bool> IsTableExistsAsync(string tableName, CancellationToken cancellationToken = default);


        bool CreateTableFromEntity<T>() where T : class;
        Task<bool> CreateTableFromEntityAsync<T>(CancellationToken ct = default) where T : class;


        bool CreateTableFromSql(string sqliteSql);
        Task<bool> CreateTableFromSqlAsync(string sqliteSql, CancellationToken ct = default);


        /// <summary>
        /// 检查指定表是否存在某个字段，如果不存在则添加该字段
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="columnName">字段名</param>
        /// <param name="columnType">字段类型（如 TEXT, VARCHAR(255), INT 等）</param>
        /// <returns>是否成功执行（包括字段已存在的情况）</returns>
        bool CheckAndAddColumnIfNotExists(string tableName, string columnName, string columnType);




        #region 增删查改

        /// <summary>
        /// 插入一条记录到指定的数据库表中
        /// </summary>
        long InsertEntity<T>(string tableName, T entity) where T : class;

        /// <summary>
        /// 异步插入一条记录到指定的数据库表中
        /// </summary>
        Task<long> InsertEntityAsync<T>(string tableName, T entity, CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// 更新一条记录到指定的数据库表中
        /// </summary>
        bool UpdateEntity<T>(string tableName, T entity) where T : class;


        /// <summary>
        /// 异步更新一条记录到指定的数据库表中
        /// </summary>
        Task<bool> UpdateEntityAsync<T>(string tableName, T entity, CancellationToken cancellationToken = default) where T : class;


        // 同步版本
        bool UpdateEntity<T>(string tableName, T entity, object conditions) where T : class;

        // 异步版本
        Task<bool> UpdateEntityAsync<T>(string tableName, T entity, object conditions, CancellationToken cancellationToken = default) where T : class;


        /// <summary>
        /// 查询所有记录
        /// </summary>
        List<T> GetAllEntities<T>(string tableName) where T : class;

        /// <summary>
        /// 异步查询所有记录
        /// </summary>
        Task<List<T>> GetAllEntitiesAsync<T>(string tableName, CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// 根据主键查询记录
        /// </summary>
        T GetEntityById<T>(string tableName, object id) where T : class;

        /// <summary>
        /// 异步根据主键查询记录
        /// </summary>
        Task<T> GetEntityByIdAsync<T>(string tableName, object id, CancellationToken cancellationToken = default) where T : class;

        #endregion

    }

}

