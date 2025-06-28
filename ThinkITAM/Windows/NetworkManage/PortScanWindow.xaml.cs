using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;
using ThinkITAM.DatabaseOperation;
using ThinkITAM.DataBridge;
using ThinkITAM.FunctionClass;
using ThinkITAM.Functions.Converters;
using ThinkITAM.Functions.FunctionClass;
using ThinkITAM.UserControls.General;
using ThinkITAM.ViewModels.Others;

namespace ThinkITAM.Windows.NetworkManage
{
    /// <summary>
    /// PortScanWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PortScanWindow : Window
    {
        public PortScanWindow(string hosts)
        {
            InitializeComponent();
            HostStatusDataGrid.ItemsSource = _hostResults;
            DataContext = this;
            _hostScannerHelper = new HostScannerHelper(_hostResults, UpdateProgress, Application.Current.Dispatcher);
            HostTextBox.Text = hosts;
        }


        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            _hostResults.Clear();
            // 主机列表
            var ipList = IPParserHelper.ParseIPAddresses(HostTextBox.Text);

            if (ipList == null)
            {
                PortScannerHelper.ShowMessageDialog("地址输入有误", $"无效的IP地址或格式");
                return;
            }

            // 端口列表
            var portList = PortScannerHelper.ParsePortRanges(PortTextBox.Text);

            if (portList == null)
            {
                PortScannerHelper.ShowMessageDialog("端口输入有误", $"无效的端口范围或格式");
                return;
            }

            bool allTest = (bool)AllTestRadioButton.IsChecked;

            bool hostnameScan = (bool)HostNameCheckBox.IsChecked;

            int time = 1000;

            try
            {
                time = int.Parse(TimeOutBox.Text);
            }
            catch (Exception exception)
            {
                TimeOutBox.Text = "1000";
                time = 1000;
            }


            if (ipList.Count <= DataBridge.GlobalLimit.ScanHostNumber)
            {
                if (ipList.Count > 1) // 扫描数量大于1
                {
                    var dialog = new ConfirmationDialog
                    {
                        Title = "危险操作警告！",
                        Prompt = "警告：执行大范围端口扫描可能会导致网络性能下降、触发安全警报、违反法律法规及服务条款，甚至引发法律诉讼。请确保您拥有相应权限，并谨慎操作。",
                        ConfirmButtonText = "我已知晓并愿意承担相关风险",
                        TitleColor = ColorConverterClass.ColorToBrush("#CF3539"),
                        PromptColor = ColorConverterClass.ColorToBrush("#CF3539"),
                        ConfirmButtonColor = ColorConverterClass.ColorToBrush("#CF3539")
                    };



                    // 假设你的 DialogHost 在 XAML 中定义，并且 x:Name 设置为 "RootDialogHost"
                    var dialogHost = DialogHost.GetDialogSession("MessageDialogHost"); // 或者直接引用你的 DialogHost, 如: this.RootDialogHost

                    if (dialogHost != null)
                    {
                        // 如果 DialogHost 已经打开，则先关闭它
                        dialogHost.Close();
                        // 等待一小段时间以确保对话框已关闭，如果必要的话
                        // await Task.Delay(100);  // 这可能不需要，视具体情况而定
                    }

                    // 显示对话框并获取结果
                    var result = await DialogHost.Show(dialog, "MessageDialogHost");

                    if (result != null && (bool)result)
                    {
                        await StartHostScan(allTest, hostnameScan, ipList, portList);
                    }
                    else
                    {
                        // 用户点击了取消按钮或关闭了对话框
                    }
                }
                else // 扫描数量等于1
                {
                    await StartHostScan(allTest, hostnameScan, ipList, portList);
                }
            }
            else
            {
                var dialog = new ConfirmationDialog
                {
                    Title = "扫描限制",
                    Prompt = $"单次扫描限制最多{DataBridge.GlobalLimit.ScanHostNumber}个主机",
                    ConfirmButtonText = "确认",

                };

                // 显示对话框
                await DialogHost.Show(dialog, "MessageDialogHost");
            }
        }




