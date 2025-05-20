using Newtonsoft.Json;
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
using ThinkITAM.DataBridge;
using ThinkITAM.ViewModels.NetworkManage;

namespace ThinkITAM.Windows.NetworkManage
{
    /// <summary>
    /// NetworkEditWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NetworkEditWindow : Window
    {
        public NetworkEditWindow()
        {
            InitializeComponent();
        }



        private void NetworkEditWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            LoadTags();//加载自定义标签
            LoadHierarchyInfo();//加载层级信息


            if (!string.IsNullOrWhiteSpace(DataBridge.DataBridge.SelectNetworkInfo.NetworkId))
            {

                string query =
                    $"SELECT * FROM Network WHERE NetworkId='{DataBridge.DataBridge.SelectNetworkInfo.NetworkId}';";


                var rows = GlobalVariables.DbService.ExecuteQuery(query);
                var info = new NetworkInfoViewMode();
                foreach (var row in rows)
                {
                    info.Name = row["Name"].ToString();
                    info.Description = row["Description"].ToString();
                    info.Network = row["Network"].ToString();
                    info.Netmask = row["Netmask"].ToString();
                    info.Parent = row["Parent"].ToString();
                    info.Child = row["Child"].ToString();
                    info.TagA = row["TagA"].ToString();
                    info.TagB = row["TagB"].ToString();
                    info.TagC = row["TagC"].ToString();
                    info.TagD = row["TagD"].ToString();
                    info.TagE = row["TagE"].ToString();
                    info.TagF = row["TagF"].ToString();
                    //info.Percentage = CalculateUseValue(tableName);

                    if (info.Parent != null)
                    {
                        TbParent.SelectedIndex = parentList.IndexOf(info.Parent);
                    }

                    if (info.Child != null)
                    {
                        Child.SelectedIndex = childList.IndexOf(info.Child);
                    }

                }





                this.DataContext = info;
            }



        }




        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            var id = DataBridge.DataBridge.SelectNetworkInfo.NetworkId;
            var name = TbName.Text;
            var description = Description.Text;
            var parent = TbParent.Text;
            var child = Child.Text;
            var tagA = TagA.Text;
            var tagB = TagB.Text;
            var tagC = TagC.Text;
            var tagD = TagD.Text;


            var sql = $"UPDATE  Network  SET  Name  = '{name}', Description='{description}',Parent='{parent}',Child='{child}',TagA='{tagA}',TagB='{tagB}',TagC='{tagC}',TagD='{tagD}' WHERE NetworkId = '{id}'";


            GlobalVariables.DbService.ExecuteNonQuery(sql);

            this.DialogResult = true;

        }




        /// <summary>
        /// 加载自定义标签
        /// </summary>
        private void LoadTags()
        {

            var tags = DbClass.LoadWindowTag("AddNetwork");

            if (tags != null)
            {
                dynamic settings = JsonConvert.DeserializeObject(tags);

                LabelA.Content = settings.TagA + ":";
                LabelB.Content = settings.TagB + ":";
                LabelC.Content = settings.TagC + ":";
                LabelD.Content = settings.TagD + ":";
                LabelE.Content = settings.TagE + ":";
                LabelF.Content = settings.TagF + ":";
            }

        }


        private List<string> parentList = new List<string>();


        /// <summary>
        /// 加载组织信息
        /// </summary>
        private void LoadHierarchyInfo()
        {
            parentList.Clear();

            string query = "SELECT DISTINCT Parent FROM Network;";


            var rows = GlobalVariables.DbService.ExecuteQuery(query);

            foreach (var row in rows)
            {
                parentList.Add(row["Parent"].ToString());
            }


            TbParent.ItemsSource = parentList;


        }

        private List<string> childList = new List<string>();

        private void TbParent_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            childList.Clear();
            Child.ItemsSource = null;
            if (TbParent.SelectedIndex != -1)
            {
                string sql = $"SELECT DISTINCT Child FROM Network WHERE Parent='{parentList[TbParent.SelectedIndex]}'";

                var rows = GlobalVariables.DbService.ExecuteQuery(sql);

                foreach (var row in rows)
                {
                    childList.Add(row["Child"].ToString());
                }


                Child.ItemsSource = childList;

            }
        }
    }
}
