using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ThinkITAM.Functions.Import;
public static class ExcelImporter
{
    /// <summary>
    /// 从Excel文件中导入数据并转换为指定类型的对象列表
    /// </summary>
    /// <typeparam name="T">目标实体类型</typeparam>
    /// <param name="filePath">Excel文件路径</param>
    /// <returns>实体对象列表</returns>
    public static List<T> ImportFromExcel<T>(string filePath) where T : class, new()
    {
        var result = new List<T>();

        using (var workbook = new XLWorkbook(filePath))
        {
            var worksheet = workbook.Worksheet(1); // 默认读取第一个工作表

            // 获取所有列标题（第一行）
            var headerRow = worksheet.Row(1);
            var headers = new Dictionary<string, int>();

            foreach (var cell in headerRow.Cells())
            {
                if (!cell.IsEmpty()) // 先判断是否为空单元格
                {
                    string headerName = cell.Value.ToString().Trim(); // 直接调用 ToString()
                    headers[headerName] = cell.Address.ColumnNumber;
                }
            }

            // 获取实体类的所有属性
            PropertyInfo[] properties = typeof(T).GetProperties();

            foreach (var row in worksheet.RowsUsed().Skip(1)) // 跳过标题行
            {
                var obj = new T();

                foreach (var prop in properties)
                {
                    if (headers.TryGetValue(prop.Name, out int colIndex))
                    {
                        var cell = row.Cell(colIndex);

                        if (!cell.IsEmpty())
                        {
                            string cellValue = cell.Value.ToString();

                            try
                            {
                                object convertedValue = Convert.ChangeType(cellValue, prop.PropertyType);
                                prop.SetValue(obj, convertedValue);
                            }
                            catch
                            {
                                // 可记录转换失败的日志
                            }
                        }
                    }
                }

                result.Add(obj);
            }
        }

        return result;
    }
}
