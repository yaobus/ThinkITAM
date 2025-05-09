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
    /// BrowserUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class BrowserUserControl : UserControl
    {
        public BrowserUserControl()
        {
            InitializeComponent();
        }

        private DbClass dbClass;

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




        private ObservableCollection<PresetPortClass> portInfos = new ObservableCollection<PresetPortClass>();

        private void LoadPortInfo()
        {
            portInfos.Clear();

            string query = "SELECT * FROM PortList;";

            SQLiteCommand command = new SQLiteCommand(query, dbClass.connection);
            SQLiteDataReader reader = command.ExecuteReader();

            int index = 0;

            while (reader.Read())
            {
                index++;
                PresetPortClass info = new PresetPortClass();

                info.Index = index;
                info.Port = Convert.ToInt32(reader["Port"].ToString());
                info.Note = reader["Note"].ToString();

                portInfos.Add(info);
            }

        }



        private void BrowserUserControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            string dbFilePath = AppDomain.CurrentDomain.BaseDirectory + @"db\Address_database.db";
            dbClass = new DbClass(dbFilePath);
            dbClass.OpenConnection();


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

        

        private ObservableCollection<ViewModes.Preset.ProtocolClass> protocolInfos = new ObservableCollection<ProtocolClass>();

        private void LoadProtocolInfo()
        {
            protocolInfos.Clear();

            string query = "SELECT * FROM Protocol;";

            SQLiteCommand command = new SQLiteCommand(query, dbClass.connection);
            SQLiteDataReader reader = command.ExecuteReader();

            int index = 0;

            while (reader.Read())
            {
                index++;
                ProtocolClass info = new ProtocolClass();

                info.Index = index;
                info.Protocol = reader["Protocol"].ToString();
                info.Note = reader["Note"].ToString();

                protocolInfos.Add(info);
            }




        }


    }
}
