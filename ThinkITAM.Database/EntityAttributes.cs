namespace ThinkITAM.Database;

/// <summary>
/// 标记属性为主键
/// </summary>
public class PrimaryKeyAttribute : Attribute
{
}

/// <summary>
/// 标记整数类型的属性为自增列
/// </summary>
public class AutoIncrementAttribute : Attribute
{
}
