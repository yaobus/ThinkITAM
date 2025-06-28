using System.Windows;
using ThinkITAM.DataBridge;
using ThinkITAM.ViewModels.LinkManage;

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

        private void PanelPortTagModify_OnLoaded(object sender, RoutedEventArgs e)
        {

            TitleTextBlock.Text = port.PortIndex;
            TagTextBox.Text = port.PortTag;

        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {

            if (TagTextBox.Text.Replace(" ", "").Length >= 2)
            {
                string tag = TagTextBox.Text;

                string sql;

                sql = $"UPDATE  Bu_{DataBridge.DataBridge.SelectBuildingId}  SET  PortTag  = '{tag}' WHERE  UID = '{port.UID}' ";



                GlobalVariables.DbService.ExecuteNonQuery(sql);

                DialogResult = true;

            }

        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
