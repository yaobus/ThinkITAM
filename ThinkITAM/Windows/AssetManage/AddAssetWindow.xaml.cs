using ThinkITAM.DatabaseOperation;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ThinkITAM.ViewModes.Preset;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using ThinkITAM.ViewModes.AssetManage;
using ThinkITAM.FunctionClass;

namespace ThinkITAM.Windows.AssetManage;
/// <summary>
/// AddAssetWindow.xaml 的交互逻辑
/// </summary>
public partial class AddAssetWindow : Window
{
    private AssetViewModel assetInfo;

    private int editMode = 0;//当前的编辑模式，默认为新增模式



    public AddAssetWindow(AssetViewModel? rowData)
    {
        InitializeComponent();


        if (rowData != null)//有信息传入，说明是修改模式
        {
            editMode = 1;   //修改模式

            assetInfo = rowData;

        }


    }

    private DbClass dbClass;

    private void AddAssetWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        string dbFilePath = AppDomain.CurrentDomain.BaseDirectory + @"db\Address_database.db";

        dbClass = new DbClass(dbFilePath);
        dbClass.OpenConnection();

        LoadTags();
        LoadAssetType();
        LoadOrganizationInfo();
        LoadAddress();

        if (editMode == 1)
        {
            this.DataContext = assetInfo;
        }
    }



    private ObservableCollection<AddressInfoViewModel> addressInfos = new ObservableCollection<AddressInfoViewModel>();


    private void LoadAddress()
    {
        addressInfos.Clear();

        string query = "SELECT * FROM Address;";

        SQLiteCommand command = new SQLiteCommand(query, dbClass.connection);
        SQLiteDataReader reader = command.ExecuteReader();

        int index = 0;

        while (reader.Read())
        {
            index++;
            AddressInfoViewModel info = new AddressInfoViewModel();

            info.Index = index;
            info.Location = reader["Location"].ToString();
            info.Note = reader["Note"].ToString();

            addressInfos.Add(info);
        }

        PresetAddress.ItemsSource = addressInfos;


    }


    private ObservableCollection<string> organizationInfo = new ObservableCollection<string>();

    /// <summary>
    /// 加载组织信息
    /// </summary>
    private void LoadOrganizationInfo()
    {
        organizationInfo.Clear();

        string query = "SELECT DISTINCT Organization FROM Organization;";

        SQLiteCommand command = new SQLiteCommand(query, dbClass.connection);
        SQLiteDataReader reader = command.ExecuteReader();


        while (reader.Read())
        {
            organizationInfo.Add(reader["Organization"].ToString());
        }

        UserOrganization.ItemsSource = organizationInfo;


    }





    /// <summary>
    /// 资产类型列表
    /// </summary>
    private ObservableCollection<string> assetTypeInfos = new ObservableCollection<string>();

    private void LoadAssetType()
    {

        assetTypeInfos.Clear();

        string query = "SELECT DISTINCT AssetType FROM AssetTag;";

        SQLiteCommand command = new SQLiteCommand(query, dbClass.connection);

        SQLiteDataReader reader = command.ExecuteReader();

        if (reader != null)
        {
            while (reader.Read())
            {
                assetTypeInfos.Add(reader["AssetType"].ToString());
            }
        }


        AssetType.ItemsSource = assetTypeInfos;

    }

    /// <summary>
    /// 资产类型列表
    /// </summary>
    private ObservableCollection<string> deviceTypeInfos = new ObservableCollection<string>();


    /// <summary>
    /// 资产类型被选择
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AssetType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (AssetType.SelectedIndex != -1)
        {
            deviceTypeInfos.Clear();

            AssetTag.Text = "";
            AssetNumber.Text = "";


            string query = $"SELECT  DeviceType FROM AssetTag WHERE AssetType='{assetTypeInfos[AssetType.SelectedIndex].ToString()}';";

            //Console.WriteLine(query);

            SQLiteCommand command = new SQLiteCommand(query, dbClass.connection);
            SQLiteDataReader reader = command.ExecuteReader();


            while (reader.Read())
            {
                deviceTypeInfos.Add(reader["DeviceType"].ToString());
            }

            DeviceType.ItemsSource = deviceTypeInfos;
        }
        else
        {
            deviceTypeInfos.Clear();
        }
    }


    /// <summary>
    /// 设备类型被选择
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DeviceType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DeviceType.SelectedIndex != -1)
        {
            AssetTag.Text = "";

            string query = $"SELECT  AssetTag FROM AssetTag WHERE AssetType='{assetTypeInfos[AssetType.SelectedIndex].ToString()}' AND DeviceType='{deviceTypeInfos[DeviceType.SelectedIndex].ToString()}';";

            //Console.WriteLine(query);

            SQLiteCommand command = new SQLiteCommand(query, dbClass.connection);
            SQLiteDataReader reader = command.ExecuteReader();

            string tag = "";
            while (reader.Read())
            {


                tag = reader["AssetTag"].ToString();

            }

            if (tag != "")
            {
                AssetTag.Text = tag;

                AssetNumber.Text = CalculateAssetId(tag).ToString();
            }
        }
    }



    /// <summary>
    /// 查询并创建资产编号
    /// </summary>
    private int CalculateAssetId(string assetTag)
    {
        //计算已有编号数量

        string sqlTemp = string.Format($"SELECT AssetNumber FROM Asset WHERE AssetTag ='{assetTag}'");
        SQLiteCommand command = new SQLiteCommand(sqlTemp, dbClass.connection);
        SQLiteDataReader reader = command.ExecuteReader();

        List<int> idList = new List<int>();

        while (reader.Read())
        {
            idList.Add(Convert.ToInt32(reader["AssetNumber"]));
        }


        return FindMissingNumber(idList);
    }


    /// <summary>
    /// 寻找遗失的数值
    /// </summary>
    /// <param name="numbers"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static int FindMissingNumber(List<int> numbers)
    {
        if (numbers == null || numbers.Count == 0)
        {
            Console.WriteLine("List cannot be null or empty");

        }
        else
        {
            int max = numbers.Max();

            for (int i = 1; i <= max; i++)
            {
                if (!numbers.Contains(i))
                {
                    return i;

                }
            }

        }


        return numbers.Count + 1;
    }




    private void SetButton_OnClick(object sender, RoutedEventArgs e)
    {
        AddAssetWindowSet set = new AddAssetWindowSet();


        //窗口放中间
        var window = Window.GetWindow(this);
        if (window != null)
        {
            set.Owner = window;
        }




        if (set.ShowDialog() == true)
        {

            LoadTags();

        }
    }

    /// <summary>
    /// 加载自定义标签
    /// </summary>
    private void LoadTags()
    {

        var tags = dbClass.LoadWindowTag("AddAsset");

        if (tags != null)
        {
            dynamic settings = JsonConvert.DeserializeObject(tags);

            LabelA.Content = settings.TagA + ":";
            LabelB.Content = settings.TagB + ":";
            LabelC.Content = settings.TagC + ":";
            LabelD.Content = settings.TagD + ":";
            LabelE.Content = settings.TagE + ":";
            LabelF.Content = settings.TagF + ":";
        }

    }



    //双击添加购买日期
    private void BuyDate_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        BuyDate.SelectedDate = DateTime.Today;


    }

    /// <summary>
    /// 双击添加报废日期
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ScrapDate_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        ScrapDate.SelectedDate = DateTime.Today;
    }


    private ObservableCollection<string> departmentInfo = new ObservableCollection<string>();


    /// <summary>
    /// 单位被选择
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void UserOrganization_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (UserOrganization.SelectedIndex != -1)
        {
            departmentInfo.Clear();

            string query = $"SELECT  Department FROM Organization WHERE Organization='{organizationInfo[UserOrganization.SelectedIndex].ToString()}';";

            Console.WriteLine(query);

            SQLiteCommand command = new SQLiteCommand(query, dbClass.connection);
            SQLiteDataReader reader = command.ExecuteReader();


            while (reader.Read())
            {
                departmentInfo.Add(reader["Department"].ToString());
            }

            UserDepartment.ItemsSource = departmentInfo;
        }
        else
        {
            departmentInfo.Clear();
        }

    }

    /// <summary>
    /// 责任人列表
    /// </summary>
    private ObservableCollection<PeopleViewModel> peopleInfo = new ObservableCollection<PeopleViewModel>();



    /// <summary>
    /// 部门被选择
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void UserDepartment_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (UserDepartment.SelectedIndex != -1)
        {
            peopleInfo.Clear();

            string query = $"SELECT * FROM UserInfo WHERE Organization='{organizationInfo[UserOrganization.SelectedIndex].ToString()}' AND Department='{departmentInfo[UserDepartment.SelectedIndex].ToString()}';";

            //Console.WriteLine(query);

            SQLiteCommand command = new SQLiteCommand(query, dbClass.connection);
            SQLiteDataReader reader = command.ExecuteReader();


            while (reader.Read())
            {
                var info = new PeopleViewModel();

                info.Name = reader["Name"].ToString();
                info.Phone = reader["Phone"].ToString();
                peopleInfo.Add(info);

            }


            AssignedTo.ItemsSource = peopleInfo;
        }
    }

    /// <summary>
    /// 责任人被选择
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AssignedTo_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (AssignedTo.SelectedIndex != -1)
        {
            Phone.Text = peopleInfo[AssignedTo.SelectedIndex].Phone;
            Consumer.Text = peopleInfo[AssignedTo.SelectedIndex].Name;
        }
    }



    /// <summary>
    /// 计算已用年限
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BuyDate_OnSelectedDateChanged(object? sender, SelectionChangedEventArgs e)
    {
        DateTime nowDate = DateTime.Now;

        var buyDate = (DateTime)BuyDate.SelectedDate;




        int x;

        if (ScrapDate.SelectedDate != null)
        {

            x = ((DateTime)ScrapDate.SelectedDate).Year - buyDate.Year;
        }
        else
        {
            x = nowDate.Year - buyDate.Year;
        }

        ServiceLife.Text = x.ToString();
    }



    /// <summary>
    /// 保存资产信息
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SaveButton_OnClick(object sender, RoutedEventArgs e)
    {
        var info = CheckInput();

        if (info.Item1 > 0)
        {
            MessageBox.Show(info.Item2, $"当前存在以下{info.Item1}项问题需要注意", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        else//可以存储
        {

            //判断是新建模式还是编辑模式
            if (editMode == 1)//进入编辑模式
            {


                string sql = $"UPDATE \"Asset\" SET  \"AssetType\" = '{AssetType.Text}',\r\n    \"DeviceType\" = '{DeviceType.Text}',\r\n    \"AssetTag\" = '{AssetTag.Text}',\r\n    \"AssetNumber\" = {Convert.ToInt32(AssetNumber.Text)},\r\n    \"PurchaseDate\" = '{BuyDate.SelectedDate.ToString()}',\r\n    \"PurchasePrice\" = '{Price.Text}',\r\n    \"Manufacturer\" = '{Maker.Text}',\r\n    \"Model\" = '{Model.Text}',\r\n    \"SerialNumber\" = '{SerialNumber.Text}',\r\n    \"Configuration\" = '{Parameter.Text}',\r\n    \"Location\" = '{PresetAddress.Text}',\r\n    \"UserOrganization\" = '{UserOrganization.Text}',\r\n    \"UserDepartment\" = '{UserDepartment.Text}',\r\n    \"User\" = '{AssignedTo.Text}',\r\n    \"UserPhone\" = '{Phone.Text}',\r\n    \"Consumer\" = '{Consumer.Text}',\r\n    \"Status\" = '{AssetStatus.Text}',\r\n    \"UsedYear\" = '{ServiceLife.Text}',\r\n    \"ScrapDate\" = '{ScrapDate.SelectedDate.ToString()}',\r\n    \"Notes\" = '{Description.Text}',\r\n    \"TagA\" = '{TagA.Text}',\r\n    \"TagB\" = '{TagB.Text}',\r\n    \"TagC\" = '{TagC.Text}',\r\n    \"TagD\" = '{TagD.Text}',\r\n    \"TagE\" = '{TagE.Text}',\r\n    \"TagF\" = '{TagF.Text}'\r\nWHERE\r\n    \"AssetId\" = '{assetInfo.AssetId}';";

               

                dbClass.ExecuteQuery(sql);

            }
            else//进入新建模式
            {

                //创建资产ID

                //创建资产ID字符串，0为机房，1为机柜，2为设备,3为机架
                string assetId = $"2{AssetCodeClass.GenerateChecksum(AssetIdCreate.CreateAssetId( DateTime.Now.ToString("yyyyMMddHHmmss"))).ToUpper()}";


                //创建资产二维码,0为机房，1为机柜，2为设备
                string qrCode = "ITAM:" + FunctionClass.AssetCodeClass.GenerateChecksum(assetId).ToUpper();

                string sql =
                    $"INSERT INTO \"Asset\" (\"AssetId\",\"AssetQrCode\",\"AssetType\", \"DeviceType\", \"AssetTag\", \"AssetNumber\", \"PurchaseDate\", \"PurchasePrice\", \"Manufacturer\", \"Model\", \"SerialNumber\", \"Configuration\", \"Location\", \"UserOrganization\", \"UserDepartment\", \"User\", \"UserPhone\", \"Consumer\", \"Status\", \"UsedYear\", \"ScrapDate\", \"Notes\", \"TagA\", \"TagB\", \"TagC\", \"TagD\", \"TagE\", \"TagF\") VALUES ('{assetId}','{qrCode}','{AssetType.Text}', '{DeviceType.Text}', '{AssetTag.Text}', {Convert.ToInt32(AssetNumber.Text)}, '{BuyDate.SelectedDate.ToString()}', '{Price.Text}', '{Maker.Text}', '{Model.Text}', '{SerialNumber.Text}', '{Parameter.Text}', '{PresetAddress.Text}', '{UserOrganization.Text}', '{UserDepartment.Text}', '{AssignedTo.Text}', '{Phone.Text}', '{Consumer.Text}', '{AssetStatus.Text}', '{ServiceLife.Text}', '{ScrapDate.SelectedDate.ToString()}', '{Description.Text}', '{TagA.Text}', '{TagB.Text}', '{TagC.Text}', '{TagD.Text}', '{TagE.Text}', '{TagF.Text}')";

              

                dbClass.ExecuteQuery(sql);
            }


            this.DialogResult = true;
            this.Close();
        }




    }

    /// <summary>
    /// 创建资产ID
    /// </summary>
    /// <returns></returns>
    private string CreateAssetId(string assetTagNumber)
    {
        string str = DateTime.Now.ToString() + assetTagNumber;

        return EncryptionDecryption.EncryptionDecryption.CalculateMD5(str).ToUpper();

    }

    private (int, string) CheckInput()
    {
        int index = 0;
        string message = "当前存在以下问题需要解决:\r";

        if (AssetType.SelectedIndex == -1)
        {
            index++;
            message += index.ToString() + ":未选择资产类型\r";
        }


        if (DeviceType.SelectedIndex == -1)
        {
            index++;
            message += index.ToString() + ":未选择设备类型\r";
        }

        Regex regex = new Regex(@"^\d+$");

        if (!regex.IsMatch(AssetNumber.Text))
        {
            index++;
            message += index.ToString() + ":资产编号必须为整数数字\r";
        }



        if (Model.Text.Replace(" ", "").Length < 2)
        {
            index++;
            message += index.ToString() + ":设备型号太短\r";
        }


        if (AssetStatus.SelectedIndex == -1)
        {
            index++;
            message += index.ToString() + ":未选择资产设备状态\r";
        }

        return (index, message);
    }


}
