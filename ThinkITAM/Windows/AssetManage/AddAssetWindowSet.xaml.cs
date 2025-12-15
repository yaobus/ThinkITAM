using System.Windows;
using Newtonsoft.Json;
using ThinkITAM.DatabaseOperation;

namespace ThinkITAM.Windows.AssetManage;
/// <summary>
/// AddAssetWindowSet.xaml 的交互逻辑
/// </summary>
public partial class AddAssetWindowSet : Window
{
    public AddAssetWindowSet()
    {
        InitializeComponent();
    }



    private void AddAssetWindowSet_OnLoaded(object sender, RoutedEventArgs e)
    {


        var tags = DbClass.LoadWindowTag("AddAsset");

        if (tags != null)
        {
            dynamic settings = JsonConvert.DeserializeObject(tags);
            if (settings != null)
            {
                TagA.Text = settings.TagA;
                TagB.Text = settings.TagB;
                TagC.Text = settings.TagC;
                TagD.Text = settings.TagD;
                TagE.Text = settings.TagE;
                TagF.Text = settings.TagF;
            }
        }
    }
    private void SaveButton_OnClick(object sender, RoutedEventArgs e)
    {
        var settings = new
        {
            TagA = TagA.Text,    // 自定义标签1
            TagB = TagB.Text,    // 自定义标签2
            TagC = TagC.Text,    // 自定义标签3
            TagD = TagD.Text,    // 自定义标签4
            TagE = TagE.Text,    // 自定义标签5
            TagF = TagF.Text,    // 自定义标签6
        };

        // 将匿名对象序列化为JSON字符串
        string json = JsonConvert.SerializeObject(settings);

        DbClass.SaveWindowTag("AddAsset", json);

        this.DialogResult = true;
        this.Close();
    }


}
