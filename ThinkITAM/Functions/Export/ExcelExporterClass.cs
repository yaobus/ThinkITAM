using System.Windows;
using ClosedXML.Excel;
using Newtonsoft.Json;
using ThinkITAM.ViewModels.DevicePortManage;
using ThinkITAM.ViewModels.NetworkManage;
using ThinkITAM.ViewModels.Others;


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
                MessageBox.Show($"数据已成功导出至：{filePath}", "导出完毕", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        /// <summary>
        /// 将集合导出为 Excel 文件，使用自定义表头（来自配置）
        /// </summary>
        /// <typeparam name="T">数据模型类型</typeparam>
        /// <param name="collection">数据集合</param>
        /// <param name="filePath">保存路径</param>
        /// <param name="expInfo">包含 NetworkId 用于获取表头配置</param>
        public static void ExportToExcel<T>(IEnumerable<T> collection, string filePath, ExportNetworkInfoClass expInfo)
        {
            // 获取自定义表头：Dictionary<PropertyName, DisplayName>
            var headerNames = GetNetworkAddressHeader(expInfo);

            // 如果没有配置，使用属性名作为表头
            if (headerNames == null || headerNames.Count == 0)
            {
                headerNames = typeof(T)
                    .GetProperties()
                    .Where(p => p.CanRead)
                    .ToDictionary(p => p.Name, p => p.Name);
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");

                // 获取 T 中所有可读属性，并按 headerNames 的 Key 过滤和排序
                var properties = typeof(T)
                    .GetProperties()
                    .Where(p => p.CanRead && headerNames.ContainsKey(p.Name))
                    .ToArray();

                // 写入表头（只写 headerNames 中定义的列）
                for (int i = 0; i < properties.Length; i++)
                {
                    string propertyName = properties[i].Name;
                    if (headerNames.TryGetValue(propertyName, out string displayName))
                    {
                        worksheet.Cell(1, i + 1).Value = displayName;
                    }
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
                MessageBox.Show($"数据已成功导出至：{filePath}", "导出完毕", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }



        /// <summary>
        /// 将集合导出为 Excel 文件，使用自定义表头（来自配置）
        /// </summary>
        /// <typeparam name="T">数据模型类型</typeparam>
        /// <param name="collection">数据集合</param>
        /// <param name="filePath">保存路径</param>
        /// <param name="expInfo">包含 NetworkId 用于获取表头配置</param>
        public static void ExportToExcel<T>(IEnumerable<T> collection, string filePath, CommonExportClass expInfo)
        {
            // 获取自定义表头：Dictionary<PropertyName, DisplayName>
            var headerNames = GetHeader(expInfo);



            // 如果没有配置，使用属性名作为表头
            if (headerNames == null || headerNames.Count == 0)
            {
                headerNames = typeof(T)
                    .GetProperties()
                    .Where(p => p.CanRead)
                    .ToDictionary(p => p.Name, p => p.Name);
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");

                // 获取 T 中所有可读属性，并按 headerNames 的 Key 过滤和排序
                var properties = typeof(T)
                    .GetProperties()
                    .Where(p => p.CanRead && headerNames.ContainsKey(p.Name))
                    .ToArray();

                // 写入表头（只写 headerNames 中定义的列）
                for (int i = 0; i < properties.Length; i++)
                {
                    string propertyName = properties[i].Name;
                    if (headerNames.TryGetValue(propertyName, out string displayName))
                    {
                        worksheet.Cell(1, i + 1).Value = displayName;
                    }
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
                MessageBox.Show($"数据已成功导出至：{filePath}", "导出完毕", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }



        /// <summary>
        /// 获取要导出的数据列
        /// </summary>
        /// <returns></returns>
        private static Dictionary<string, string> GetNetworkAddressHeader(ExportNetworkInfoClass expInfo)
        {
            var headers = new Dictionary<string, string>
            {
                {"Address","IP号"},
                {"FullAddress","IP地址"},
                {"AddressStatus","地址状态"},
                {"PingTime","Ping延时"},
                {"Name","用户名"},
                {"Organization","组织"},
                {"Department","部门"},
                {"Group","群组"},
                {"Unit","单元"},
                {"Phone","电话"},
                {"HostName","主机名"},
                {"MacAddress","Mac地址"},
                {"TagA","TagA"},
                {"TagB","TagB"},
                {"TagC","TagC"},
                {"TagD","TagD"},
                {"TagE","TagE"},
                {"TagF","TagF"},
            };

            if (expInfo.WindowTags != null)
            {
                var tags = expInfo.WindowTags;

                headers["TagA"] = tags.TagA;
                headers["TagB"] = tags.TagB;
                headers["TagC"] = tags.TagC;
                headers["TagD"] = tags.TagD;
                headers["TagE"] = tags.TagE;
                headers["TagF"] = tags.TagF;
            }

            return headers;
        }

        /// <summary>
        /// 获取要导出的数据列
        /// </summary>
        /// <returns></returns>
        private static Dictionary<string, string> GetHeader(CommonExportClass expInfo)
        {
            var headers = new Dictionary<string, string>();

            switch (expInfo.Type)
            {
                case 0://网段信息
                    headers = new Dictionary<string, string>
                    {
                        {"Address","IP地址"},
                        {"AddressStatus","地址状态"},
                        {"Name","用户名"},
                        {"Organization","组织"},
                        {"Department","部门"},
                        {"Group","群组"},
                        {"Unit","单元"},
                        {"Phone","电话"},
                        {"HostName","主机名"},
                        {"MacAddress","Mac地址"},
                        {"TagA","TagA"},
                        {"TagB","TagB"},
                        {"TagC","TagC"},
                        {"TagD","TagD"},
                        {"TagE","TagE"},
                        {"TagF","TagF"},
                    };


                    if (expInfo.WindowTags != null)
                    {
                        var tags = expInfo.WindowTags;

                        headers["TagA"] = tags.TagA;
                        headers["TagB"] = tags.TagB;
                        headers["TagC"] = tags.TagC;
                        headers["TagD"] = tags.TagD;
                        headers["TagE"] = tags.TagE;
                        headers["TagF"] = tags.TagF;
                    }


                    break;

                case 1://设备端口信息

                    headers = new Dictionary<string, string>
                    {
                        {"PortType","端口类型"},
                        {"PortTag","端口标签"},
                        {"PortSpeed","端口速度"},
                        {"FullPortId","端口号"},
                        {"Status","状态"},
                        {"Mode","模式"},
                        {"PortName","端口名称"},
                        {"VlanId","VlanID"},
                        {"DeviceLinkId","关联设备"},
                        {"OnTheLine","关联链路"},

                        {"TagA","TagA"},
                        {"TagB","TagB"},
                        {"TagC","TagC"},
                        {"TagD","TagD"},
                        {"TagE","TagE"},
                        {"TagF","TagF"},
                    };


                    if (expInfo.WindowTags != null)
                    {
                        string ts = expInfo.WindowTags;

                        var tags = JsonConvert.DeserializeObject<TagViewModel>(ts);

                        if (tags != null)
                        {
                            headers["TagA"] = tags.TagA;
                            headers["TagB"] = tags.TagB;
                            headers["TagC"] = tags.TagC;
                            headers["TagD"] = tags.TagD;
                            headers["TagE"] = tags.TagE;
                            headers["TagF"] = tags.TagF;
                        }

                    }


                    break;
                case 2://资产信息

                    headers = new Dictionary<string, string>
                    {
                        {"AssetType","资产类型"},
                        {"DeviceType","设备类型"},
                        {"AssetNumber","资产编号"},
                        {"PurchaseDate","购置日期"},
                        {"PurchasePrice","购置价格"},
                        {"Manufacturer","制造商"},
                        {"Model","型号"},
                        {"SerialNumber","序列号"},
                        {"Configuration","配置"},
                        {"Location","地址"},
                        {"UserOrganization","组织"},
                        {"UserDepartment","部门"},
                        {"Unit","单元"},
                        {"User","责任人"},
                        {"UserPhone","责任人电话"},
                        {"Consumer","使用人"},
                        {"AssetStatus","资产状态"},
                        {"UsedYear","已用年限"},
                        {"ScrapDate","报废日期"},
                        {"Notes","备注"},


                        {"TagA","TagA"},
                        {"TagB","TagB"},
                        {"TagC","TagC"},
                        {"TagD","TagD"},
                        {"TagE","TagE"},
                        {"TagF","TagF"},
                    };


                    if (expInfo.WindowTags != null)
                    {
                        var tags = expInfo.WindowTags;

                        headers["TagA"] = tags.TagA;
                        headers["TagB"] = tags.TagB;
                        headers["TagC"] = tags.TagC;
                        headers["TagD"] = tags.TagD;
                        headers["TagE"] = tags.TagE;
                        headers["TagF"] = tags.TagF;
                    }



                    break;
                case 3://资产日志

                    headers = new Dictionary<string, string>
                    {
                        {"Index","序号"},
                        {"EventDate","事件日期"},
                        {"EventContent","事件内容"},
                        {"Name","相关用户"},
                        {"Note","备注信息"},



                        {"TagA","TagA"},
                        {"TagB","TagB"},
                        {"TagC","TagC"},
                        {"TagD","TagD"},
                        {"TagE","TagE"},
                        {"TagF","TagF"}
                    };


                    if (expInfo.WindowTags != null)
                    {
                        var tags = expInfo.WindowTags;

                        headers["TagA"] = tags.TagA;
                        headers["TagB"] = tags.TagB;
                        headers["TagC"] = tags.TagC;
                        headers["TagD"] = tags.TagD;
                        headers["TagE"] = tags.TagE;
                        headers["TagF"] = tags.TagF;
                    }



                    break;
            }




            return headers;
        }

    }
}
