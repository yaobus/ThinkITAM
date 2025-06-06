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
using ThinkITAM.UserControls.General;
using ThinkITAM.ViewModels.NetworkManage;
using ThinkITAM.ViewModels.Others;
using MaterialDesignThemes.Wpf;
using ThinkITAM.Functions.FunctionClass;


namespace ThinkITAM.Windows.ToolWindows
{
    /// <summary>
    /// StorageCalculationWindow.xaml 的交互逻辑
    /// </summary>
    public partial class StorageCalculationWindow : Window
    {
        public StorageCalculationWindow()
        {
            InitializeComponent();
            LoadData();
            RaidComboBox.ItemsSource = raidList;
            UnitBox.ItemsSource = unitList;

            CodingTypeComboBox.ItemsSource = codingTypeList;
            ResolutionComboBox.ItemsSource = resolutionList;
            CameraDataGrid.ItemsSource = storageCalculatorList;
            storageCalculatorList.CollectionChanged += StorageCalculatorList_CollectionChanged;
            BitRateComboBox.ItemsSource = bitRatelist;

            CameraDataGrid2.ItemsSource=storageBitRateViewModels;
            storageBitRateViewModels.CollectionChanged += StorageBitRateViewModels_CollectionChanged;
        }



        private List<string> raidList = new List<string>();
        private List<string> unitList = new List<string>()
        {
            "GB","TB"
        };

        private List<string> bitRatelist= new List<string>()
        {
            "512Kbps", "1Mbps","2Mbps","3Mbps", "4Mbps","5Mbps", "6Mbps","7Mbps","8Mbps","9Mbps", "10Mbps","11Mbps","12Mbps","13Mbps", "14Mbps","15Mbps","16Mbps"
        };

        private void LoadData()
        {

            raidList.Add("RAID0");
            raidList.Add("RAID1");
            raidList.Add("RAID5");
            raidList.Add("RAID6");
            raidList.Add("RAID10");
        }


