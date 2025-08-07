using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DocumentFormat.OpenXml.EMMA;
using Newtonsoft.Json;
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

                    //info.SortIndex = row["SortIndex"] as int?;


                    string? sort = row["SortIndex"].ToString();

                    if (!string.IsNullOrWhiteSpace(sort))
                    {
                        info.SortIndex = Convert.ToInt32(sort);

                    }
                    else
                    {
                        info.SortIndex = 0;
                    }


                    info.Description = row["Description"].ToString();
                    info.Network = row["Network"].ToString();
                    info.Netmask = row["Netmask"].ToString();
                    info.TagA = row["TagA"].ToString();
                    info.TagB = row["TagB"].ToString();
                    info.TagC = row["TagC"].ToString();
                    info.TagD = row["TagD"].ToString();
                    info.TagE = row["TagE"].ToString();
                    info.TagF = row["TagF"].ToString();

                    //info.Percentage = CalculateUseValue(tableName);

                    //if (info.TagA != null)
                    //{
                    //    TagA.SelectedItem = info.TagA;
                    //}

                    //if (info.TagB != null)
                    //{
                    //    TagB.SelectedItem = info.TagB;
                    //}

                }





                this.DataContext = info;
            }
            else
            {
                SortIndex.Text = DbClass.GetNextAvailableNumber("Network", "SortIndex").ToString();
            }



        }


        /// <summary>
        /// 是否是数字
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool IsNumeric(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;


            //如果只允许整数，使用 int.TryParse
            return int.TryParse(text, out _);
        }


        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {

            if (IsNumeric(SortIndex.Text) == false)
            {

                MessageBox.Show("网段排序只支持整数\r数字越大越靠前", "输入有误", MessageBoxButton.OK, MessageBoxImage.Information);

                return;
            }


            var id = DataBridge.DataBridge.SelectNetworkInfo.NetworkId;
            var sortIndex= Convert.ToInt32(SortIndex.Text); 
            var name = TbName.Text;
            var description = Description.Text;
            var tagA = TagA.Text;
            var tagB = TagB.Text;
            var tagC = TagC.Text;
            var tagD = TagD.Text;
            var tagE = TagE.Text;
            var tagF = TagF.Text;

            var sql = $"UPDATE  Network  SET  Name  = '{name}', SortIndex ='{sortIndex}', Description='{description}',TagA='{tagA}',TagB='{tagB}',TagC='{tagC}',TagD='{tagD}',TagE='{tagE}',TagF='{tagF}' WHERE NetworkId = '{id}'";


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

            string query = "SELECT DISTINCT TagA FROM Network WHERE TagA IS NOT NULL AND TagA != '';";


            var rows = GlobalVariables.DbService.ExecuteQuery(query);

            foreach (var row in rows)
            {
                parentList.Add(row["TagA"].ToString());
            }


            TagA.ItemsSource = parentList;


        }

        private List<string> childList = new List<string>();

        private void TbParent_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            childList.Clear();
            TagB.ItemsSource = null;
            if (TagA.SelectedIndex != -1)
            {
                string sql = $"SELECT DISTINCT TagB FROM Network WHERE TagA='{parentList[TagA.SelectedIndex]}' AND TagB IS NOT NULL AND TagB !='' ";

                var rows = GlobalVariables.DbService.ExecuteQuery(sql);

                foreach (var row in rows)
                {
                    childList.Add(row["TagB"].ToString());
                }


                TagB.ItemsSource = childList;

            }
        }

        private void SortIndex_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SortIndex.Text = DbClass.GetNextAvailableNumber("Network", "SortIndex").ToString();
        }
    }
}
