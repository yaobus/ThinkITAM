# ThinkITAM.Database

ThinkITAM 数据库操作类库，提供统一的数据库访问接口，支持 SQLite、MySQL/MariaDB 和 SQL Server。

## 功能特性

- 统一的 `IDatabaseService` 接口，屏蔽底层数据库差异
- 支持 SQLite、MySQL/MariaDB、SQL Server 三种数据库
- 提供同步和异步方法
- 支持泛型查询和实体操作
- 自动表创建和字段管理
- 基于 Dapper 的高性能数据访问

## 快速开始

### 1. 配置数据库

```csharp
// SQLite 配置
var config = new DatabaseConfig
{
    Type = "sqlite",
    NickName = "MyProject",
    Path = "C:\\Data\\mydb.db"
};

// MySQL 配置
var config = new DatabaseConfig
{
    Type = "mysql",
    NickName = "MyProject",
    Host = "localhost",
    Port = 3306,
    UserName = "root",
    Password = "password",
    DatabaseName = "thinkitam"
};
```

### 2. 创建数据库服务

```csharp
IDatabaseService dbService = DatabaseServiceFactory.CreateService(config);
```

### 3. 测试连接

```csharp
bool isConnected = await dbService.TestConnectionAsync();
```

### 4. 执行查询

```csharp
// 返回字典列表
var rows = dbService.ExecuteQuery("SELECT * FROM Users WHERE Age > @Age", new { Age = 18 });

// 返回强类型列表
var users = dbService.ExecuteQuery<User>("SELECT * FROM Users");

// 异步查询
var users = await dbService.ExecuteQueryAsync<User>("SELECT * FROM Users");
```

## 注意事项

1. **连接超时**：`TestConnectionAsync()` 方法默认超时时间为 3 秒
2. **NULL 值处理**：所有数据库 NULL 值都会被转换为 `string.Empty`
3. **参数化查询**：建议使用参数化查询防止 SQL 注入
4. **线程安全**：每次调用都会创建新的连接，确保线程安全

## 许可证

本项目为 ThinkITAM 的一部分，遵循原项目的许可协议。
