using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Policy;
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
using DocumentFormat.OpenXml.Drawing.Charts;
using ThinkITAM.DataBridge;
using ThinkITAM.Functions.FunctionClass;
using ThinkITAM.ViewModels.Others;

namespace ThinkITAM.Windows.ToolWindows;
/// <summary>
/// AndNoteBookWindow.xaml 的交互逻辑
/// </summary>
public partial class AddNoteBookWindow : Window
{
    public AddNoteBookWindow(NoteBookViewModel inputNoteBookViewModel = null)
    {
        InitializeComponent();

        if (inputNoteBookViewModel != null)
        {
            noteBookViewModel = inputNoteBookViewModel;
        }
    }
    private NoteBookViewModel noteBookViewModel = null;

    private void AddNoteBookWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        LoadNoteBookName();

        NoteBookName.ItemsSource = noteGroups;
        UnitsCombobox.ItemsSource = noteUnits;

        if (noteBookViewModel != null)
        {
            TitleTextBlock.Text = "修改笔记";
            NoteBookName.SelectedItem = noteBookViewModel.NoteGroup;
            UnitsCombobox.SelectedItem = noteBookViewModel.NoteUnit;
            NoteName.Text = noteBookViewModel.NoteName;
            DisplayOrder.Text = noteBookViewModel.DisplayOrder.ToString();
        }


    }

    private ObservableCollection<string> noteGroups = new ObservableCollection<string>();

    /// <summary>
    /// 加载笔记本名称
    /// </summary>
    private void LoadNoteBookName()
    {
        var query = $"SELECT DISTINCT NoteGroup FROM NoteBook WHERE Del != 1 OR Del IS NULL ;";
        var rows = GlobalVariables.DbService.ExecuteQuery(query);
        noteGroups.Clear();
        foreach (var row in rows)
        {
            if (!string.IsNullOrWhiteSpace(row["NoteGroup"].ToString()))
            {
                noteGroups.Add(row["NoteGroup"].ToString());
            }

        }

    }



    private ObservableCollection<string> noteUnits = new ObservableCollection<string>();

    private void NoteBookName_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
       

        noteUnits.Clear();

        string group = noteGroups[NoteBookName.SelectedIndex];

        if (!string.IsNullOrWhiteSpace(group))
        {
            var query = $"SELECT * FROM NoteBook WHERE (Del != 1 OR Del IS NULL ) AND NoteGroup='{group}';";

            

            var rows = GlobalVariables.DbService.ExecuteQuery(query);
            int order = 0;
            foreach (var row in rows)
            {
                if (!string.IsNullOrWhiteSpace(row["NoteUnit"].ToString()))
                {
                    noteUnits.Add(row["NoteUnit"].ToString());
                }
               
              

                try
                {
                    order = Convert.ToInt32(row["DisplayOrder"]);
                }
                catch (Exception )
                {
                    // ignored
                }
               
            }

            DisplayOrder.Text = order.ToString();
        }



    }

    private void SaveButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(NoteBookName.Text))
        {
           
            if (!string.IsNullOrWhiteSpace(NoteName.Text))
            {
                if (noteBookViewModel != null)
                {


                    var updateNote = new
                    {
                        NoteId = noteBookViewModel.NoteId,
                        NoteGroup = NoteBookName.Text,
                        NoteUnit = UnitsCombobox.Text,
                        NoteName = NoteName.Text,
                        CreatedDate = noteBookViewModel.CreatedDate,
                        EditDate= DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss"),
                        DisplayOrder = string.IsNullOrWhiteSpace(DisplayOrder.Text) ? 0 : int.Parse(DisplayOrder.Text)

                    };

                    var conditions = new {NoteId = noteBookViewModel.NoteId };

                    GlobalVariables.DbService.UpdateEntity("NoteBook", updateNote, conditions);

                    this.DialogResult=true;
                }
                else
                {
                    var date = DateTime.Now.ToString();

                    string NoteId = $"7{AssetCodeClass.GenerateChecksum(AssetIdCreate.CreateAssetId(Guid.NewGuid().ToString())).ToUpper()}";

                    var note = new
                    {
                        NoteId = NoteId,
                        NoteGroup = NoteBookName.Text,
                        NoteUnit = UnitsCombobox.Text,
                        NoteName = NoteName.Text,
                        CreatedDate = date,
                        DisplayOrder = string.IsNullOrWhiteSpace(DisplayOrder.Text) ? 0 : int.Parse(DisplayOrder.Text)

                    };
                    GlobalVariables.DbService.InsertEntity("NoteBook", note);
                    this.DialogResult = true;
                }





            }
            else
            {
                MessageBox.Show("笔记名称不得为空","缺少必要信息");
            }




        }
    }

    /// <summary>
    /// 限制输入的数据类型
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DisplayOrder_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        // 只允许数字字符
        if (!char.IsDigit(e.Text, e.Text.Length - 1))
        {
            e.Handled = true; // 阻止非数字输入
        }
    }


    private void DisplayOrder_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        TextBox textBox = sender as TextBox;
        string text = textBox.Text;

        // 移除所有非数字字符
        string cleanedText = new string(text.Where(char.IsDigit).ToArray());

        // 如果文本被修改了，更新 TextBox
        if (text != cleanedText)
        {
            textBox.Text = cleanedText;
            // 将光标移动到文本末尾
            textBox.CaretIndex = cleanedText.Length;
        }
    }
}
