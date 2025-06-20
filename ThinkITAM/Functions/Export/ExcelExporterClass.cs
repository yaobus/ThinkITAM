using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ClosedXML.Excel;
using LicenseContext = System.ComponentModel.LicenseContext;

namespace ThinkITAM.Functions.Export
{


    #region MyRegion




    //public static class ExcelExporter
    //{
    //    /// <summary>
    //    /// 将任意类型的集合导出为Excel文件，并指定保存路径
    //    /// </summary>
    //    /// <typeparam name="T">集合元素类型</typeparam>
    //    /// <param name="collection">数据集合</param>
    //    /// <param name="filePath">要保存的文件路径</param>
    //    public static void ExportToExcel<T>(IEnumerable<T> collection, string filePath)
    //    {
    //        if (collection == null || !collection.Any())
    //        {
    //            MessageBox.Show("没有可导出的数据。");
    //            return;
    //        }

    //        try
    //        {
    //            // 在创建 ExcelPackage 前设置许可证
    //            ExcelPackage.License.SetNonCommercialPersonal("yaobus");

    //            using (var package = new ExcelPackage())
    //            {
    //                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

    //                // 获取类型的所有公共属性
    //                PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

    //                // 写入表头
    //                for (int i = 0; i < properties.Length; i++)
    //                {
    //                    worksheet.Cells[1, i + 1].Value = properties[i].Name;
    //                }

    //                // 写入数据
    //                int row = 2;
    //                foreach (var item in collection)
    //                {
    //                    for (int col = 0; col < properties.Length; col++)
    //                    {
    //                        var value = properties[col].GetValue(item);
    //                        worksheet.Cells[row, col + 1].Value = value?.ToString() ?? string.Empty;
    //                    }
    //                    row++;
    //                }

    //                // 自动调整列宽
    //                worksheet.Cells.AutoFitColumns();

    //                // 保存文件
    //                package.SaveAs(new FileInfo(filePath));

    //                MessageBox.Show($"数据已成功导出至：{filePath}");
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            MessageBox.Show($"导出失败：{ex.Message}");
    //        }
    //    }
    //} 

    #endregion

    public static class ExcelExporter
    {
        public static void ExportToExcel<T>(IEnumerable<T> collection, string filePath)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");

                // 获取属性
                var properties = typeof(T).GetProperties();
                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cell(1, i + 1).Value = properties[i].Name;
                }

                // 写入数据
                int row = 2;
                foreach (var item in collection)
                {
                    for (int col = 0; col < properties.Length; col++)
                    {
                        var value = properties[col].GetValue(item);
                        worksheet.Cell(row, col + 1).Value = value?.ToString() ?? "";
                    }

                    row++;
                }

                // 自动调整列宽
                worksheet.Columns().AdjustToContents();

                // 保存文件
                workbook.SaveAs(filePath);
                MessageBox.Show($"数据已成功导出至：{filePath}","导出完毕",MessageBoxButton.OK,MessageBoxImage.Information);
            }
        }

    }
}
