using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using ThinkITAM.DataBridge;
using ThinkITAM.ViewModels.Bookmark;

namespace ThinkITAM.Windows.Bookmark
{
    /// <summary>
    /// GroupOrderWindow.xaml 的交互逻辑
    /// </summary>
    public partial class GroupOrderWindow : Window
    {
        public GroupOrderWindow()
        {
            InitializeComponent();
            GroupsListView.ItemsSource = groups;
            groups.CollectionChanged += Groups_CollectionChanged;
        }

        private void Groups_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            //groups.CollectionChanged -= Groups_CollectionChanged;
            //int count = groups.Count;

            //if (count > 0)
            //{
            //    int index = 1;

            //    foreach (var item in groups)
            //    {

            //        Console.WriteLine(item.GroupName+item.DisplayOrder);

            //      //  item.DisplayOrder = index++;


            //    }


            //}
            //groups.CollectionChanged += Groups_CollectionChanged;
        }







        private void GroupOrderWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            LoadBookmarkGroups();
        }



        private ObservableCollection<GroupNameClass> groups = new ObservableCollection<GroupNameClass>();
        /// <summary>
        /// 加载书签组
        /// </summary>
        private void LoadBookmarkGroups()
        {
            groups.Clear();

            var sql = $"SELECT * FROM BookmarkGroupOrder WHERE Del != 1 OR Del IS NULL ORDER BY DisplayOrder ASC ";

            var rows = GlobalVariables.DbService.ExecuteQuery(sql);

            foreach (var row in rows)
            {
                var info = new ViewModels.Bookmark.GroupNameClass();
                info.DisplayOrder = Convert.ToInt32(row["DisplayOrder"]);
                info.TypeGroup = row["TypeGroup"].ToString();
                groups.Add(info);
            }


        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            int index = 0;

            foreach (var item in groups)
            {
                index++;
                var info = new GroupNameClass();
                info.DisplayOrder = index;
                info.TypeGroup = item.TypeGroup;

                var conditions = new { TypeGroup = item.TypeGroup };


                GlobalVariables.DbService.UpdateEntity("BookmarkGroupOrder", info, conditions);

            }

            this.DialogResult = true;
        }
    }
}
