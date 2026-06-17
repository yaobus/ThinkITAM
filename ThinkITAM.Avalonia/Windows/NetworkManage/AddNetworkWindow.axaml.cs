using System.Net;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using ThinkITAM.DataBridge;
using ThinkITAM.Database;
using ThinkITAM.Functions.FunctionClass;
using ThinkITAM.Shared.Network;

namespace ThinkITAM.Windows.NetworkManage;

public partial class AddNetworkWindow : Window
{
    public bool LoadStatus = false;

    public AddNetworkWindow()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object? sender, RoutedEventArgs e)
    {
        LoadStatus = true;
        MaskSlider.ValueChanged += MaskSlider_OnValueChanged;
        IpTextBox.TextChanged += IpTextBox_OnTextChanged;
    }

    private void IpTextBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (LoadStatus) UpdateIPCalculations();
    }

   private void MaskSlider_OnValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (LoadStatus)
        {
            MaskLength.Text = ((int)e.NewValue).ToString();
            UpdateIPCalculations();
        }
    }

    private void UpdateIPCalculations()
    {
        try
        {
            if (!IPAddress.TryParse(IpTextBox.Text, out var ip)) return;

            int maskLength = (int)MaskSlider.Value;
            var mask = IPAddressCalculations.SubnetMaskFromPrefixLength(maskLength);
            Netmask.Text = mask.ToString();

            var networkAddress = ip.GetNetworkAddress(mask);
            Network.Text = networkAddress.ToString();

            var firstAddress = networkAddress.GetFirstUsable(ip.AddressFamily);
            First.Text = firstAddress.ToString();

            var lastAddress = networkAddress.GetLastUsable(ip.AddressFamily, maskLength);
            Last.Text = lastAddress.ToString();

            var broadcastAddress = networkAddress.GetBroadcastAddress(maskLength);
            Broadcast.Text = broadcastAddress.ToString();

            long addressCount = IPAddressCalculations.AddressCount(maskLength);
            NumBox.Text = addressCount.ToString();
        }
        catch { }
    }

    private void CancelButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void SaveButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TbName.Text))
        {
            ShowError("请输入网段名称");
            return;
        }

        if (string.IsNullOrWhiteSpace(IpTextBox.Text))
        {
            ShowError("请输入IP地址");
            return;
        }

        if (!IPAddress.TryParse(IpTextBox.Text, out var ip))
        {
            ShowError("IP地址格式不正确");
            return;
        }

        if (string.IsNullOrWhiteSpace(Network.Text))
        {
            ShowError("无法计算网络地址，请检查IP和子网掩码");
            return;
        }

        // 检查网段地址是否重复
        string checkSql = $"SELECT COUNT(*) FROM Network WHERE Network='{Network.Text}' AND (Del != 1 OR Del IS NULL)";
        if (Convert.ToInt32(GlobalVariables.DbService.ExecuteScalar(checkSql)) > 0)
        {
            ShowError("该网段地址已存在，请勿重复添加");
            return;
        }

        try
        {
            string networkId = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string tableName = "Net_" + networkId;

            // 插入网段信息
            string sql = $@"INSERT INTO Network (NetworkId, Name, Description, Network, Netmask, SortIndex, TagA, TagB)
                VALUES ('{networkId}', '{TbName.Text}', '{Description.Text ?? ""}', '{Network.Text}',
                        '{Netmask.Text}', {SortIndex.Text ?? "0"}, '{TagA.Text ?? ""}', '{TagB.Text ?? ""}')";

            GlobalVariables.DbService.ExecuteNonQuery(sql);

            // 创建对应IP地址表
            int maskLength = (int)MaskSlider.Value;
            int addressCount = (int)IPAddressCalculations.AddressCount(maskLength);

            string createSql = $@"CREATE TABLE IF NOT EXISTS {tableName} (
                Address INTEGER, FullAddress TEXT, AddressStatus INTEGER DEFAULT 0,
                AddressColor INTEGER DEFAULT 0, User TEXT, HostName TEXT,
                MacAddress TEXT, LinkDevice TEXT,
                TagA TEXT, TagB TEXT, TagC TEXT, TagD TEXT, TagE TEXT, TagF TEXT)";

            GlobalVariables.DbService.ExecuteNonQuery(createSql);

            // 预填充IP地址记录
            var baseIp = IPAddress.Parse(Network.Text);
            var baseBytes = baseIp.GetAddressBytes();
            Array.Reverse(baseBytes);
            uint baseAddr = System.BitConverter.ToUInt32(baseBytes, 0);

            for (int i = 1; i < addressCount - 1 && i <= 65536; i++)
            {
                uint addr = baseAddr + (uint)i;
                byte[] addrBytes = System.BitConverter.GetBytes(addr);
                Array.Reverse(addrBytes);
                string fullAddr = new IPAddress(addrBytes).ToString();

                string insertSql = $"INSERT INTO {tableName} (Address, FullAddress) VALUES ({i}, '{fullAddr}')";
                GlobalVariables.DbService.ExecuteNonQuery(insertSql);
            }

            var mainWindow = Owner as Window;
            Close(true);
        }
        catch (Exception ex)
        {
            ShowError($"保存网段失败: {ex.Message}");
        }
    }

    private async void ShowError(string message)
    {
        await Services.DialogService.ShowError(message);
    }
}
