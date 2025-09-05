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
using System.Windows.Shapes;
using DocumentFormat.OpenXml.ExtendedProperties;
using MaterialDesignThemes.Wpf;
using ThinkITAM.DataBridge;
using ThinkITAM.ViewModels.Others;

namespace ThinkITAM.Windows.ToolWindows
{
    /// <summary>
    /// NotePadWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NotePadWindow : Window
    {
        public NotePadWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 启动时加载笔记本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NotePadWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            LoadNoteBooks();
            MessageQueue = new SnackbarMessageQueue();
        }

        public SnackbarMessageQueue MessageQueue { get; set; }


        /// <summary>
        /// 加载笔记本
        /// </summary>
        private void LoadNoteBooks(string keyword=null)
        {
            //NoteTree.Items.Clear();


            string query = string.Empty;

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = $"SELECT DISTINCT * FROM NoteBook WHERE( Del != 1 OR Del IS NULL)  AND  (NoteGroup LIKE '%{keyword}%' OR NoteUnit Like '%{keyword}%' OR NoteName LIKE '%{keyword}%' OR Note LIKE '%{keyword}%' )";

            }
            else
            {
                query = $"SELECT DISTINCT * FROM NoteBook WHERE Del != 1 OR Del IS NULL ;";
            }

            

            var noteList = new List<NoteBookViewModel>();


            var rows = GlobalVariables.DbService.ExecuteQuery(query);

            foreach (var row in rows)
            {
                var node = new NoteBookViewModel();
                node.BookGroup= row["NoteGroup"].ToString();
                node.BookUnit= row["NoteUnit"].ToString();
                node.BookName= row["NoteName"].ToString();
                node.NoteId= row["NoteId"].ToString();
                node.CreatedDate= row["CreatedDate"].ToString();
                node.EditDate= row["EditDate"].ToString();
                node.Note= row["Note"].ToString();

                noteList.Add(node);

            }



            // 遍历笔记本名称列表，然后取出笔记本全部章节（去重）

            var treeData = BuildTree(noteList);

            NoteTree.ItemsSource=treeData;

            //foreach (var row in rows)
            //{
            //    TreeViewItem groupItem = new TreeViewItem
            //    {
            //        Header = row["NoteGroup"],
            //        Tag = row["NoteGroup"]
            //    };

            //    var sql =
            //        $"SELECT DISTINCT NoteUnit FROM NoteBook WHERE (Del != 1 OR Del IS NULL) AND NoteGroup = '{row["NoteGroup"]}';";

            //    var units = GlobalVariables.DbService.ExecuteQuery(sql);

            //    foreach (var unit in units)
            //    {

            //        TreeViewItem item = new TreeViewItem
            //        {
            //            Header = $"{unit["NoteUnit"]}",
            //            Tag = new { Group = row["NoteGroup"], Unit = unit["NoteUnit"] }
            //        };


            //        //遍历章节列表，生成 TreeViewItem 并添加到 TreeView 控件中

            //        var sql2 =$"SELECT * FROM NoteBook WHERE (Del != 1 OR Del IS NULL) AND NoteGroup = '{row["NoteGroup"]}' AND NoteUnit = '{unit["NoteUnit"]}';";



            //        var notes = GlobalVariables.DbService.ExecuteQuery(sql2);


            //        foreach (var note in notes)
            //        {
            //            if (string.IsNullOrWhiteSpace(note.ToString()))
            //            {
            //                TreeViewItem book = new TreeViewItem()
            //                {
            //                    Header = note["NoteName"],
            //                    Tag = new
            //                    {
            //                        Group = note["NoteGroup"],
            //                        Unit = note["NoteUnit"],
            //                        NoteName = note["NoteName"],
            //                        NoteId = note["NoteId"]
            //                    }
            //                };

            //                groupItem.Items.Add(book);
            //            }
            //            else
            //            {
            //                TreeViewItem book = new TreeViewItem()
            //                {
            //                    Header = note["NoteName"],
            //                    Tag = new
            //                    {
            //                        Group = note["NoteGroup"],
            //                        Unit = note["NoteUnit"],
            //                        NoteName = note["NoteName"],
            //                        NoteId = note["NoteId"]
            //                    }
            //                };


