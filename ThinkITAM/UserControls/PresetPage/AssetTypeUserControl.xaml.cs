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



            if (addAssetTag.ShowDialog() == true)
            {
                LoadAssetTagInfo();
            }
        }

        private void AssetTagListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }

        private void AssetTagListView_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void EditAssetButton_OnClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void DeleteAssetButton_OnClick(object sender, RoutedEventArgs e)
        {
           
        }
    }
}
