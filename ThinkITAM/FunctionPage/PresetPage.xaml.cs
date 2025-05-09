using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using ThinkITAM.ChildrenWindows.NetworkManage;
using ThinkITAM.ChildrenWindows.PresetWindows;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.IPAddressCalculations;
using ThinkITAM.UserControls.NetworkManage;
using ThinkITAM.UserControls.PresetPage;
using ThinkITAM.ViewModes.NetworkManage;
using ThinkITAM.ViewModes.Preset;

namespace ThinkITAM.FunctionPage
{
    /// <summary>
    /// PresetPage.xaml 的交互逻辑
    /// </summary>
    public partial class PresetPage : UserControl
    {
        public PresetPage()
        {
            InitializeComponent();
        }

       private DbClass dbClass;

        private void PresetPage_OnLoaded(object sender, RoutedEventArgs e)
        {

            string dbFilePath = AppDomain.CurrentDomain.BaseDirectory + @"db\Address_database.db";
            dbClass = new DbClass(dbFilePath);
            dbClass.OpenConnection();


            //初始化时打开第一个标签页
            PresetTreeView.SelectedIndex = 0;



        }







        private ObservableCollection<BrowserInfoViewModel> browserInfos = new ObservableCollection<BrowserInfoViewModel>();

        private void LoadBrowserInfo()
        {

            browserInfos.Clear();

            string query = "SELECT * FROM Browser;";

            SQLiteCommand command = new SQLiteCommand(query, dbClass.connection);
            SQLiteDataReader reader = command.ExecuteReader();

            int index = 0;

            while (reader.Read())
            {
                index++;
                BrowserInfoViewModel info = new BrowserInfoViewModel();

                info.Index = index;
                info.Browser = reader["Browser"].ToString();
                info.Path = reader["Path"].ToString();

                browserInfos.Add(info);
            }

            //BrowserListView.ItemsSource = browserInfos;


        }

        private ObservableCollection<AddressInfoViewModel> addressInfos = new ObservableCollection<AddressInfoViewModel>();


        private void LoadAddress()
        {
            addressInfos.Clear();

            string query = "SELECT * FROM Address;";

            SQLiteCommand command = new SQLiteCommand(query, dbClass.connection);
            SQLiteDataReader reader = command.ExecuteReader();

            int index = 0;

            while (reader.Read())
            {
                index++;
                AddressInfoViewModel info = new AddressInfoViewModel();

                info.Index = index;
               info.Location = reader["Location"].ToString();
                info.Note = reader["Note"].ToString();

                addressInfos.Add(info);
            }

            //AddressListView.ItemsSource = addressInfos;


        }






        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {
            AddOrganizationWindow addOrganizationWindow = new AddOrganizationWindow();

            
            if (addOrganizationWindow.ShowDialog() == true)
            {

                LoadOrganization();

            }
        }


        private ObservableCollection<OrganizationViewModel> organizationInfos = new ObservableCollection<OrganizationViewModel>();


        private void LoadOrganization()
        {
            organizationInfos.Clear();

            string query = "SELECT * FROM Organization;";

            SQLiteCommand command = new SQLiteCommand(query, dbClass.connection);
            SQLiteDataReader reader = command.ExecuteReader();

            int index = 0;

            while (reader.Read())
            {
                index++;
                OrganizationViewModel info = new OrganizationViewModel();

                info.Index = index;
                info.Organization = reader["Organization"].ToString();
                info.Department = reader["Department"].ToString();
                info.Note = reader["Note"].ToString();

                organizationInfos.Add(info);
            }

            //OrganizationListView.ItemsSource = organizationInfos;


        }




        private void AddPeopleButton_OnClick(object sender, RoutedEventArgs e)
        {
            AddPeopleWindow addPeople = new AddPeopleWindow();


            if (addPeople.ShowDialog() == true)
            {

                LoadPeopleInfos();

            }
        }

        private ObservableCollection<PeopleViewModel> peopleInfos = new ObservableCollection<PeopleViewModel>();


        /// <summary>
        /// 加载人员信息
        /// </summary>
        private void LoadPeopleInfos()
        {
            peopleInfos.Clear();

            string query = "SELECT * FROM UserInfo;";

            SQLiteCommand command = new SQLiteCommand(query, dbClass.connection);
            SQLiteDataReader reader = command.ExecuteReader();

            

            int index = 0;

            while (reader.Read())
            {
                index++;
                PeopleViewModel info = new PeopleViewModel();
                info.Index = index;
                info.Name = reader["Name"].ToString();
                info.Organization = reader["Organization"].ToString();
                info.Department = reader["Department"].ToString();
                info.Phone = reader["Phone"].ToString();
                info.Note = reader["Note"].ToString();

                peopleInfos.Add(info);
            }

            //PeopleListView.ItemsSource = peopleInfos;


        }


        private void AddAssetTag_OnClick(object sender, RoutedEventArgs e)
        {
            AddAssetTagWindow addAssetTag = new AddAssetTagWindow();

            if (addAssetTag.ShowDialog()==true)
            {
                LoadAssetTagInfo();
            }

            
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
                info.Index=index;
                info.AssetType = reader["AssetType"].ToString();
                info.DeviceType = reader["DeviceType"].ToString();
                info.NumberPrefix = reader["AssetTag"].ToString();
                info.Note = reader["Note"].ToString();

                assetTags.Add(info);
            }

            //AssetTagListView.ItemsSource = assetTags;

        }



