using System;
using System.Collections.Generic;
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
using ThinkITAM.ViewModes.LinkManage;
using ThinkITAM.ViewModes.PortPanel;
using Microsoft.Xaml.Behaviors.Layout;
using Newtonsoft.Json;

namespace ThinkITAM.Windows.PortPanel
{
    /// <summary>
    /// PanelPortTagModify.xaml 的交互逻辑
    /// </summary>
    public partial class PanelPortTagModify : Window
    {
        public PanelPortTagModify(PortClass portInfo)
        {
            InitializeComponent();
            port = portInfo;
        }


        private PortClass port;
        private DbClass dbClass;
        private void PanelPortTagModify_OnLoaded(object sender, RoutedEventArgs e)
        {
            string dbFilePath = AppDomain.CurrentDomain.BaseDirectory + @"db\Address_database.db";

            dbClass = new DbClass(dbFilePath);
            dbClass.OpenConnection();


            TitleTextBlock.Text = port.PortIndex;
            TagTextBox.Text = port.PortTag;

        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {

            if (TagTextBox.Text.Replace(" ", "").Length >= 2)
            {
                string tag = TagTextBox.Text;

                string sql;

                 sql = $"UPDATE \"Bu_{DataBridge.DataBridge.SelectBuildingId}\" SET \"PortTag\" = '{tag}' WHERE  UID = '{port.UID}' ";

                
                dbClass.ExecuteQuery(sql);

                DialogResult = true;

            }

        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
