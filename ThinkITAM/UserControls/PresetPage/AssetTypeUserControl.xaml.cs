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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ThinkITAM.Windows.PresetWindows;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.ViewModels.Preset;
using ThinkITAM.DataBridge;

namespace ThinkITAM.UserControls.PresetPage
{
    /// <summary>
    /// AssetTypeUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class AssetTypeUserControl : UserControl
    {
        public AssetTypeUserControl()
        {
            InitializeComponent();
        }



        private void AssetTypeUserControl_OnLoaded(object sender, RoutedEventArgs e)
        {




            LoadAssetTagInfo();

            AssetTagListView.ItemsSource = assetTags;
        }


        /// <summary>
        /// 资产信息标签预设列表
        /// </summary>
        private ObservableCollection<AssetTagClass> assetTags = new ObservableCollection<AssetTagClass>();



        /// <summary>
        /// 加载资产标签预设信息
        /// </summary>
        private void LoadAssetTagInfo()
        {
            assetTags.Clear();

            string query = "SELECT * FROM AssetTag;";

            var rows = GlobalVariables.DbService.ExecuteQuery(query);



            int index = 0;

            foreach (var row in rows)
            {
                index++;
                var info = new AssetTagClass();
                info.Index = index;
                info.AssetType = row["AssetType"].ToString();
                info.DeviceType = row["DeviceType"].ToString();
                info.NumberPrefix = row["AssetTag"].ToString();
                info.Note = row["Note"].ToString();

                assetTags.Add(info);
            }

        }

        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {
            AddAssetTagWindow addAssetTag = new AddAssetTagWindow();

            //窗口放中间
            var window = Window.GetWindow(this);
            if (window != null)
            {
                addAssetTag.Owner = window;
            }

            if (addAssetTag.ShowDialog() == true)
            {
                LoadAssetTagInfo();
            }
        }

        private void AssetTagListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            var index = AssetTagListView.SelectedIndex;

            if (index != -1)
            {
                EditAssetButton.IsEnabled = true;
                DeleteAssetButton.IsEnabled = true;
            }
            else
            {
                EditAssetButton.IsEnabled = false;
                DeleteAssetButton.IsEnabled = false;
            }



        }

        private void AssetTagListView_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var index = AssetTagListView.SelectedIndex;

            if (index != -1)
            {

                var info = assetTags[index];

                AddAssetTagWindow addAssetTag = new AddAssetTagWindow(info);



                if (addAssetTag.ShowDialog() == true)
                {
                    LoadAssetTagInfo();
                }


            }

        }

        private void EditAssetButton_OnClick(object sender, RoutedEventArgs e)
        {
            var index = AssetTagListView.SelectedIndex;

            if (index != -1)
            {

                var info = assetTags[index];

                AddAssetTagWindow addAssetTag = new AddAssetTagWindow(info);



                if (addAssetTag.ShowDialog() == true)
                {
                    LoadAssetTagInfo();
                }


            }
        }

        private void DeleteAssetButton_OnClick(object sender, RoutedEventArgs e)
        {
            var index = AssetTagListView.SelectedIndex;

            if (index != -1)
            {

                var info = assetTags[index];

                var message = $"确定要删除吗？\r资产类型:{info.AssetType}\r设备类型:{info.DeviceType}\r编号前缀:{info.NumberPrefix}\r该操作不可逆！";


                var result = MessageBox.Show(message, "警告", MessageBoxButton.YesNo, MessageBoxImage.Question);


                if (result == MessageBoxResult.Yes)
                {
                    string query = $"DELETE FROM  AssetTag WHERE AssetTag='{info.NumberPrefix}'";


                    GlobalVariables.DbService.ExecuteNonQuery(query);

                    LoadAssetTagInfo();

                }


            }
        }
    }
}
