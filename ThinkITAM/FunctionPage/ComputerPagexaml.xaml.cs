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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ThinkITAM.Windows.Computer;
using ThinkITAM.Windows.DevicePortManage;

namespace ThinkITAM.FunctionPage
{
    /// <summary>
    /// ComputerPagexaml.xaml 的交互逻辑
    /// </summary>
    public partial class ComputerPagexaml : UserControl
    {
        public ComputerPagexaml()
        {
            InitializeComponent();
        }

        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {
            var newWindow = new AddComputerWindow();


            //窗口放中间
            var window = Window.GetWindow(this);
            if (window != null)
            {
                newWindow.Owner = window;
            }



            if (newWindow.ShowDialog() == true)
            {

                // 当子窗口关闭后执行这里的代码
                //LoadAssetTreeViewInfos();

                //加载设备信息
                //LoadTags();
            }
        }
    }
}
