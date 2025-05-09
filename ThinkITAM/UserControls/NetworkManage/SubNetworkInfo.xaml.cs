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

namespace ThinkITAM.UserControls.NetworkManage
{
    /// <summary>
    /// SubNetworkInfo.xaml 的交互逻辑
    /// </summary>
    public partial class SubNetworkInfo : UserControl
    {
        public SubNetworkInfo()
        {
            InitializeComponent();
        }

        // 假设节点名称为 Name，并且提供了一个公共属性来获取和设置节点名称
        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("SubTableName", typeof(string), typeof(SubNetworkInfo));

        public string SubTableName
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }
    }
}
