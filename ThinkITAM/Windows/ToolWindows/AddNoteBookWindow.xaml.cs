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
            var query = $"SELECT DISTINCT NoteUnit FROM NoteBook WHERE (Del != 1 OR Del IS NULL ) AND NoteGroup='{group}';";

            

            var rows = GlobalVariables.DbService.ExecuteQuery(query);

            foreach (var row in rows)
            {
                if (!string.IsNullOrWhiteSpace(row["NoteUnit"].ToString()))
                {
                    noteUnits.Add(row["NoteUnit"].ToString());
                }

            }

           
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
                        EditDate= DateTime.Now.ToString()

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
                        CreatedDate = date

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
}
