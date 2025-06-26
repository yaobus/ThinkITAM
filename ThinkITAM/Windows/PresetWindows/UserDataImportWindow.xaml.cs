using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.Win32;
using Nodify;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.DataBridge;
using ThinkITAM.Functions.FunctionClass;
using ThinkITAM.Functions.Import;
using ThinkITAM.ViewModels.AssetManage;
using ThinkITAM.ViewModels.Preset;
using Path = System.IO.Path;

namespace ThinkITAM.Windows.PresetWindows;

/// <summary>
/// AssetDataImportWindow.xaml 的交互逻辑
/// </summary>
public partial class UserDataImportWindow : Window
{
    public UserDataImportWindow()
    {
        InitializeComponent();
    }

    private void AssetDataImportWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        LoadAssetType();
    }

    /// <summary>
    /// 资产类型列表
    /// </summary>
    private ObservableCollection<string> assetTypeInfos = new ObservableCollection<string>();

    private void LoadAssetType()
    {

        assetTypeInfos.Clear();

        string query = "SELECT DISTINCT AssetType FROM AssetTag;";

        var rows = GlobalVariables.DbService.ExecuteQuery(query);


        foreach (var row in rows)
        {
            assetTypeInfos.Add(row["AssetType"].ToString());
        }







    }



    private void BrowseButton_OnClick(object sender, RoutedEventArgs e)
    {

        var title = (string)FindResource("AawOpenFileTitle");
        var filter = (string)FindResource("AawOpenFileFilter");

        // ToggleButton 未选中，选择 .db 文件
        OpenFileDialog openFileDialog = new OpenFileDialog();

        openFileDialog.Filter = $"{filter}";
        openFileDialog.Title = $"{title}";

        if (openFileDialog.ShowDialog() == true)
        {
            ExcelFilePath.Text = openFileDialog.FileName;
        }




    }

    private void TemplateGetButton_OnClick(object sender, RoutedEventArgs e)
    {
        // 1. 弹出保存对话框让用户选择路径
        SaveFileDialog saveFileDialog = new SaveFileDialog();
        saveFileDialog.Filter = "Excel 文件 (*.xlsx)|*.xlsx";
        saveFileDialog.FileName = "UserInfoTemplate.xlsx";

        if (saveFileDialog.ShowDialog() == true)
        {
            string destinationPath = saveFileDialog.FileName;

            // 2. 获取嵌入资源或者本地文件内容
            string sourceFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "Resources\\Template\\UserInfoTemplate.xlsx");

            try
            {
                // 3. 复制文件到目标位置
                File.Copy(sourceFilePath, destinationPath, overwrite: true);

                // 4. 打开资源管理器并定位到该文件夹
                Process.Start("explorer.exe", $"/select,\"{destinationPath}\"");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"无法导出数据模板：{ex.Message}");
            }
        }
    }


    /// <summary>
    /// 资产类型列表
    /// </summary>
    private ObservableCollection<string> deviceTypeInfos = new ObservableCollection<string>();


    /// <summary>
    /// 点击导入按钮
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ImportButton_OnClick(object sender, RoutedEventArgs e)
    {

        if (!string.IsNullOrWhiteSpace(ExcelFilePath.Text))
        {
            if (ExcelImporter.IsFileLocked(ExcelFilePath.Text))
            {
                MessageBox.Show("该文件正被其他程序使用，请关闭后再尝试导入。", "文件被占用", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }


            var importDatas = ExcelImporter.ImportFromExcel<PeopleImportViewModel>(ExcelFilePath.Text);

            //设置进度条最大值
            Dispatcher.Invoke(() =>
            {
                ImportProgressBar.Maximum = importDatas.Count;
                ImportProgressBar.Value = 0;
            });

            int index = 0;
            int successCount = 0;
            foreach (var data in importDatas)
            {

                index++;

                var number = DbClass.GetNextAvailableNumber("UserInfo", "Number");

                string userId =
                    $"9{AssetCodeClass.GenerateChecksum(AssetIdCreate.CreateAssetId(Guid.NewGuid().ToString())).ToUpper()}";

                data.UserId = userId;
                data.Number = number;

                //保存组织机构信息


                if (SaveOrganizationInfo(data.Organization, data.Department, data.UserGroup, data.UserUnit) == true)
                {
                    GlobalVariables.DbService.InsertEntity("UserInfo", data);

                    successCount++;

                }
                await UpdateProgressBarAsync(index);


            }

            MessageBox.Show($"尝试人员信息{index}条，\r成功导入{successCount}条\r失败{index - successCount}条\r失败原因：组织机构信息不满足顺序依赖验证", "导入完毕", MessageBoxButton.OK, MessageBoxImage.Information);

            DialogResult = true;

        }
        else
        {
            MessageBox.Show("请选择需要导入的数据", "信息不完整", MessageBoxButton.OK, MessageBoxImage.Information);
        }



    }

    /// <summary>
    /// 更新极度条
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private async Task UpdateProgressBarAsync(int value)
    {

        // 同样使用Dispatcher更新进度值
        Dispatcher.Invoke(() =>
        {
            ImportProgressBar.Value = value;
        });

    }




    /// <summary>
    /// 保存组织信息,如果保存成功或者已存在，则返回true，否则返回false
    /// </summary>
    /// <param name="organization"></param>
    /// <param name="department"></param>
    /// <param name="groups"></param>
    /// <param name="unit"></param>
    /// <returns></returns>
    private bool SaveOrganizationInfo(string organization, string department = null, string groups = null, string unit = null)
    {


        if (ValidateInput(organization, department, groups, unit) == true)
        {

            var organizationInfo = organization;
            var departmentInfo = department;
            var groupsInfo = groups;
            var unitsInfo = unit;

            //判断一级组织是否单独存在，不存在则创建
            var query = $"SELECT COUNT( Organization) FROM Organization WHERE Organization='{organizationInfo}' AND (Department IS NULL OR Department ='')";

            var count = Convert.ToInt32(GlobalVariables.DbService.ExecuteScalar(query));

            if (count == 0)//不存在
            {
                var info = new
                {
                    Organization = organizationInfo
                };

                GlobalVariables.DbService.InsertEntity("Organization", info);
            }
            else
            {
                //存在，但是已删除
                query = $"SELECT COUNT( Organization) FROM Organization WHERE Organization='{organizationInfo}' AND (Department IS NULL OR Department ='') AND Del = 1";

                count = Convert.ToInt32(GlobalVariables.DbService.ExecuteScalar(query));

                if (count == 1)
                {
                    query = $"UPDATE Organization SET Del = NULL WHERE  Organization='{organizationInfo}' AND (Department IS NULL OR Department ='') ";
                    GlobalVariables.DbService.ExecuteNonQuery(query);

                }
            }


            //判断二级组织是否单独存在，不存在则创建
            if (!string.IsNullOrWhiteSpace(departmentInfo))
            {
                query = $"SELECT COUNT(Department) FROM Organization WHERE Organization='{organizationInfo}' AND Department='{departmentInfo}' AND (Groups IS NULL OR Groups ='')";

                count = Convert.ToInt32(GlobalVariables.DbService.ExecuteScalar(query));

                if (count == 0)//不存在
                {
                    var info = new
                    {
                        Organization = organizationInfo,
                        Department = departmentInfo
                    };

                    GlobalVariables.DbService.InsertEntity("Organization", info);
                }
                else
                {
                    //存在，但是已删除
                    query = $"SELECT COUNT( Department) FROM Organization WHERE Organization='{organizationInfo}'  AND Department='{departmentInfo}' AND (Groups IS NULL OR Groups ='') AND Del = 1";

                    count = Convert.ToInt32(GlobalVariables.DbService.ExecuteScalar(query));

                    if (count == 1)
                    {
                        query = $"UPDATE Organization SET Del = NULL WHERE  Organization='{organizationInfo}' AND Department='{departmentInfo}' AND (Groups IS NULL OR Groups ='') ";
                        GlobalVariables.DbService.ExecuteNonQuery(query);

                    }
                }

            }


            //判断三级组织是否单独存在，不存在则创建
            if (!string.IsNullOrWhiteSpace(groupsInfo))
            {
                query = $"SELECT COUNT(Groups) FROM Organization WHERE Organization='{organizationInfo}' AND Department='{departmentInfo}'  AND Groups='{groupsInfo}' AND (UserUnit IS NULL OR UserUnit ='')";

                count = Convert.ToInt32(GlobalVariables.DbService.ExecuteScalar(query));

                if (count == 0)//不存在
                {
                    var info = new
                    {
                        Organization = organizationInfo,
                        Department = departmentInfo,
                        Groups = groupsInfo
                    };

                    GlobalVariables.DbService.InsertEntity("Organization", info);
                }
                else
                {
                    //存在，但是已删除
                    query = $"SELECT COUNT( Groups) FROM Organization WHERE Organization='{organizationInfo}'  AND Department='{departmentInfo}' AND Groups='{groupsInfo}' AND (UserUnit IS NULL OR UserUnit ='') AND Del = 1";

                    count = Convert.ToInt32(GlobalVariables.DbService.ExecuteScalar(query));

                    if (count == 1)
                    {
                        query = $"UPDATE Organization SET Del = NULL WHERE  Organization='{organizationInfo}' AND Department='{departmentInfo}' AND Groups='{groupsInfo}' AND (UserUnit IS NULL OR UserUnit ='') ";
                        GlobalVariables.DbService.ExecuteNonQuery(query);

                    }
                }

            }


            string sqlTemp =
                $"SELECT COUNT(*) FROM Organization WHERE Organization ='{organizationInfo}' AND Department = '{departmentInfo}' AND Groups ='{groupsInfo}' AND UserUnit ='{unitsInfo}'";

            var num = DbClass.ExecuteScalarTableNum(sqlTemp);

            if (num == 0)
            {
                var info = new
                {
                    Organization = organizationInfo,
                    Department = departmentInfo,
                    Groups = groupsInfo,
                    UserUnit = unitsInfo
                };


                GlobalVariables.DbService.InsertEntity("Organization", info);

            }
            else
            {
                //存在，但是已删除
                query = $"SELECT COUNT(UserUnit) FROM Organization WHERE Organization='{organizationInfo}'  AND Department='{departmentInfo}' AND Groups='{groupsInfo}' AND UserUnit ='{unitsInfo}' AND Del = 1";

                count = Convert.ToInt32(GlobalVariables.DbService.ExecuteScalar(query));

                if (count == 1)
                {
                    query = $"UPDATE Organization SET Del = NULL WHERE  Organization='{organizationInfo}' AND Department='{departmentInfo}' AND Groups='{groupsInfo}' AND UserUnit ='{unitsInfo}' ";
                    GlobalVariables.DbService.ExecuteNonQuery(query);

                }

            }

            return true;

        }
        else
        {

            return false;
        }


    }










    /// <summary>
    /// 输入检测
    /// </summary>
    /// <param name="organization"></param>
    /// <param name="department"></param>
    /// <param name="groups"></param>
    /// <param name="unit"></param>
    /// <returns></returns>
    public bool ValidateInput(string organization, string department, string groups, string unit)
    {
        // 去除空格并赋值
        var organizationInfo = organization?.Replace(" ", "");
        var departmentInfo = department?.Replace(" ", "");
        var groupsInfo = groups?.Replace(" ", "");
        var unitsInfo = unit?.Replace(" ", "");

        // 检查顺序是否正确：如果后面的字段有值，则前面的字段必须有值

        // 如果 unitsInfo 有值，则 groupsInfo 必须有值
        if (!string.IsNullOrEmpty(unitsInfo) && string.IsNullOrEmpty(groupsInfo))
            return false;

        // 如果 groupsInfo 有值，则 departmentInfo 必须有值
        if (!string.IsNullOrEmpty(groupsInfo) && string.IsNullOrEmpty(departmentInfo))
            return false;

        // 如果 departmentInfo 有值，则 organizationInfo 必须有值
        if (!string.IsNullOrEmpty(departmentInfo) && string.IsNullOrEmpty(organizationInfo))
            return false;

        // 如果 organizationInfo 没有值，但其他字段有值，则不符合规则
        if (string.IsNullOrEmpty(organizationInfo) &&
            (!string.IsNullOrEmpty(departmentInfo) || !string.IsNullOrEmpty(groupsInfo) || !string.IsNullOrEmpty(unitsInfo)))
            return false;

        // 所有条件都满足，返回 true
        return true;
    }
}
