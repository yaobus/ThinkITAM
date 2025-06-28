using System.Windows;
using Newtonsoft.Json;
using ThinkITAM.DataBridge;
using ThinkITAM.ViewModels.LinkManage;

namespace ThinkITAM.Windows.LinkWindows
{
    /// <summary>
    /// TagModifyWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TagModifyWindow : Window
    {
        /// <summary>
        /// 传入两个参数
        /// </summary>
        /// <param name="parameter">要修改的标签类型</param>
        /// <param name="info">要修改的标签信息</param>
        public TagModifyWindow(string parameter, object info)
        {
            InitializeComponent();
            tagType = parameter;
            tagInfo = info;
        }

        private string tagType;
        private object tagInfo;






        private void TagModifyWindow_OnLoaded(object sender, RoutedEventArgs e)
        {


            switch (tagType)
            {
                case "port":

                    PortClass portInfo = (PortClass)tagInfo;

                    TitleTextBlock.Text = portInfo.PortIndex.ToString();

                    TagTextBox.Text = portInfo.PortTag;

                    break;

                case "slot":

                    SlotClass slotInfo = (SlotClass)tagInfo;

                    TitleTextBlock.Text = "槽位标签";

                    TagTextBox.Text = slotInfo.SlotName;

                    break;


                case "rack":

                    MdfRackClass rackInfo = (MdfRackClass)tagInfo;

                    TitleTextBlock.Text = "机架标签";
                    TagTextBox.Text = rackInfo.RackName;
                    break;


            }



        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {


            //if (TagTextBox.Text.Replace(" ", "").Length >= 2)
            //{
            string tag = TagTextBox.Text;
            string sql;

            switch (tagType)
            {
                case "port":

                    PortClass portInfo = (PortClass)tagInfo;

                    sql = $"UPDATE  Ra_{DataBridge.DataBridge.SelectRackId[0]}  SET  PortTag  = '{tag}' WHERE UID = '{portInfo.UID}'";


                    GlobalVariables.DbService.ExecuteNonQuery(sql);

                    DialogResult = true;

                    break;

                case "slot":

                    var slots = DataBridge.DataBridge.SelectRackInfo.slotInfos;

                    SlotClass slotInfo = (SlotClass)tagInfo;


                    var itemToUpdate = slots.FirstOrDefault(item => item.SlotIndex == slotInfo.SlotIndex);

                    if (itemToUpdate != null)
                    {
                        itemToUpdate.SlotName = $"{tag}";

                    }

                    foreach (var slot in slots)
                    {
                        slot.Ports = null;
                    }


                    // 将列表序列化为JSON字符串
                    string json = JsonConvert.SerializeObject(slots);


                    sql = $"UPDATE  Racks  SET  SlotInfos  = '{json}' WHERE RackId = '{DataBridge.DataBridge.SelectRackId[0]}'";



                    GlobalVariables.DbService.ExecuteNonQuery(sql);
                    DialogResult = true;


                    break;


                case "rack":

                    sql = $"UPDATE  Racks  SET  RackName  = '{tag}' WHERE RackId = '{DataBridge.DataBridge.SelectRackId[0]}'";



                    GlobalVariables.DbService.ExecuteNonQuery(sql);
                    DialogResult = true;

                    break;


            }





            // }

        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void ClearInput_OnClick(object sender, RoutedEventArgs e)
        {
            TagTextBox.Text = "";
        }
    }
}
