using ThinkITAM.ChildrenWindows.NetworkManage;
using ThinkITAM.DatabaseOperation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace ThinkITAM.UserControls.IndexPage
{
    /// <summary>
    /// IndexTag.xaml 的交互逻辑
    /// </summary>
    public partial class IndexTag : UserControl
    {
        public IndexTag()
        {
            InitializeComponent();
        }

        private DbClass dbClass;
        private void IndexButton_OnClick(object sender, RoutedEventArgs e)
        {
            var tagInfo = (sender as Button).DataContext as ViewModes.Index.IndexTagViewModel;

            string url = $"{tagInfo.Protocol}{tagInfo.Host}";
            string browser = tagInfo.Browser;

            if (tagInfo.Port.Length == 0)//未配置端口
            {
                tagInfo.Url = url;
            }
            else
            {
                tagInfo.Url = $"{url}:{tagInfo.Port}";
            }

            try
            {
                if (browser != null && browser.Length > 0)//有指定浏览器
                {
                    FunctionClass.OpenUrlClass.OpenUrlInSpecificBrowser(url, browser);
                }
                else
                {
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }


               
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                
            }


            
            

        }



        /// <summary>
        /// 右键菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModes.Index.IndexTagViewModel tagInfo=null;
            var menuItem = sender as MenuItem;
            if (menuItem != null)
            {
                var contextMenu = menuItem.Parent as ContextMenu;
                if (contextMenu != null)
                {
                    var button = contextMenu.PlacementTarget as Button;
                    if (button != null)
                    {
                        tagInfo = button.DataContext as ViewModes.Index.IndexTagViewModel;
                        // 在这里进行进一步的操作...
                    }
                }
            }

            //string url = DataBridge.DataBridge.SelectNetwork + selectAddress;



            if (menuItem != null)
            {
                // 根据菜单项的不同进行相应的处理
                switch (menuItem.Header.ToString())
                {
                    case "删除标签":

                        // 执行选项1的操作
                        dbClass = new DbClass(DataBridge.DataBridge.dbFilePath);
                        dbClass.OpenConnection();

                        string portSql;
                        if (tagInfo.Port.Length == 0)
                        {
                            portSql = $"(\"Port\"='' OR \"Port\" IS NULL) ";
                        }
                        else
                        {
                            portSql = $" \"Port\" = '{tagInfo.Port}' ";
                        }



                        string sql = $"DELETE FROM \"Index\" WHERE ( \"Group\"='{tagInfo.Group}' AND Protocol='{tagInfo.Protocol}' AND Host='{tagInfo.Host}' AND {portSql})";

                        

                        dbClass.ExecuteQuery(sql);

                        DataBridge.DataBridge.modifyIndexTags.Add("1");

                        break;

                    case "编辑标签":

                        if (tagInfo != null)
                        {
                            AddressCollectWindow addressCollectWindow = new AddressCollectWindow(null, tagInfo);
                            //窗口放中间
                            var window = Window.GetWindow(this);
                            if (window != null)
                            {
                                addressCollectWindow.Owner = window;
                            }

                            addressCollectWindow.ShowDialog();


                        }

                        break;




                }
            }
        }
    }
}