            //                item.Items.Add(book);
            //            }


            //        }

            //        if (item.Items.Count > 0)
            //        {
            //            groupItem.Items.Add(item);
            //        }


            //    }


            //    NoteTree.Items.Add(groupItem);



            //}




        }

        private ObservableCollection<TreeItem> BuildTree(List<NoteBookViewModel> records)
        {
            var treeItems = new ObservableCollection<TreeItem>();
            var groupDict = new Dictionary<string, TreeItem>();

            foreach (var record in records)
            {
                if (string.IsNullOrEmpty(record.BookGroup))
                    continue; // 跳过无效数据

                // 获取或创建 BookGroup 节点
                if (!groupDict.TryGetValue(record.BookGroup, out TreeItem groupItem))
                {
                    groupItem = new TreeItem { Name = record.BookGroup };
                    groupDict[record.BookGroup] = groupItem;
                    treeItems.Add(groupItem);
                }

                // 判断 BookUnit 是否为空
                if (string.IsNullOrEmpty(record.BookUnit))
                {
                    // 直接添加 BookName 到 BookGroup 下
                    groupItem.Children.Add(new TreeItem { Name = record.BookName, Tag = record });
                }
                else
                {
                    // 查找或创建 BookUnit 节点
                    TreeItem unitItem = null;
                    var existingUnit = groupItem.Children.FirstOrDefault(c => c.Name == record.BookUnit);
                    if (existingUnit != null && existingUnit.Tag == null) // Tag 为 null 表示它是分组节点，不是 BookName
                    {
                        unitItem = existingUnit;
                    }
                    else
                    {
                        unitItem = new TreeItem { Name = record.BookUnit };
                        groupItem.Children.Add(unitItem);
                    }

                    // 将 BookName 添加到 BookUnit 节点下
                    unitItem.Children.Add(new TreeItem { Name = record.BookName, Tag = record });
                }
            }

            return treeItems;
        }


        /// <summary>
        /// 加粗
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OverStrikingButton_OnClick(object sender, RoutedEventArgs e)
        {
            InsertBeMarkDownCode("**");
        }

        /// <summary>
        /// 斜体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormatItalicButton_OnClick(object sender, RoutedEventArgs e)
        {
            InsertBeMarkDownCode("*");
        }

        /// <summary>
        /// 在鼠标选中文本首尾插入 Markdown 代码符号
        /// </summary>
        /// <param name="mdCode"></param>
        private void InsertBeMarkDownCode(string mdCode)
        {
            // 获取 TextBox 控件
            TextBox textBox = this.InputTextBox;
            // 检查是否有选中的文本
            if (!string.IsNullOrEmpty(textBox.SelectedText) && textBox.SelectionLength > 0)
            {
                int selectionStart = textBox.SelectionStart;
                int selectionLength = textBox.SelectionLength;

                // 获取原始文本
                string originalText = textBox.Text;

                // 构建新文本：在选中部分前后加上符号
                StringBuilder sb = new StringBuilder(originalText);
                sb.Insert(selectionStart + selectionLength, mdCode); // 先在末尾插入后缀
                sb.Insert(selectionStart, mdCode); // 再在开头插入前缀

                // 更新 TextBox 文本
                textBox.Text = sb.ToString();

                // 调整光标位置和选中范围
                // 将光标定位到新插入的后缀之后
                textBox.CaretIndex = selectionStart + mdCode.Length + selectionLength + mdCode.Length;

                // 可选：取消选中状态
                textBox.Select(0, 0);
            }
            else
            {
                // 没有选中任何文本时的提示（可选）
                MessageBox.Show("请先选中一段文本。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

        /// <summary>
        /// 在 TextBox 当前光标位置插入指定文本
        /// </summary>
        /// <param name="textBox">TextBox 控件</param>
        /// <param name="textToInsert">要插入的文本</param>
        private void InsertTextAtCaret(string textToInsert)
        {
            // 获取 TextBox 控件
            TextBox textBox = this.InputTextBox;

            if (textBox == null) return;
            if (string.IsNullOrEmpty(textToInsert)) return;

            // 获取当前光标位置
            int caretIndex = textBox.CaretIndex;
            string currentText = textBox.Text;

            // 如果文本为空，直接设置
            if (string.IsNullOrEmpty(currentText))
            {
                textBox.Text = textToInsert;
                textBox.CaretIndex = textToInsert.Length;
                return;
            }

            // 在光标位置插入文本
            textBox.Text = currentText.Insert(caretIndex, textToInsert);

            // 将光标移动到插入内容的末尾
            textBox.CaretIndex = caretIndex + textToInsert.Length;
        }


        /// <summary>
        /// 在鼠标所在行的行首插入指定字符串
        /// </summary>
        /// <param name="textToInsert"></param>
        public void InsertAtLineStart(string textToInsert)
        {
            // 获取 TextBox 控件
            TextBox textBox = this.InputTextBox;

            if (textBox == null || string.IsNullOrEmpty(textToInsert))
                return;

            string originalText = textBox.Text;
            int caretIndex = textBox.CaretIndex;

            // 如果文本为空，直接插入并设置光标
            if (string.IsNullOrEmpty(originalText))
            {
                textBox.Text = textToInsert;
                textBox.CaretIndex = textToInsert.Length;
                return;
            }

            // 获取光标所在行号
            int lineIndex = textBox.GetLineIndexFromCharacterIndex(caretIndex);

            // 获取该行的起始字符索引
            int lineStartIndex = textBox.GetCharacterIndexFromLineIndex(lineIndex);

            // 构建新文本：在行首插入指定字符串
            string newText = originalText.Insert(lineStartIndex, textToInsert);

            // 更新 TextBox 文本
            textBox.Text = newText;

            // 调整光标位置：原光标位置 + 插入字符串的长度
            textBox.CaretIndex = caretIndex + textToInsert.Length;

            // 可选：滚动到光标位置（尤其当内容超出可视区域时）
            textBox.ScrollToHorizontalOffset(textBox.HorizontalOffset);
            textBox.ScrollToVerticalOffset(textBox.VerticalOffset);
        }

        /// <summary>
        /// 在指定 TextBox 的光标位置插入一个 X 行 Y 列的 Markdown 表格模板
        /// </summary>
        /// <param name="rows">总行数（包含表头和分隔行）</param>
        /// <param name="cols">列数</param>
        public void InsertMarkdownTable(int rows, int cols)
        {
            rows += 1;
            // 获取 TextBox 控件
            TextBox textBox = this.InputTextBox;

            if (textBox == null) return;
            if (rows < 2 || cols < 1) throw new ArgumentException("行数至少为2（包含表头和分隔行），列数至少为1。");

            var sb = new System.Text.StringBuilder();

            // 第1行：表头 | 列1 | 列2 | ... |
            sb.Append("|");
            for (int i = 1; i <= cols; i++)
            {
                sb.Append($" 列{i} |");
            }

            sb.AppendLine();

            // 第2行：分隔行 | --- | --- | ... |
            sb.Append("|");
            for (int i = 0; i < cols; i++)
            {
                sb.Append(" --- |");
            }

            sb.AppendLine();

            // 第3行到第X行：空数据行
            for (int row = 2; row < rows; row++) // 从第3行开始（索引2）
            {
                sb.Append("|");
                for (int col = 0; col < cols; col++)
                {
                    sb.Append("     |"); // 留空格方便填写
                }

                sb.AppendLine();
            }

            // 获取当前光标位置
            int caretIndex = textBox.CaretIndex;
            string currentText = textBox.Text;

            // 插入表格
            string newText = currentText.Insert(caretIndex, "\r\r" + sb.ToString());

            // 更新文本
            textBox.Text = newText;

            // 将光标移到插入内容的末尾
            textBox.CaretIndex = caretIndex + sb.Length;

            // 可选：滚动到光标位置
            textBox.ScrollToHorizontalOffset(textBox.HorizontalOffset);
            textBox.ScrollToVerticalOffset(textBox.VerticalOffset);
        }


        /// <summary>
        /// 在 TextBox 选中文本的每一行前面插入指定前缀
        /// </summary>
        /// <param name="textBox">TextBox 控件</param>
        /// <param name="prefix">要插入的前缀，如 "> ", "# " 等</param>
        public void InsertPrefixToEachSelectedLine(string prefix)
        {
            // 获取 TextBox 控件
            TextBox textBox = this.InputTextBox;

            if (!string.IsNullOrEmpty(textBox.SelectedText) && textBox.SelectionLength > 0)
            {

                if (textBox == null) return;
                if (string.IsNullOrEmpty(prefix)) return;
                if (string.IsNullOrEmpty(textBox.SelectedText)) return;

                int selectionStart = textBox.SelectionStart;
                int selectionLength = textBox.SelectionLength;
                string selectedText = textBox.SelectedText;
                string fullText = textBox.Text;

                // 按行拆分选中的文本（支持 \r\n 和 \n）
                string[] lines = selectedText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

                // 为每一行添加前缀
                for (int i = 0; i < lines.Length; i++)
                {
                    lines[i] = prefix + lines[i];
                }

                // 用 Environment.NewLine 重新拼接（保持与系统一致的换行符）
                string newSelectedText = string.Join(Environment.NewLine, lines);

                // 替换原文本中的选中部分
                string newText = fullText.Substring(0, selectionStart) +
                                 newSelectedText +
                                 fullText.Substring(selectionStart + selectionLength);

                // 更新 TextBox
                textBox.Text = newText;

                // 重新选中新插入的区域（可选）
                textBox.Select(selectionStart, newSelectedText.Length);

                // 或者：将光标放在最后
                // textBox.CaretIndex = selectionStart + newSelectedText.Length;
            }
            else
            {
                // 没有选中任何文本时的提示（可选）
                MessageBox.Show("请先选中一段文本。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }


        /// <summary>
        /// 在 TextBox 选中文本的每一行前面插入指定前缀,带ID
        /// </summary>
        /// <param name="textBox">TextBox 控件</param>
        /// <param name="prefix">要插入的前缀，如 "> ", "# " 等</param>
        public void InsertPrefixIndexToEachSelectedLine(string prefix)
        {
            // 获取 TextBox 控件
            TextBox textBox = this.InputTextBox;

            if (!string.IsNullOrEmpty(textBox.SelectedText) && textBox.SelectionLength > 0)
            {

                if (textBox == null) return;
                if (string.IsNullOrEmpty(prefix)) return;
                if (string.IsNullOrEmpty(textBox.SelectedText)) return;

                int selectionStart = textBox.SelectionStart;
                int selectionLength = textBox.SelectionLength;
                string selectedText = textBox.SelectedText;
                string fullText = textBox.Text;

                // 按行拆分选中的文本（支持 \r\n 和 \n）
                string[] lines = selectedText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

                // 为每一行添加前缀
                for (int i = 0; i < lines.Length; i++)
                {
                    lines[i] = prefix + (i + 1) + " " + lines[i];
                }

                // 用 Environment.NewLine 重新拼接（保持与系统一致的换行符）
                string newSelectedText = string.Join(Environment.NewLine, lines);

                // 替换原文本中的选中部分
                string newText = fullText.Substring(0, selectionStart) +
                                 newSelectedText +
                                 fullText.Substring(selectionStart + selectionLength);

                // 更新 TextBox
                textBox.Text = newText;

                // 重新选中新插入的区域（可选）
                textBox.Select(selectionStart, newSelectedText.Length);

                // 或者：将光标放在最后
                // textBox.CaretIndex = selectionStart + newSelectedText.Length;
            }
            else
            {
                // 没有选中任何文本时的提示（可选）
                MessageBox.Show("请先选中一段文本。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

        /// <summary>
        /// H1标题
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetH1_OnClick(object sender, RoutedEventArgs e)
        {
            InsertAtLineStart("# ");
        }

        /// <summary>
        /// H2标题
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetH2_OnClick(object sender, RoutedEventArgs e)
        {
            InsertAtLineStart("## ");
        }



        /// <summary>
        /// 分割线
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MinusLineButton_OnClick(object sender, RoutedEventArgs e)
        {
            InsertTextAtCaret("\r\r --- \r\r");

        }

        /// <summary>
        /// 下标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubscriptButton_OnClick(object sender, RoutedEventArgs e)
        {
            InsertBeMarkDownCode("~");
        }

        /// <summary>
        /// 上标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void SuperscriptButton_OnClick(object sender, RoutedEventArgs e)
        {
            InsertBeMarkDownCode("^");
        }

        /// <summary>
        /// 插入表格
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TableButton_OnClick(object sender, RoutedEventArgs e)
        {
            InsertMarkdownTable(NumX.Value, NumY.Value);
        }

        private void FormatQuoteOpenButton_OnClick(object sender, RoutedEventArgs e)
        {
            InsertPrefixToEachSelectedLine("> ");
        }

        private void CodeJsonButton_OnClick(object sender, RoutedEventArgs e)
        {
            InsertBeMarkDownCode("\r```\r");
        }

        private void ListBulletedButton_OnClick(object sender, RoutedEventArgs e)
        {
            InsertPrefixToEachSelectedLine("- ");
        }

        private void FormatListNumberedButton_OnClick(object sender, RoutedEventArgs e)
        {
            InsertPrefixIndexToEachSelectedLine("- ");
        }

        /// <summary>
        /// 插入图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageButton_OnClick(object sender, RoutedEventArgs e)
        {
            InsertTextAtCaret($"![IMAGE]({ImageUrl.Text})\r");
        }

        private void SetH3_OnClick(object sender, RoutedEventArgs e)
        {
            InsertAtLineStart("### ");
        }

        private void SetH4_OnClick(object sender, RoutedEventArgs e)
        {
            InsertAtLineStart("#### ");
        }

        private void SetH5_OnClick(object sender, RoutedEventArgs e)
        {
            InsertAtLineStart("##### ");
        }




        private NoteBookViewModel noteBook = new NoteBookViewModel();

        private void NoteTree_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {


            if (NoteTree.SelectedItem is TreeItem selectedItem)
            {
                // 判断是否是叶子节点（没有子节点）
                if (selectedItem.Children == null || selectedItem.Children.Count == 0)
                {
                    // 是叶子节点 -> 它代表一个 BookName
                    if (selectedItem.Tag is NoteBookViewModel record)
                    {
                        noteBook = record;

                        // 假设你想显示 BookName + BookGroup + BookUnit 等信息
                        string detail = record.Note;

                        InputTextBox.Text = detail;


                        //查询数据库，获取对应笔记本的内容
                        var query =
                            $"SELECT * FROM NoteBook WHERE  NoteId = '{record.NoteId}' AND (Del != 1 OR Del IS NULL) LIMIT 1;";
                        var rows = GlobalVariables.DbService.ExecuteQuery(query);
                        if (rows.Count > 0)
                        {

                            InputTextBox.Text = rows[0]["Note"].ToString();


                        }
                        else
                        {
                            InputTextBox.Text = ""; //如果没有内容，清空 TextBox
                            NoteBookCount.Content = 0;
                        }








                        var path = string.Empty;
                        if (!string.IsNullOrWhiteSpace(record.BookUnit))
                        {
                             path = $"{record.BookGroup}/{record.BookUnit}/{record.BookName}";
                        }
                        else
                        {
                            path= $"{record.BookGroup}/{record.BookName}";
                        }

                        NoteBookName.Content = path;
                    }
                }

            }















            ////获取选中的 TreeViewItem
            //var selectedItem = NoteTree.SelectedItem as TreeViewItem;
            //if (selectedItem != null)
            //{
            //    //检查是否有子项节点，如果有则展开当前节点
            //    //如果没有子节点，则认为是具体的笔记本，加载内容
            //    if (selectedItem.Items.Count > 0)
            //    {
            //        selectedItem.IsExpanded = !selectedItem.IsExpanded; // 切换展开/收起状态
            //    }
            //    else
            //    {
            //        var node = selectedItem.Tag as NoteBookViewModel;

            //        InputTextBox.Text=node.Note;

            //        ////获取笔记本名称
            //        //dynamic noteTag = selectedItem.Tag;
            //        //string noteId = noteTag.NoteId;

            //        //NoteBookName.Content = noteTag.NoteName;


            //        ////查询数据库，获取对应笔记本的内容
            //        //var query =
            //        //    $"SELECT * FROM NoteBook WHERE  NoteId = '{noteId}' AND (Del != 1 OR Del IS NULL) LIMIT 1;";
            //        //var rows = GlobalVariables.DbService.ExecuteQuery(query);
            //        //if (rows.Count > 0)
            //        //{
            //        //    noteBook = rows[0];
            //        //    //显示内容到 TextBox
            //        //    InputTextBox.Text = rows[0]["Note"].ToString();

            //        //    NoteBookCount.Content = InputTextBox.Text.Length.ToString();
            //        //}
            //        //else
            //        //{
            //        //    InputTextBox.Text = ""; //如果没有内容，清空 TextBox
            //        //    NoteBookCount.Content = 0;
            //        //}
            //    }
            //}
        }

        private async void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {

                if (!string.IsNullOrWhiteSpace(noteBook.NoteId))
                {
                    var editDate = DateTime.Now;
                    var note = InputTextBox.Text;
                    var data = new
                    {
                        NoteId = noteBook.NoteId,
                        NoteGroup = noteBook.BookGroup,
                        NoteUnit = noteBook.BookUnit,
                        NoteName = noteBook.BookName,
                        CreatedDate = noteBook.CreatedDate,
                        EditDate = editDate,
                        Note = note
                    };

                    var conditions = new
                    {
                        NoteId = noteBook.NoteId
                    };

                    GlobalVariables.DbService.UpdateEntity("NoteBook", data, conditions);

                    SendMessage("已保存",1);

                

                }
                else
                {
                    MessageBox.Show("请先选择要编辑的笔记", "未选择笔记");
                }



        }



        /// <summary>
        /// 发送通知
        /// </summary>
        /// <param name="message"></param>
        /// <param name="time"></param>
        private void SendMessage(string message,int time)
        {
            var duration = time;

            SnackbarThree.MessageQueue?.Enqueue(
                message,
                null,
                null,
                null,
                true,
                true,
                TimeSpan.FromSeconds(duration));
        }


        private void InputTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            NoteBookCount.Content= InputTextBox.Text.Length.ToString();
        }

        private void NoteTree_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var treeView = sender as TreeView;
            var pos = e.GetPosition(treeView);
            var hit = treeView.InputHitTest(pos) as DependencyObject;

            var treeViewItem = GetAncestor<TreeViewItem>(hit);
            if (treeViewItem == null) return;

            // 获取绑定的数据项
            if (treeViewItem.Header is TreeItem item)
            {
                // 只有当该节点有子节点时，才手动处理展开/折叠
                if (item.Children != null && item.Children.Count > 0)
                {
                    // 阻止默认行为，手动切换展开状态
                    e.Handled = true;
                    treeViewItem.IsExpanded = !treeViewItem.IsExpanded;
                }
                // 如果是叶子节点（如 BookName），不处理展开，让系统处理选中
                // 即：不设置 e.Handled = true，允许事件继续传播
            }
        }

        static T GetAncestor<T>(DependencyObject obj) where T : class
        {
            while (obj != null && !(obj is T))
                obj = VisualTreeHelper.GetParent(obj);
            return obj as T;
        }

        private void SearchButton_OnClick(object sender, RoutedEventArgs e)
        {
            LoadNoteBooks(SearchKeyWord.Text);
        }

        private void ClearSearchKeyWord_OnClick(object sender, RoutedEventArgs e)
        {
            SearchKeyWord.Text=string.Empty;
            LoadNoteBooks();
        }

        private void SearchKeyWord_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key==Key.Enter)
            {
                LoadNoteBooks(SearchKeyWord.Text);
            }
        }
    }
}