using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
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
using ThinkITAM.ChildrenWindows.PresetWindows;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.ViewModes.Preset;

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

        private DbClass dbClass;

        private void AssetTypeUserControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            string dbFilePath = AppDomain.CurrentDomain.BaseDirectory + @"db\Address_database.db";
            dbClass = new DbClass(dbFilePath);
            dbClass.OpenConnection();



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

            SQLiteCommand command = new SQLiteCommand(query, dbClass.connection);

            SQLiteDataReader reader = command.ExecuteReader();


            int index = 0;
            while (reader.Read())
            {
                index++;
                var info = new AssetTagClass();
                info.Index = index;
                info.AssetType = reader["AssetType"].ToString();
                info.DeviceType = reader["DeviceType"].ToString();
                info.NumberPrefix = reader["AssetTag"].ToString();
                info.Note = reader["Note"].ToString();

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
    }
}
