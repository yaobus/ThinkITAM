using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.DataBridge;
using ThinkITAM.UserControls.PresetPage;
using ThinkITAM.ViewModels.Preset;
using ThinkITAM.Windows.PresetWindows;

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

            //初始化时打开第一个标签页
            PresetTreeView.SelectedIndex = 0;



        }







        private ObservableCollection<BrowserInfoViewModel> browserInfos = new ObservableCollection<BrowserInfoViewModel>();

        private void LoadBrowserInfo()
        {

            browserInfos.Clear();

            string query = "SELECT * FROM Browser;";

            var rows = GlobalVariables.DbService.ExecuteQuery(query);


            int index = 0;

            foreach (var row in rows)
            {
                index++;
                BrowserInfoViewModel info = new BrowserInfoViewModel();

                info.Index = index;
                info.Browser = row["Browser"].ToString();
                info.Path = row["Path"].ToString();

                browserInfos.Add(info);
            }



            //BrowserListView.ItemsSource = browserInfos;


        }

        private ObservableCollection<AddressInfoViewModel> addressInfos = new ObservableCollection<AddressInfoViewModel>();


        private void LoadAddress()
        {
            addressInfos.Clear();

            string query = "SELECT * FROM Address;";


            var rows = GlobalVariables.DbService.ExecuteQuery(query);
            int index = 0;

            foreach (var row in rows)
            {
                index++;
                AddressInfoViewModel info = new AddressInfoViewModel();

                info.Index = index;
                info.Location = row["Location"].ToString();
                info.Note = row["Note"].ToString();

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


            var rows = GlobalVariables.DbService.ExecuteQuery(query);
            int index = 0;

            foreach (var row in rows)
            {
                index++;
                OrganizationViewModel info = new OrganizationViewModel();

                info.Index = index;
                info.Organization = row["Organization"].ToString();
                info.Department = row["Department"].ToString();
                info.Note = row["Note"].ToString();

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


            var rows = GlobalVariables.DbService.ExecuteQuery(query);

            int index = 0;

            foreach (var row in rows)
            {
                index++;
                PeopleViewModel info = new PeopleViewModel();
                info.Index = index;
                info.Name = row["Name"].ToString();
                info.Organization = row["Organization"].ToString();
                info.Department = row["Department"].ToString();
                info.Phone = row["Phone"].ToString();
                info.Note = row["Note"].ToString();

                peopleInfos.Add(info);
            }



            //PeopleListView.ItemsSource = peopleInfos;


        }


        private void AddAssetTag_OnClick(object sender, RoutedEventArgs e)
        {
            AddAssetTagWindow addAssetTag = new AddAssetTagWindow();

            if (addAssetTag.ShowDialog() == true)
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



            //AssetTagListView.ItemsSource = assetTags;

        }



        private void AddAddressButton_OnClick(object sender, RoutedEventArgs e)
        {
            AddAddressWindow add = new AddAddressWindow();


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
                        var organization = new OrganizationUserControl();


                        organization.Style = (Style)FindResource("OrganizationUserControlPageStyle");

                        PresetSubPanel.Children.Add(organization);

                        break;

                    case 1:

                        PresetSubPanel.Children.Clear();
                        var peopleUserControl = new PeopleUserControl();


                        peopleUserControl.Style = (Style)FindResource("PeopleUserControlPageStyle");
                        PresetSubPanel.Children.Add(peopleUserControl);

                        break;

                    case 2:

                        PresetSubPanel.Children.Clear();
                        var addressUserControl = new AddressUserControl();


                        addressUserControl.Style = (Style)FindResource("AddressUserControlPageStyle");


                        PresetSubPanel.Children.Add(addressUserControl);

                        break;


                    case 3:


                        PresetSubPanel.Children.Clear();
                        var cabinetUserControl = new CabinetUserControl();


                        cabinetUserControl.Style = (Style)FindResource("CabinetUserControlPageStyle");


                        PresetSubPanel.Children.Add(cabinetUserControl);

                        break;

                    case 4:

                        PresetSubPanel.Children.Clear();

                        var buildingsUserControl = new BuildingsUserControl();


                        buildingsUserControl.Style = (Style)FindResource("BuildingsUserControlPageStyle");


                        PresetSubPanel.Children.Add(buildingsUserControl);

                        break;

                    case 5:

                        PresetSubPanel.Children.Clear();

                        var assetTypeUserControl = new AssetTypeUserControl();


                        assetTypeUserControl.Style = (Style)FindResource("AssetTypeUserControlPageStyle");


                        PresetSubPanel.Children.Add(assetTypeUserControl);




                        break;

                    case 6:

                        PresetSubPanel.Children.Clear();

                        var browserUserControl = new BrowserUserControl();


                        browserUserControl.Style = (Style)FindResource("BrowserUserControlPageStyle");



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
