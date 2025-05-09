using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using AForge.Video;
using AForge.Video.DirectShow;
using Microsoft.Win32;
using SkiaSharp;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using ZXing.SkiaSharp;
using static MaterialDesignThemes.Wpf.Theme;

namespace ThinkITAM.Windows.Scan
{
    public partial class ScanWindow : Window
    {
        private FilterInfoCollection videoDevices;

        private VideoCaptureDevice videoSource;

        private BarcodeReader barcodeReader = new BarcodeReader();

        private DispatcherTimer _timer;

        public ScanWindow()
        {
            InitializeComponent();
            Loaded += ScanWindow_Loaded;
        }

        ObservableCollection<string> cameraList = new ObservableCollection<string>();

        private void ScanWindow_Loaded(object sender, RoutedEventArgs e)
        {

            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            ComboBox.ItemsSource= cameraList;

            if (videoDevices.Count == 0)
            {
                //MessageBox.Show("No video sources found");
                return;
            }

            foreach (FilterInfo device in videoDevices)
            {
                cameraList.Add(device.Name);
            }


        }


        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
                Dispatcher.Invoke(() => videoImage.Source = BitmapToImageSource(bitmap));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource = null;
            }
            base.OnClosed(e);


            if (_timer!=null)
            {
                _timer.Stop();
            }



            
        }

        /// <summary>
        /// 是否进行了扫描
        /// </summary>
        private bool isScanning;

        private void ComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            isScanning = false;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(3);

            videoSource = new VideoCaptureDevice(videoDevices[ComboBox.SelectedIndex].MonikerString);

            videoSource.NewFrame += VideoSource_NewFrame;


            videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
            videoSource.Start();

            _timer.Tick += _timer_Tick; ;
        }

        private void _timer_Tick(object? sender, EventArgs e)
        {
            isScanning = false;
        }

        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                // 从摄像头获取新帧
                Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();

                // 转换为SkiaSharp.SKBitmap
                using (MemoryStream stream = new MemoryStream())
                {
                    bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

                    stream.Seek(0, SeekOrigin.Begin);

                    SKBitmap skBitmap = SKBitmap.Decode(stream);

                    // 在UI线程上执行二维码扫描
                    Dispatcher.Invoke(() =>
                    {
                        try
                        {
                            // 尝试识别二维码
                            var result = barcodeReader.Decode(skBitmap); // 这一行
                            if (result != null && isScanning == false)
                            {
                                
                               
                                MessageBox.Show("扫描结果：" + result.Text);
                                isScanning = true;
                                _timer.Start();
                            }
                            
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("扫描失败：" + ex.Message);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("获取摄像头画面失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 选择二维码文件路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectFileButton_OnClick(object sender, RoutedEventArgs e)
        {
            // 创建 OpenFileDialog 实例
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // 设置文件类型筛选，仅允许选择 CSV 文件
            openFileDialog.Filter = "Jpg files (*.jpg)|*jpg|Png files (*.png)|*.png|All files (*.*)|*.*";

            // 显示对话框并获取用户选择的结果
            bool? result = openFileDialog.ShowDialog();

            // 如果用户选择了文件，则将文件路径加载到 TextBox 中
            if (result == true)
            {
                string selectedFilePath = openFileDialog.FileName;
                FilePath.Text = selectedFilePath;
            }
        }

        /// <summary>
        /// 读取二维码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReadButton_OnClick(object sender, RoutedEventArgs e)
        {

            ReadQrCodeFromBitmapPath(FilePath.Text);

        }

        public SKBitmap ReadQrCodeFromBitmapPath(string imagePath)
        {
            // 1. 使用 BitmapImage 加载图像文件
            BitmapImage bitmapImage = new BitmapImage(new Uri(imagePath));

            // 2. 将 BitmapImage 转换为 System.Drawing.Bitmap
            Bitmap bitmap;

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                bitmap = new Bitmap(outStream);


                // 转换为SkiaSharp.SKBitmap
                using (MemoryStream stream = new MemoryStream())
                {
                    bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

                    stream.Seek(0, SeekOrigin.Begin);

                    SKBitmap skBitmap = SKBitmap.Decode(stream);

                    Console.WriteLine(skBitmap.Info.ColorSpace);

                    // 在UI线程上执行二维码扫描
                    Dispatcher.Invoke(() =>
                    {
                        try
                        {
                            // 尝试识别二维码
                            var result = barcodeReader.Decode(skBitmap); // 这一行
                            if (result != null)
                            {
                                MessageBox.Show("扫描结果：" + result.Text);
                                return result.Text;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("扫描失败：" + ex.Message);
                           
                        }


                        return null;
                    });
                }
            }


            return null;

        }


        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            AssetCode.Text = FunctionClass.AssetCodeClass.GenerateChecksum(AssetCodeSource.Text);
        }

        private void Button2_OnClick(object sender, RoutedEventArgs e)
        {
            FunctionClass.AssetCodeClass.CheckAssetCode(AssetCode.Text);
        }
    }
}

