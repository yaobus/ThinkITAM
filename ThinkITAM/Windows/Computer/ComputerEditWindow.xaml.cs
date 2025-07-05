using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.DataBridge;
using ThinkITAM.ViewModels.DatabaseEntity.Computer;
using ThinkITAM.ViewModels.LinkManage;
using ThinkITAM.ViewModels.NetworkManage;

namespace ThinkITAM.Windows.Computer
{
    /// <summary>
    /// NetworkEditWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ComputerEditWindow : Window
    {
        public ComputerEditWindow(ObservableCollection<PortClass>  portInfos=null)
        {
            InitializeComponent();
            TagA.ItemsSource=tagAs;
            TagB.ItemsSource=tagBs;

            if (portInfos != null)
            {
                InfoTextBox.Text = $"共选中{portInfos[0].AssetNumber}等{portInfos.Count}个终端"; ;

                inputInfos = portInfos;
                this.DataContext = inputInfos[0];
            }


        }


        private ObservableCollection<PortClass> inputInfos;
        private void ComputerEditWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            LoadTags();//加载自定义标签
            LoadLabelAs();


        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (var item in inputInfos)
            {
                var info = new ComputerEntityViewModel();

                info.UID = item.UID;
                info.DeviceId = item.DeviceId;
                info.AssetId = item.AssetId;
                info.AssetUser = item.AssetUser;
                info.PortId = item.PortIndex;

                if (ExcludeTag.IsChecked == true)
                {
                    info.PortTag = item.PortTag;
                }
                else
                {
                    info.PortTag = PortTag.Text;
                }


                if (ExcludeGroup.IsChecked ==true)
                {
                    info.PortGroup= item.PortGroup;
                }
                else
                {
                   info.PortGroup = PortGroup.Text;
                }


                info.PortType = item.PortType;
                info.PortStatus = PortStatus.Text;
                
                info.OnTheLine = item.OnTheLine;
                info.PortColor = PortColor.SelectedIndex;
                info.BuildingId = item.BuildingId;
                info.Floor = item.SlotIndex;
                info.Room = item.Room;
                info.LinkIp = item.LinkIp;


                info.TagA = TagA.Text;
                info.TagB = TagB.Text;
                info.TagC = TagC.Text;
                info.TagD = TagD.Text;
                info.TagE = TagE.Text;
                info.TagF = TagF.Text;

                var conditions = new { UID = item.UID };

                GlobalVariables.DbService.UpdateEntity("Computer", info, conditions);

            }

            this.DialogResult = true;
        }



        private ObservableCollection<string>tagAs= new ObservableCollection<string>();
        /// <summary>
        /// 加载自定义标签A（带有层级关系的标签A），
        /// </summary>
        private void LoadLabelAs()
        {
            tagAs.Clear();

            var sql = $"SELECT DISTINCT(TagA) FROM Computer WHERE TagA IS NOT NULL AND TagA != '' AND (Del != 1 OR Del IS NULL)";

            var rows = GlobalVariables.DbService.ExecuteQuery(sql);

            foreach (var row in rows)
            {
                tagAs.Add(row["TagA"].ToString());
            }


        }


        /// <summary>
        /// 加载自定义标签
        /// </summary>
        private void LoadTags()
        {

            var tags = DbClass.LoadWindowTag("ComputerTag");

            if (tags != null)
            {
                dynamic settings = JsonConvert.DeserializeObject(tags);

                LabelA.Content = settings.TagA;
                LabelB.Content = settings.TagB;
                LabelC.Content = settings.TagC;
                LabelD.Content = settings.TagD;
                LabelE.Content = settings.TagE;
                LabelF.Content = settings.TagF;
            }

        }


        private void PortColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = PortColor.SelectedIndex;

            int x = 0;



            if (index != -1)
            {
                foreach (var selectedItem in PortColor.Items)
                {
                    var item = selectedItem as ListBoxItem;

                    if (item != null && index == x)
                    {
                        item.Opacity = 1;
                        item.BorderBrush = SystemColors.ActiveBorderBrush;
                        item.BorderThickness = new Thickness(2);
                    }
                    else
                    {
                        item.Opacity = 0.1;
                        item.BorderBrush = null;
                        item.BorderThickness = new Thickness(0);
                    }

                    x++;
                }

            }
        }


        private ObservableCollection<string> tagBs = new ObservableCollection<string>();
        private void TagA_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tagBs.Clear();

            var sql = $"SELECT DISTINCT(TagB) FROM Computer WHERE TagA = '{TagA.SelectedItem}' AND TagB IS NOT NULL AND TagB != '' AND (Del != 1 OR Del IS NULL)";

            var rows = GlobalVariables.DbService.ExecuteQuery(sql);

            foreach (var row in rows)
            {
                tagBs.Add(row["TagB"].ToString());
            }

        }
    }
}
