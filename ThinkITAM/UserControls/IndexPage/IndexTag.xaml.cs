using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using ThinkITAM.DataBridge;
using ThinkITAM.Windows.NetworkManage;

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


        private void IndexButton_OnClick(object sender, RoutedEventArgs e)
        {
            var tagInfo = (sender as Button).DataContext as ViewModels.Index.IndexTagViewModel;

            string url = $"{tagInfo.Protocol}{tagInfo.Host}";
            string browser = tagInfo.Browser;



            if (tagInfo.Port == null)//未配置端口
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
                    Functions.FunctionClass.OpenUrlClass.OpenUrlInSpecificBrowser(tagInfo.Url, browser);
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
            ViewModels.Index.IndexTagViewModel tagInfo = null;
            var menuItem = sender as MenuItem;
            if (menuItem != null)
            {
                var contextMenu = menuItem.Parent as ContextMenu;
                if (contextMenu != null)
                {
                    var button = contextMenu.PlacementTarget as Button;
                    if (button != null)
                    {
                        tagInfo = button.DataContext as ViewModels.Index.IndexTagViewModel;
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



                        string portSql;
                        if (tagInfo.Port.Length == 0)
                        {
                            portSql = $"(\"Port\"='' OR \"Port\" IS NULL) ";
                        }
                        else
                        {
                            portSql = $" \"Port\" = '{tagInfo.Port}' ";
                        }



                        string sql = $"DELETE FROM \"Bookmark\" WHERE ( \"TypeGroup\"='{tagInfo.Group}' AND Protocol='{tagInfo.Protocol}' AND Host='{tagInfo.Host}' AND {portSql})";




                        GlobalVariables.DbService.ExecuteNonQuery(sql);

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
