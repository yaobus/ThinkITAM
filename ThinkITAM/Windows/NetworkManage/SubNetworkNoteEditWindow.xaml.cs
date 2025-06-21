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
using System.Windows.Shapes;
using DocumentFormat.OpenXml.ExtendedProperties;
using ThinkITAM.DataBridge;
using ThinkITAM.ViewModels.NetworkManage;

namespace ThinkITAM.Windows.NetworkManage;
/// <summary>
/// SubNetworkNoteEditWindow.xaml 的交互逻辑
/// </summary>
public partial class SubNetworkNoteEditWindow : Window
{
    public SubNetworkNoteEditWindow(SubNetworkInfoViewModel subNetworkInfo)
    {
        InitializeComponent();
        this.DataContext = subNetworkInfo;
        info= subNetworkInfo;
    }

    private SubNetworkInfoViewModel info=null;
    private void SubNetworkNoteEditWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        
    }

    private void CancelButton_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }

    private void SaveButton_OnClick(object sender, RoutedEventArgs e)
    {
        var sql = $"SELECT COUNT(*) FROM Notes WHERE NoteId='{info.TableName}'";

        var count =Convert.ToInt32( GlobalVariables.DbService.ExecuteScalar(sql));

        if (count == 0)//INSERT
        {
            var data = new { NoteId = info.TableName, Note = NoteTextBox.Text };

            GlobalVariables.DbService.InsertEntity("Notes",data);


        }
        else//UPDATE
        {
            var data = new { NoteId = info.TableName, Note = NoteTextBox.Text };
            var conditions = new { NoteId = info.TableName };

            GlobalVariables.DbService.UpdateEntity("Notes", data,conditions);
        }
        DialogResult = true;

    }
}
