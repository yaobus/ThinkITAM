using System.IO;
using System.Text.Json;
using System.Windows.Controls;

namespace ThinkITAM.Functions.DataGridColumn;

/// <summary>
/// 数据表列排序
/// </summary>
public class DataGridColumnOrderClass
{
    /// <summary>
    /// 保存布局：仅记录路径、位置和宽度
    /// </summary>
    public static void SaveColumnOrder(DataGrid dataGrid)
    {
        var configs = dataGrid.Columns
            .Where(c => !string.IsNullOrEmpty(c.SortMemberPath)) // 过滤掉没有绑定路径的列
            .Select(column => new ColumnOrderConfig
            {
                SortPath = column.SortMemberPath,
                DisplayIndex = column.DisplayIndex,
                Width = column.ActualWidth
            }).ToList();
        
        var dataGridName = dataGrid.Name;
        
        string json = JsonSerializer.Serialize(configs);
        File.WriteAllText(GetSavePath(dataGridName), json);
    }

    /// <summary>
    /// 创建存储路径
    /// </summary>
    /// <param name="dataGridName"></param>
    /// <returns></returns>
    private static string GetSavePath(string dataGridName)
    {
        // 获取当前用户的文档目录
        string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string appDataPath = Path.Combine(documentsPath, "ThinkITAM"); // 自定义应用数据目录
        string dbConfigPath = Path.Combine(appDataPath, "DataGridColumnOrder");
        string configFilePath = Path.Combine(dbConfigPath, $"{dataGridName}.json");

        // 如果目录不存在，则创建
        if (!Directory.Exists(dbConfigPath))
        {
            Directory.CreateDirectory(dbConfigPath);
        }

        return configFilePath;
    }

    /// <summary>
    /// 加载布局
    /// </summary>
    public static void LoadColumnOrder(DataGrid grid)
    {
        var dataGridName = grid.Name;
        var filePath = GetSavePath(dataGridName);
        if (!File.Exists(filePath)) return;

        try
        {
            string json = File.ReadAllText(filePath);
            var layout = JsonSerializer.Deserialize<List<ColumnOrderConfig>>(json);
            if (layout == null) return;

            // 关键：按 DisplayIndex 升序应用，防止索引冲突
            foreach (var item in layout.OrderBy(x => x.DisplayIndex))
            {
                var column = grid.Columns.FirstOrDefault(c => c.SortMemberPath == item.SortPath);
                if (column != null)
                {
                    column.DisplayIndex = item.DisplayIndex;
                    column.Width = new DataGridLength(item.Width);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"加载列配置失败: {ex.Message}");
            //System.Diagnostics.Debug.WriteLine();
        }
    }
    
    
    /// <summary>
    /// 重置 DataGrid 布局：删除本地文件并恢复 UI 默认状态
    /// </summary>
    /// <param name="grid">目标 DataGrid</param>
    public static void ResetLayout(DataGrid grid)
    {
        var dataGridName = grid.Name;
        
        // 1. 删除本地持久化文件
        DeleteColumnOrder(dataGridName);

        // 2. 恢复 UI 状态
        // 必须按 0 到 Count-1 的顺序赋值，否则 WPF 可能会抛出索引冲突异常
        var columns = grid.Columns.ToList();
        
        // 按照它们在代码/XAML中定义的原始顺序进行恢复
        for (int i = 0; i < columns.Count; i++)
        {
            columns[i].DisplayIndex = i;
            
            // 可选：恢复默认宽度
            // 如果你希望恢复到自动宽度：
            columns[i].Width = DataGridLength.Auto;
            // 或者恢复到你指定的某个默认像素值：
            // columns[i].Width = new DataGridLength(100);
        }

        // 3. 强制 UI 刷新（可选，通常赋值后会自动触发）
        grid.UpdateLayout();
    }
    
    
    /// <summary>
    /// 删除指定 DataGrid 的排序/布局配置文件
    /// </summary>
    /// <param name="dataGridName">DataGrid 的名称（需与保存时一致）</param>
    /// <returns>返回是否成功删除（如果文件不存在也视为成功，或者根据需求返回 false）</returns>
    private static bool DeleteColumnOrder(string dataGridName)
    {
        try
        {
            string filePath = GetSavePath(dataGridName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }
            return false; // 文件原本就不存在
        }
        catch (Exception ex)
        {

            Console.WriteLine($"删除列配置失败: {ex.Message}");
            return false;
        }
    }
    
}

/// <summary>
/// 列配置模型
/// </summary>
public class ColumnOrderConfig
{
    public string SortPath { get; set; }     // 对应 SortMemberPath
    public int DisplayIndex { get; set; }    // 显示顺序
    public double Width { get; set; }        // 像素宽度
}