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
using ThinkITAM.Windows.LinkWindows;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.FunctionClass;
using ThinkITAM.Functions.FunctionClass;
using ThinkITAM.ViewModels.LinkManage;

namespace ThinkITAM.UserControls.LinkPage
{
    /// <summary>
    /// ODF.xaml 的交互逻辑
    /// </summary>
    public partial class MDF : UserControl
    {
        public MDF()
        {
            InitializeComponent();
        }



        private void TagButton_OnClick(object sender, RoutedEventArgs e)
        {

            SlotClass slot = (SlotClass)this.DataContext;


            // 获取 rack 层的 DataContext
            var rackDataContext = (sender as FrameworkElement)?.FindAncestor<Rack>()?.DataContext;

            if (rackDataContext != null)
            {
                var info = rackDataContext as MdfRackClass;

                if (info != null)
                {
                    DataBridge.DataBridge.SelectRackId.Clear();
                    DataBridge.DataBridge.SelectRackId.Add(info.RackId);
                }


            }


            TagModifyWindow add = new TagModifyWindow("slot", slot);

            //窗口放中间
            var window = Window.GetWindow(this);
            if (window != null)
            {
                add.Owner = window;
            }



            if (add.ShowDialog() == true)
            {
                DataBridge.DataBridge.ModifyTagList.Add("SlotTag");
                // 当子窗口关闭后执行这里的代码

            }
        }


        private void MDF_OnLoaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
