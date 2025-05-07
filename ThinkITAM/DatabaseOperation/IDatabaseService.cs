using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