        /// <summary>
        /// 启动扫描
        /// </summary>
        private async Task StartHostScan(bool allTest, bool hostnameScan, List<string> ipList, List<int> portList, int timeOut = 1000)
        {
            if (allTest == true) // 测试全部主机
            {
                await _hostScannerHelper.CheckPortsAsync(ipList, portList, hostnameScan, true, timeOut);
            }
            else // 测试能Ping通的主机
            {
                List<string> ableIp = await PingTesterClass.GetPingableIpsAsync(ipList);

                if (ableIp.Count > 0)
                {
                    await _hostScannerHelper.CheckPortsAsync(ableIp, portList, hostnameScan, true, timeOut);
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {

                        PortScannerHelper.ShowMessageDialog("无Ping通主机", $"所有主机共:{ipList.Count}个，均无法Ping通，可能是主机本身不在线或者是防火墙等原因导致无法Ping通,无法Ping通并不代表该主机一定不在线。");

                    });
                }
            }
        }


        private void UpdateProgress(int current, int total)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                double progressPercentage = (double)current / total * 100;
                ScanMainProgressBar.Value = progressPercentage;
            });


        }



        /// <summary>
        /// 启动扫描
        /// </summary>
        private async void StartScan(bool allTest, List<string> ipList, List<int> portList)
        {
            if (allTest == true)//测试全部主机
            {
                var progressBar = new Progress<int>(progressValue =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        ScanMainProgressBar.Value = progressValue; // 更新ProgressBar的值
                    });
                });

                var results = await PortScanner.CheckPortsAsync(ipList, portList, progressBar);

                foreach (var result in results)
                {
                    Console.WriteLine(result.Host + ":" + result.Port + ":" + result.IsOpen);
                }
            }
            else//测试能Ping通的主机
            {
                List<string> ableIp = await PingTesterClass.GetPingableIpsAsync(ipList);


                if (ableIp.Count > 0)
                {
                    var progressBar = new Progress<int>(progressValue =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            ScanMainProgressBar.Value = progressValue; // 更新ProgressBar的值
                        });
                    });

                    var results = await PortScanner.CheckPortsAsync(ableIp, portList, progressBar);

                    foreach (var result in results)
                    {
                        Console.WriteLine(result.Host + ":" + result.Port + ":" + result.IsOpen);
                    }
                }
                else
                {
                    MessageBox.Show($"所有主机共:{ipList.Count}个，均无法Ping通，可能是防火墙设置等原因导致", "无Ping通主机", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }




        }


        private ObservableCollection<HostCheckResult> _hostResults = new ObservableCollection<HostCheckResult>();
        private HostScannerHelper _hostScannerHelper;

        public ObservableCollection<HostCheckResult> HostResults
        {
            get => _hostResults;
            set => _hostResults = value;
        }



        /// <summary>
        /// 主机输入帮助
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void HostHelp_OnClick(object sender, RoutedEventArgs e)
        {

            var dialog = new ConfirmationDialog
            {
                Title = "主机输入帮助",
                Prompt = $"在对话框输入要检测的IP地址，多个主机地址请用逗号或空格隔开，支持IP段输入(如:192.168.0.1-192.168.0.9)，支持CIDR格式输入(如:192.168.0.0/28)，支持单个主机、网段、和CIDR混合输入。",
                ConfirmButtonText = "确认",


            };

            // 显示对话框
            await DialogHost.Show(dialog, "MessageDialogHost");



        }

        /// <summary>
        /// 端口输入帮助
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void PortHelp_OnClick(object sender, RoutedEventArgs e)
        {

            var dialog = new ConfirmationDialog
            {
                Title = "端口输入帮助",
                Prompt = $"在对话框输入要检测的端口，多个端口请用逗号或空格隔开，支持端口范围输入(如:100-200)，支持单个端口、端口范围混合输入。",
                ConfirmButtonText = "确认",


            };

            // 显示对话框
            await DialogHost.Show(dialog, "MessageDialogHost");




        }



        private void PortScanWindow_OnLoaded(object sender, RoutedEventArgs e)
        {

            PortPresetComboBox.ItemsSource = scanPorts;

            //HostPortStatusDataGrid.ItemsSource = hostPortStatusList;

            //加载端口组预设方案
            LoadScanPortsPreset();


        }

        private ObservableCollection<ViewModels.Preset.ScanPortsViewModel> scanPorts =
            new ObservableCollection<ViewModels.Preset.ScanPortsViewModel>();

        private void LoadScanPortsPreset()
        {
            scanPorts.Clear();
            string sql = $"SELECT * FROM ScanPorts";


            var rows = GlobalVariables.DbService.ExecuteQuery(sql);

            foreach (var row in rows)
            {
                var ports = new ViewModels.Preset.ScanPortsViewModel();

                ports.Name = row["Name"].ToString();
                ports.Ports = row["Ports"].ToString();

                scanPorts.Add(ports);
            }


        }

        private void PortPresetComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PortPresetComboBox.SelectedIndex != -1)
            {
                //WebPortRadioButton.IsChecked = false;
                PresetRadioButton.IsChecked = true;
                PortTextBox.Text = scanPorts[PortPresetComboBox.SelectedIndex].Ports;
            }


        }


        private void SavePresetButton_OnClick(object sender, RoutedEventArgs e)
        {
            SavePresetDialogHost.IsOpen = true;

            // 直接调用 DialogHost 的 IsOpen 属性来打开对话框
            //var dialogHost = (DialogHost)FindName("SavePresetDialogHost"); // 确保替换为你的 DialogHost 实际的名字
            //if (dialogHost != null)
            //{
            //    // 打开对话框
            //    dialogHost.IsOpen = true;
            //}
            //else
            //{
            //    MessageBox.Show("无法找到 DialogHost 控件。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            //}


        }

        /// <summary>
        /// 端口组预设方案保存对话框关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventargs"></param>
        private void DialogHost_OnDialogClosed(object sender, DialogClosedEventArgs eventargs)
        {
            LoadScanPortsPreset();
        }


        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {

            string name = NameTextBox.Text;

            if (name.Replace(" ", "").Length > 0)
            {

                //查询名称是否存在
                string sqlTemp = $"SELECT COUNT(*) FROM ScanPorts WHERE Name ='{name}'";

                var countNum = DbClass.ExecuteScalarTableNum(sqlTemp);

                if (countNum >= 1)
                {

                    PortScannerHelper.ShowMessageDialog("方案名称重复", $"该方案名称{name}已存在，请重新输入");

                }
                else
                {
                    var portList = PortScannerHelper.ParsePortRanges(PortTextBox.Text);
                    if (portList.Count > 0)
                    {
                        var info = new { Name = name, Ports = PortTextBox.Text };

                        //string sql = $"INSERT INTO ScanPorts (Name,Ports) VALUES ('{name}','{PortTextBox.Text}')";

                        GlobalVariables.DbService.InsertEntity("ScanPorts", info);
                        CancelButton_OnClick(null, null);
                    }
                    else
                    {
                        MessageBox.Show($"端口内容不得为空", "输入错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        CancelButton_OnClick(null, null);
                    }




                }

            }
            else
            {
                MessageBox.Show("方案名称不得为空", "输入错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }



        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {

            SavePresetDialogHost.IsOpen = false;


        }


        /// <summary>
        /// 接受风险提示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AcceptButton_OnClick(object sender, RoutedEventArgs e)
        {
            RejectButton_OnClick(null, null);

            //主机列表
            var ipList = IPParserHelper.ParseIPAddresses(HostTextBox.Text);


            //端口列表
            var portList = PortScannerHelper.ParsePortRanges(PortTextBox.Text);

            //全部主机一起测试
            if (AllTestRadioButton.IsChecked == true)
            {
                var progressBar = new Progress<int>(progressValue =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        ScanMainProgressBar.Value = progressValue; // 更新ProgressBar的值
                    });
                });

                var results = await PortScanner.CheckPortsAsync(ipList, portList, progressBar);

                foreach (var result in results)
                {
                    Console.WriteLine(result.Host + ":" + result.Port + ":" + result.IsOpen);
                }

            }

            //仅测试能ping的主机
            if (PingTestRadioButton.IsChecked == true)
            {

                List<string> ableIp = await PingTesterClass.GetPingableIpsAsync(ipList);


                if (ableIp.Count > 0)
                {
                    var progressBar = new Progress<int>(progressValue =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            ScanMainProgressBar.Value = progressValue; // 更新ProgressBar的值
                        });
                    });

                    var results = await PortScanner.CheckPortsAsync(ableIp, portList, progressBar);

                    foreach (var result in results)
                    {
                        Console.WriteLine(result.Host + ":" + result.Port + ":" + result.IsOpen);
                    }
                }
                else
                {
                    MessageBox.Show($"所有主机共:{ipList.Count}个，均无法Ping通，可能是防火墙设置等原因导致", "无Ping通主机", MessageBoxButton.OK, MessageBoxImage.Error);
                }


            }

        }

        /// <summary>
        /// 拒绝接受风险提示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RejectButton_OnClick(object sender, RoutedEventArgs e)
        {
            // 直接调用 DialogHost 的 IsOpen 属性来打开对话框
            var dialogHost = (DialogHost)FindName("ScanWarnDialogHost");
            if (dialogHost != null)
            {
                // 打开对话框
                dialogHost.IsOpen = false;
            }
        }

        private void SshPortRadioButton_OnClick(object sender, RoutedEventArgs e)
        {
            PortTextBox.Text = $"22,23";
        }

        private void WebPortRadioButton_OnClick(object sender, RoutedEventArgs e)
        {
            PortTextBox.Text = $"80,443";
        }

        private void DbPortRadioButton_OnClick(object sender, RoutedEventArgs e)
        {
            PortTextBox.Text = $"3306,3307";
        }

        private void RdpPortRadioButton_OnClick(object sender, RoutedEventArgs e)
        {
            PortTextBox.Text = $"3389";
        }
    }



}
