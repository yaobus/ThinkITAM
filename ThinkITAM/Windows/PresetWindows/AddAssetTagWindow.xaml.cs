using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using ThinkITAM.DatabaseOperation;
using ThinkITAM.DataBridge;
using ThinkITAM.ViewModels.Preset;

namespace ThinkITAM.Windows.PresetWindows
{
    /// <summary>
    /// AddAssetWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddAssetTagWindow : Window
    {
        public AddAssetTagWindow()
        {
            InitializeComponent();
        }




        private void AddAssetTagWindow_OnLoaded(object sender, RoutedEventArgs e)
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



            AssetType.ItemsSource = assetTypeInfos;

        }



        /// <summary>
        /// 资产类型列表
        /// </summary>
        private ObservableCollection<string> deviceTypeInfos = new ObservableCollection<string>();


        private void AssetType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AssetType.SelectedIndex != -1)
            {
                deviceTypeInfos.Clear();

                string query = $"SELECT  DeviceType FROM AssetTag WHERE AssetType='{assetTypeInfos[AssetType.SelectedIndex].ToString()}';";

                Console.WriteLine(query);


                var rows = GlobalVariables.DbService.ExecuteQuery(query);

                foreach (var row in rows)
                {
                    deviceTypeInfos.Add(row["DeviceType"].ToString());
                }



                DeviceType.ItemsSource = deviceTypeInfos;
            }
            else
            {
                deviceTypeInfos.Clear();
            }
        }


        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {

            var info = CheckInput();

            if (info.Item1 == 0)
            {

                var assetType = AssetType.Text.Replace(" ", "");
                var deviceType = DeviceType.Text.Replace(" ", "");
                var assetTag = AssetTag.Text.Replace(" ", "");

                if (assetTag.EndsWith("-") == false)
                {
                    // 如果最后一个字符不是 "-"，则添加 "-"
                    assetTag += "-";
                }


                //检查资产类型和设备类型是否已有预设
                string sqlTemp = $"SELECT COUNT(*) FROM AssetTag WHERE AssetType ='{assetType}' AND DeviceType = '{deviceType}'";


                var num = DbClass.ExecuteScalarTableNum(sqlTemp);

                if (num <= 0)
                {
                    //检查编号是否重复
                    sqlTemp = $"SELECT COUNT(*) FROM AssetTag WHERE AssetTag ='{assetTag}'";

                    num = DbClass.ExecuteScalarTableNum(sqlTemp);

                    if (num <= 0)//如果都没有
                    {

                        var assettagInfo = new
                        {
                            AssetType = assetType,
                            DeviceType = deviceType,
                            AssetTag = assetTag,
                            Note = Note.Text
                        };

                        //string sql = $"INSERT INTO  \"AssetTag\" (\"AssetType\", \"DeviceType\", \"AssetTag\", \"Note\") VALUES ('{assetType}', '{deviceType}', '{assetTag}', '{Note.Text}')";

                      
                        GlobalVariables.DbService.InsertEntity("AssetTag", assettagInfo);

                        this.DialogResult = true;
                        this.Close();

                    }
                    else
                    {
                        MessageBox.Show($"已存在编号预设{assetTag}，请勿重复设置！", "注意！", MessageBoxButton.OK, MessageBoxImage.Error);
                    }



                }
                else
                {
                    MessageBox.Show($"已存在对资产类型为{assetType}，设备类型为{deviceType}的编号预设，请勿重复设置！", "注意！", MessageBoxButton.OK, MessageBoxImage.Error);
                }



            }
            else
            {
                MessageBox.Show(info.Item2, "注意！", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private (int, string) CheckInput()
        {
            int index = 0;

            string message = "当前存在以下问题需要解决:\r";

            if (AssetType.Text.Replace(" ", "").Length < 2)
            {
                index++;

                message += index.ToString() + ":资产类型名称太短\r";
            }


            if (DeviceType.Text.Replace(" ", "").Length < 2)
            {
                index++;

                message += index.ToString() + ":设备类型名称太短\r";
            }

            if (AssetTag.Text.Replace(" ", "").Length < 3)
            {
                index++;

                message += index.ToString() + ":编号前缀太短\r";
            }


            return (index, message);
        }

    }
}
