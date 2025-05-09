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


    }
}