        private void AddAddressButton_OnClick(object sender, RoutedEventArgs e)
        {
            AddAddressWindow add= new AddAddressWindow();


            if (add.ShowDialog() == true)
            {

                LoadAddress();

            }
        }

        private void AddBrowserButton_OnClick(object sender, RoutedEventArgs e)
        {
            AddBrowserWindow addBrowser = new AddBrowserWindow();

            if (addBrowser.ShowDialog() == true)
            {
                LoadBrowserInfo();
            }
        }


        /// <summary>
        /// 预设标签选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PresetTreeView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = PresetTreeView.SelectedIndex;

            if (index != -1)
            {
                switch (index)
                {
                    case 0:

                        PresetSubPanel.Children.Clear();
                        OrganizationUserControl organization = new OrganizationUserControl();


                        // 设置用户控件的尺寸与 StackPanel 一致
                        organization.HorizontalAlignment = HorizontalAlignment.Stretch;
                        organization.VerticalAlignment = VerticalAlignment.Stretch;
                        organization.Width = PresetSubPanel.ActualWidth; // 绑定宽度
                        organization.Height = PresetSubPanel.ActualHeight; // 绑定高度



                        PresetSubPanel.Children.Add(organization);

                        break;

                    case 1:

                        PresetSubPanel.Children.Clear();
                        PeopleUserControl peopleUserControl = new PeopleUserControl();

                        // 设置用户控件的尺寸与 StackPanel 一致
                        peopleUserControl .HorizontalAlignment = HorizontalAlignment.Stretch;
                        peopleUserControl .VerticalAlignment = VerticalAlignment.Stretch;
                        peopleUserControl .Width = PresetSubPanel.ActualWidth; // 绑定宽度
                        peopleUserControl .Height = PresetSubPanel.ActualHeight; // 绑定高度
                        
                        PresetSubPanel.Children.Add(peopleUserControl );
                        
                        break;

                    case 2:

                        PresetSubPanel.Children.Clear();
                        AddressUserControl addressUserControl = new AddressUserControl();

                        // 设置用户控件的尺寸与 StackPanel 一致
                        addressUserControl.HorizontalAlignment = HorizontalAlignment.Stretch;
                        addressUserControl.VerticalAlignment = VerticalAlignment.Stretch;
                        addressUserControl.Width = PresetSubPanel.ActualWidth; // 绑定宽度
                        addressUserControl.Height = PresetSubPanel.ActualHeight; // 绑定高度

                        PresetSubPanel.Children.Add(addressUserControl);

                        break;


                    case 3:


                        PresetSubPanel.Children.Clear();
                        CabinetUserControl cabinetUserControl = new CabinetUserControl();

                        // 设置用户控件的尺寸与 StackPanel 一致
                        cabinetUserControl.HorizontalAlignment = HorizontalAlignment.Stretch;
                        cabinetUserControl.VerticalAlignment = VerticalAlignment.Stretch;
                        cabinetUserControl.Width = PresetSubPanel.ActualWidth; // 绑定宽度
                        cabinetUserControl.Height = PresetSubPanel.ActualHeight; // 绑定高度

                        PresetSubPanel.Children.Add(cabinetUserControl);

                        break;

                    case 4:

                        PresetSubPanel.Children.Clear();

                        BuildingsUserControl buildingsUserControl = new BuildingsUserControl();

                        // 设置用户控件的尺寸与 StackPanel 一致
                        buildingsUserControl.HorizontalAlignment = HorizontalAlignment.Stretch;
                        buildingsUserControl.VerticalAlignment = VerticalAlignment.Stretch;
                        buildingsUserControl.Width = PresetSubPanel.ActualWidth; // 绑定宽度
                        buildingsUserControl.Height = PresetSubPanel.ActualHeight; // 绑定高度

                        PresetSubPanel.Children.Add(buildingsUserControl);

                        break;

                    case 5:

                        PresetSubPanel.Children.Clear();

                        AssetTypeUserControl assetTypeUserControl = new AssetTypeUserControl();

                        // 设置用户控件的尺寸与 StackPanel 一致
                        assetTypeUserControl.HorizontalAlignment = HorizontalAlignment.Stretch;
                        assetTypeUserControl.VerticalAlignment = VerticalAlignment.Stretch;
                        assetTypeUserControl.Width = PresetSubPanel.ActualWidth; // 绑定宽度
                        assetTypeUserControl.Height = PresetSubPanel.ActualHeight; // 绑定高度

                        PresetSubPanel.Children.Add(assetTypeUserControl);




                        break;

                    case 6:

                        PresetSubPanel.Children.Clear();

                        BrowserUserControl browserUserControl = new BrowserUserControl();

                        // 设置用户控件的尺寸与 StackPanel 一致
                        browserUserControl.HorizontalAlignment = HorizontalAlignment.Stretch;
                        browserUserControl.VerticalAlignment = VerticalAlignment.Stretch;
                        browserUserControl.Width = PresetSubPanel.ActualWidth; // 绑定宽度
                        browserUserControl.Height = PresetSubPanel.ActualHeight; // 绑定高度

                        PresetSubPanel.Children.Add(browserUserControl);



                        break;
                }



            }
        }

        /// <summary>
        /// 自动调整控件尺寸
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PresetSubPanel_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (UIElement child in PresetSubPanel.Children)
            {
                if (child is FrameworkElement frameworkElement)
                {
                    frameworkElement.Width = e.NewSize.Width;
                    frameworkElement.Height = e.NewSize.Height;
                }
            }
        }
    }
}
