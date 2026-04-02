using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using QRCoder;
using ThinkITAM.UserControls.Asset;
using ThinkITAM.ViewModels.AssetManage;
using static ThinkITAM.Functions.Export.ExportAssetTagImage;
using Size = System.Windows.Size;

namespace ThinkITAM.Functions.Export;

public class ExportAssetTagImage
{
    public enum Unit
    {
        Pixel, Millimeter
    }


    /// <summary>
    /// 生成高分辨率资产标签图片（完整版）
    /// </summary>
    /// <param name="data">数据模型</param>
    /// <param name="filePath">保存路径</param>
    /// <param name="targetWidth">目标宽度</param>
    /// <param name="targetHeight">目标高度</param>
    /// <param name="unit">单位</param>
    /// <param name="dpi">DPI（打印建议300）</param>
    public static void GenerateAssetTagImage(AssetViewModel data, string filePath, double targetWidth, double targetHeight, Unit unit, double dpi = 300)
    {
        // 1. 物理尺寸换算
        double inchWidth = unit == Unit.Millimeter ? (targetWidth / 25.4) : (targetWidth / 96.0);
        double inchHeight = unit == Unit.Millimeter ? (targetHeight / 25.4) : (targetHeight / 96.0);

        int pixelWidth = (int)(inchWidth * dpi);
        int pixelHeight = (int)(inchHeight * dpi);

        // 2. 创建并配置 UI 控件
        var assetTagControl = new AssetTagTemplateUserControl();
        assetTagControl.DataContext = data;

        // --- 恢复二维码生成步骤 ---
        using (Bitmap qrCodeImage = GenerateQRCode(data.AssetQrCode))
        {
            IntPtr hBitmap = qrCodeImage.GetHbitmap();
            try
            {
                // 将 System.Drawing.Bitmap 转换为 WPF 的 ImageSource
                assetTagControl.BarcodeImage.Source = Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                // 非常重要：释放非托管的 HBitmap 句柄，防止内存泄漏
                DeleteObject(hBitmap);
            }
        }
        // -----------------------

        // 3. 测量和布局 (基于 96 DPI 环境计算)
        assetTagControl.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
        System.Windows.Size originalSize = assetTagControl.DesiredSize;

        double target96DpiWidth = inchWidth * 96;
        double target96DpiHeight = inchHeight * 96;

        // 应用缩放以匹配目标物理尺寸
        assetTagControl.LayoutTransform = new ScaleTransform(
            target96DpiWidth / originalSize.Width,
            target96DpiHeight / originalSize.Height);

        // 重新布置
        System.Windows.Size layoutSize = new System.Windows.Size(target96DpiWidth, target96DpiHeight);
        assetTagControl.Measure(layoutSize);
        assetTagControl.Arrange(new Rect(layoutSize));
        assetTagControl.UpdateLayout();

        // 4. 渲染为高分辨率位图
        RenderTargetBitmap bitmap = new RenderTargetBitmap(
            pixelWidth,
            pixelHeight,
            dpi,
            dpi,
            PixelFormats.Pbgra32);

        bitmap.Render(assetTagControl);

        // 5. 保存文件
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            encoder.Save(fileStream);
        }
    }

    // 必须包含这个 Win32 API 引用来配合 GetHbitmap() 使用
    [System.Runtime.InteropServices.DllImport("gdi32.dll")]
    public static extern bool DeleteObject(IntPtr hObject);



    /// <summary>
    /// 生成二维码
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private static Bitmap GenerateQRCode(string text)
    {
        // 创建 QR code 生成器
        QRCodeGenerator qrGenerator = new QRCodeGenerator();

        // 创建 QR code 数据
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);


        // 生成 QR code 图像
        QRCode qrCode = new QRCode(qrCodeData);



        Bitmap qrCodeImage = qrCode.GetGraphic(20, "#2f9f9f", "#FFFFFF");

        return qrCodeImage;
    }
}
