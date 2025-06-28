using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ThinkITAM.DataBridge;
using ThinkITAM.ViewModels.Preset;
using ThinkITAM.Windows.PresetWindows;

namespace ThinkITAM.UserControls.PresetPage
{
    /// <summary>
    /// BrowserUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class BrowserUserControl : UserControl
    {
        public BrowserUserControl()
        {
            InitializeComponent();
        }



        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {
            AddBrowserWindow add = new AddBrowserWindow();

            //窗口放中间
            var window = Window.GetWindow(this);
            if (window != null)
            {
                add.Owner = window;
            }

            if (add.ShowDialog() == true)
            {
                LoadBrowserInfo();
            }
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




        private ObservableCollection<PresetPortClass> portInfos = new ObservableCollection<PresetPortClass>();

        private void LoadPortInfo()
        {
            portInfos.Clear();

            string query = "SELECT * FROM PortList;";


            var rows = GlobalVariables.DbService.ExecuteQuery(query);
            int index = 0;

            foreach (var row in rows)
            {
                index++;
                PresetPortClass info = new PresetPortClass();

                info.Index = index;
                info.Port = Convert.ToInt32(row["Port"].ToString());
                info.Note = row["Note"].ToString();

                portInfos.Add(info);
            }



        }



        private void BrowserUserControl_OnLoaded(object sender, RoutedEventArgs e)
        {



            ProtocolListView.ItemsSource = protocolInfos;
            PortListView.ItemsSource = portInfos;
            BrowserListView.ItemsSource = browserInfos;



            LoadProtocolInfo();
            LoadPortInfo();
            LoadBrowserInfo();


        }

        private void AddPortButton_OnClick(object sender, RoutedEventArgs e)
        {
            AddPortWindow add = new AddPortWindow();

            //窗口放中间
            var window = Window.GetWindow(this);
            if (window != null)
            {
                add.Owner = window;
            }

            if (add.ShowDialog() == true)
            {
                LoadPortInfo();
            }
        }

        private void AddProtocolButton_OnClick(object sender, RoutedEventArgs e)
        {
            AddProtocolWindow add = new AddProtocolWindow();

            //窗口放中间
            var window = Window.GetWindow(this);
            if (window != null)
            {
                add.Owner = window;
            }


            if (add.ShowDialog() == true)
            {
                LoadProtocolInfo();
            }
        }



        private ObservableCollection<ViewModels.Preset.ProtocolClass> protocolInfos = new ObservableCollection<ProtocolClass>();

        private void LoadProtocolInfo()
        {
            protocolInfos.Clear();

            string query = "SELECT * FROM Protocol;";


            var rows = GlobalVariables.DbService.ExecuteQuery(query);
            int index = 0;

            foreach (var row in rows)
            {
                index++;
                ProtocolClass info = new ProtocolClass();

                info.Index = index;
                info.Protocol = row["Protocol"].ToString();
                info.Note = row["Note"].ToString();

                protocolInfos.Add(info);
            }





        }



        private void ProtocolListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = ProtocolListView.SelectedIndex;

            if (index != -1)
            {
                DeleteProtocolButton.IsEnabled = true;
            }
            else
            {
                DeleteProtocolButton.IsEnabled = false;
            }
        }


        private void DeleteProtocolButton_OnClick(object sender, RoutedEventArgs e)
        {
            var index = ProtocolListView.SelectedIndex;

            if (index != -1)
            {

                var info = protocolInfos[index];

                var message = $"确定要删除吗？\r协议名称:{info.Protocol}\r协议备注:{info.Note}\r该操作不可逆！";


                var result = MessageBox.Show(message, "警告", MessageBoxButton.YesNo, MessageBoxImage.Question);


                if (result == MessageBoxResult.Yes)
                {
                    string query = $"DELETE FROM  Protocol WHERE Protocol='{info.Protocol}'";


                    GlobalVariables.DbService.ExecuteNonQuery(query);

                    LoadProtocolInfo();

                }

            }



        }

        private void PortListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = PortListView.SelectedIndex;

            if (index != -1)
            {
                DeletePortButton.IsEnabled = true;
            }
            else
            {
                DeletePortButton.IsEnabled = false;
            }
        }

        private void DeletePortButton_OnClick(object sender, RoutedEventArgs e)
        {
            var index = PortListView.SelectedIndex;

            if (index != -1)
            {

                var info = portInfos[index];

                var message = $"确定要删除吗？\r端口:{info.Port}\r端口备注:{info.Note}\r该操作不可逆！";


                var result = MessageBox.Show(message, "警告", MessageBoxButton.YesNo, MessageBoxImage.Question);


                if (result == MessageBoxResult.Yes)
                {
                    string query = $"DELETE FROM  PortList WHERE Port='{info.Port}'";


                    GlobalVariables.DbService.ExecuteNonQuery(query);

                    LoadPortInfo();

                }

            }
        }

        private void BrowserListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = BrowserListView.SelectedIndex;

            if (index != -1)
            {
                DeleteBrowserButton.IsEnabled = true;
            }
            else
            {
                DeleteBrowserButton.IsEnabled = false;
            }
        }

        private void DeleteBrowserButton_OnClick(object sender, RoutedEventArgs e)
        {
            var index = BrowserListView.SelectedIndex;

            if (index != -1)
            {

                var info = browserInfos[index];

                var message = $"确定要删除吗？\r名称:{info.Browser}\r路径:{info.Path}\r该操作不可逆！";


                var result = MessageBox.Show(message, "警告", MessageBoxButton.YesNo, MessageBoxImage.Question);


                if (result == MessageBoxResult.Yes)
                {
                    string query = $"DELETE FROM  Browser WHERE Port='{info.Browser}' AND Path='{info.Path}'";


                    GlobalVariables.DbService.ExecuteNonQuery(query);

                    LoadBrowserInfo();

                }

            }
        }
    }
}