        private void DiskNumberSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.IsLoaded)
            {
               

                if (RaidComboBox.SelectedIndex != 4)
                {

                    UpdateNumber();


                    if (diskCount >= 4)
                    {
                        HotBackupSlider.Maximum = 4;
                    }
                    else
                    {
                        HotBackupSlider.Maximum = diskCount - 1;
                    }

                    RaidCalculation();
                }


            }
        }

        private void DiskNumberSlider_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {


            UpdateNumber();


            if (diskCount >= 4)
            {
                HotBackupSlider.Maximum = 4;
            }
            else
            {
                HotBackupSlider.Maximum = diskCount - 1;
            }

            RaidCalculation();

        }


        private void RaidComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                UpdateNumber();
                RaidCalculation();
            }

            if (RaidComboBox.SelectedIndex == 4)
            {
                DiskNumberSlider_OnPreviewMouseUp(null, null);
            }

        }




        private int diskCount;//总磁盘数
        private int hotBackup;//热备磁盘数
        private int volumeDiskNumber;//RAID磁盘数

        /// <summary>
        /// 更新磁盘数量
        /// </summary>
        private void UpdateNumber()
        {
            diskCount = (int)DiskNumberSlider.Value;

            hotBackup = (int)HotBackupSlider.Value;

            volumeDiskNumber = diskCount - hotBackup;

            VolumeDiskNumber.Text = volumeDiskNumber.ToString();
        }




        /// <summary>
        /// RAID计算
        /// </summary>
        private void RaidCalculation()
        {

            if (this.IsLoaded && RaidComboBox.SelectedIndex != -1)
            {


                int space;
                // raid类型
                string raidType = raidList[RaidComboBox.SelectedIndex];
                try
                {
                    space = int.Parse(SpaceBox.Text);
                }
                catch (Exception exception)
                {
                    SpaceBox.Text = "1";
                    space = 1;
                }

                int index = RaidComboBox.SelectedIndex;

                switch (index)
                {
                    case 0://raid0
                        HotBackupSlider.Value = 0;
                        DiskNumberSlider.Minimum = 2;
                        HotBackupSlider.IsEnabled = false;


                        break;
                    case 1://raid1
                        HotBackupSlider.IsEnabled = true;
                        DiskNumberSlider.Minimum = 2;
                        if (volumeDiskNumber < 2)
                        {
                            //Snackbar.MessageQueue?.Enqueue("RAID1至少需要2块磁盘,热备盘已取消");
                            HotBackupSlider.Value = 0;
                        }


                        break;
                    case 2://raid5
                        DiskNumberSlider.Minimum = 3;
                        if (volumeDiskNumber < 3)
                        {
                            //Snackbar.MessageQueue?.Enqueue("RAID3至少需要3块磁盘,热备盘已取消");
                            HotBackupSlider.Value = 0;
                        }
                        HotBackupSlider.IsEnabled = true;

                        break;
                    case 3://raid6
                        DiskNumberSlider.Minimum = 4;
                        if (volumeDiskNumber < 4)
                        {

                            //Snackbar.MessageQueue?.Enqueue("RAID6至少需要4块磁盘,热备盘已取消");
                            HotBackupSlider.Value = 0;

                        }
                        HotBackupSlider.IsEnabled = true;

                        break;
                    case 4://raid10

                        DiskNumberSlider.Minimum = 4;

                        if (DiskNumberSlider.Value < 4)
                        {
                            DiskNumberSlider.Value = 4;
                        }


                        HotBackupSlider.IsEnabled = true;
                        break;
                }


                if (index != 4)
                {
                    SizeGrid.Visibility = Visibility.Visible;
                    ProportionalCalculation(raidType, space);
                }
                else
                {
                    if (IsEven(volumeDiskNumber))
                    {
                        SizeGrid.Visibility = Visibility.Visible;
                        ProportionalCalculation(raidType, space);
                    }
                    else
                    {
                        SizeGrid.Visibility = Visibility.Hidden;
                        //Snackbar.MessageQueue?.Enqueue($"RAID10至少需要4块磁盘,且磁盘数目必须为偶数,当前卷内磁盘数为{volumeDiskNumber}");

                    }
                }
            }
        }

        /// <summary>
        /// 卷容量计算比例
        /// </summary>
        /// <param name="raidType"></param>
        /// <param name="space"></param>
        private void ProportionalCalculation(string raidType, int space)
        {
            string unit = unitList[UnitBox.SelectedIndex];


            //RAID卷空间
            double volumeSpace = 0;

            //RAID磁盘总空间
            double diskSpace = 0;

            //热备磁盘总空间
            double hotBackupSpace = 0;

            //保护磁盘总空间
            double protectionSpace = 0;


            volumeSpace = CalculateRaidCapacity(raidType, space, volumeDiskNumber);

            VolumeSpace.Text = volumeSpace + $" {unit}";

            //热备磁盘总空间
            hotBackupSpace = space * hotBackup;
            HotBackupSpaceTextBlock.Text = hotBackupSpace.ToString() + $" {unit}";


            //磁盘总空间
            diskSpace = space * diskCount;

            //保护磁盘总空间
            protectionSpace = diskSpace - hotBackupSpace - volumeSpace;

            ProtectionSpaceTextBox.Text = protectionSpace.ToString() + $" {unit}";

            int width = (int)SizeGrid.ActualWidth;

            AvailableSpace.Width = width * (volumeSpace / diskSpace);
            ProtectionSpace.Width = width * (protectionSpace / diskSpace);
            HotBackupSpace.Width = width * (hotBackupSpace / diskSpace);

        }

        /// <summary>
        /// 判断是不是偶数
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public bool IsEven(int number)
        {
            // 使用模运算符 (%) 来判断数字是奇数还是偶数。
            if (number % 2 == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public double CalculateRaidCapacity(string raidType, double diskCapacity, int diskCount)
        {
            switch (raidType.ToUpper())
            {
                case "RAID0":
                    return diskCapacity * diskCount;
                case "RAID1":
                    return diskCapacity; // 对于RAID1，如果有多于两个磁盘，则这里可能需要调整
                case "RAID5":
                    if (diskCount < 3)
                    {
                       // Snackbar.MessageQueue?.Enqueue("RAID5至少需要3块磁盘");
                    }

                    return (diskCount - 1) * diskCapacity;
                case "RAID6":
                    if (diskCount < 4)
                    {
                       // Snackbar.MessageQueue?.Enqueue("RAID6至少需要4块磁盘");
                    }
                    return (diskCount - 2) * diskCapacity;
                case "RAID10":
                    if (diskCount % 2 != 0 || diskCount < 4)
                    {
                        //Snackbar.MessageQueue?.Enqueue("RAID10至少需要4块磁盘,且磁盘数量必须是偶数");

                    }

                    return (diskCount / 2) * diskCapacity;
                default:
                    return 0;
            }
        }



        private void HotBackupSlider_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.IsLoaded)
            {
                UpdateNumber();

                RaidCalculation();
            }
        }

        private void HotBackupSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (RaidComboBox.SelectedIndex != 4)
            {
                if (this.IsLoaded)
                {
                    UpdateNumber();

                    RaidCalculation();
                }
            }
        }


        private void SpaceBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                UpdateNumber();

                RaidCalculation();
            }
        }

        private void UnitBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                UpdateNumber();

                RaidCalculation();
            }
        }

        private void StorageCalculationWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
           // RaidCalculation();
        }

        //------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// 编码类型
        /// </summary>
        private List<string> codingTypeList = new List<string>()
        {
            "H.264","Smart264","H.265","Smart265"
        };

        private List<string> resolutionList = new List<string>()
        {
            "720P","2MP","3MP","4MP","5MP","8MP"
        };

        ObservableCollection<StorageCalculatorViewModel> storageCalculatorList = new ObservableCollection<StorageCalculatorViewModel>();

        /// <summary>
        /// 添加摄像头存储方案
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {
            StorageCalculatorViewModel storage = new StorageCalculatorViewModel();

            storage.CodingType = codingTypeList[CodingTypeComboBox.SelectedIndex];
            storage.Resolution = resolutionList[ResolutionComboBox.SelectedIndex];

            int cameraNumber = 1;

            try
            {
                cameraNumber = Convert.ToInt32(CameraCountTextBox.Text);
            }
            catch (Exception exception)
            {
                cameraNumber = 1;
            }


            storage.CameraNumber = cameraNumber;

            int saveDay = 30;

            try
            {
                saveDay = Convert.ToInt32(SaveDayTextBox.Text);
            }
            catch (Exception exception)
            {
                saveDay = 30;
            }
            storage.SaveDay = saveDay;

            storage.StorageSpace =Convert.ToInt32( StorageCalculator.CalculateStorageRequirement(storage.CodingType, storage.Resolution, cameraNumber, saveDay)).ToString();
            
            storageCalculatorList.Add(storage);
        }

        private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
        {

            DependencyObject dep = (DependencyObject)e.OriginalSource;

            // 迭代视觉树以找到 DataGridRow
            while ((dep != null) && !(dep is DataGridRow))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            if (dep == null)
                return;

            // 获取 DataGridRow
            DataGridRow row = dep as DataGridRow;
            if (row == null)
                return;

            // 获取行数据对象
            var rowData = row.Item as StorageCalculatorViewModel;
            if (rowData != null)
            {
                storageCalculatorList.RemoveAt(CameraDataGrid.SelectedIndex);
            }


        }

        /// <summary>
        /// 监控摄像头存储方案发生变化，计算存储空间总数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StorageCalculatorList_CollectionChanged(object? sender,
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {



            Double sum = storageCalculatorList.Sum(item => Convert.ToInt32(item.StorageSpace));


            DiskSpaceCountGB.Dispatcher.Invoke(() =>
            {
                DiskSpaceCountGB.Text = sum.ToString() + $" GB";
            });


                DiskSpaceCountTB.Dispatcher.Invoke(() =>
            {
                DiskSpaceCountTB.Text = (sum / 1024).ToString("F2") + $" TB";
            });

        }



        ObservableCollection<StorageBitRateViewModel> storageBitRateViewModels= new ObservableCollection<StorageBitRateViewModel>();

        private void AddButton2_OnClick(object sender, RoutedEventArgs e)
        {
            StorageBitRateViewModel storage = new StorageBitRateViewModel();

            storage.BitRate = bitRatelist[BitRateComboBox.SelectedIndex];

            double bitRate = 2;

            if (BitRateComboBox.SelectedIndex != 0)
            {
                bitRate = BitRateComboBox.SelectedIndex * 1024;
            }
            else
            {
                bitRate = 0.5 * 1024;
            }


            int cameraNumber = 1;

            try
            {
                cameraNumber = Convert.ToInt32(CameraCountTextBox2.Text);
            }
            catch (Exception exception)
            {
                cameraNumber = 1;
            }


            storage.CameraNumber = cameraNumber;

            int saveDay = 30;

            try
            {
                saveDay = Convert.ToInt32(SaveDayTextBox2.Text);
            }
            catch (Exception exception)
            {
                saveDay = 30;
            }
            storage.SaveDay = saveDay;


            storage.StorageSpace = Convert.ToInt32(StorageCalculator.CalculateStorageRequirementByBitrate(bitRate, cameraNumber,  saveDay)).ToString();

            storageBitRateViewModels.Add(storage);

        }

        private void StorageBitRateViewModels_CollectionChanged(object? sender,
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

            Double sum = storageBitRateViewModels.Sum(item => Convert.ToInt32(item.StorageSpace));

            DiskSpaceCountGB2.Text = sum.ToString() + $" GB";
            DiskSpaceCountTB2.Text = (sum / 1024).ToString("F2") + $" TB";

        }

        private void DeleteButton2_OnClick(object sender, RoutedEventArgs e)
        {
            DependencyObject dep = (DependencyObject)e.OriginalSource;

            // 迭代视觉树以找到 DataGridRow
            while ((dep != null) && !(dep is DataGridRow))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            if (dep == null)
                return;

            // 获取 DataGridRow
            DataGridRow row = dep as DataGridRow;
            if (row == null)
                return;

            // 获取行数据对象
            var rowData = row.Item as StorageBitRateViewModel;
            if (rowData != null)
            {
                storageBitRateViewModels.RemoveAt(CameraDataGrid2.SelectedIndex);
            }
        }


        private async void HelpButton_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new ConfirmationDialog
            {
                Title = "注意！数据仅供参考",
                Prompt = $"计算参数参考自海康威视，实际所需存储空间可能因多种因素而有所变化，包括但不限于视频压缩效率、场景复杂度（影响视频压缩比）、昼夜模式切换等,建议预留至少20%的额外存储空间以应对不可预见的数据增长或系统调整。",
                ConfirmButtonText = "确认",
                

            };

            // 显示对话框
            await DialogHost.Show(dialog, "MessageDialogHost");

        }
    }
}
