using System.Windows;
using System.Windows.Controls;

namespace ThinkITAM.UserControls.NetworkManage
{
    /// <summary>
    /// NetworkInfo.xaml 的交互逻辑
    /// </summary>
    public partial class NetworkInfo : UserControl
    {
        public NetworkInfo()
        {
            InitializeComponent();
        }

        // 假设节点名称为 Name，并且提供了一个公共属性来获取和设置节点名称
        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("TableName", typeof(string), typeof(SubNetworkInfo));

        public string TableName
        {
            get
            {
                return (string)GetValue(NameProperty);
            }
            set
            {
                SetValue(NameProperty, value);
            }
        }
    }
}
