using System.Windows;
using System.Windows.Controls;

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
