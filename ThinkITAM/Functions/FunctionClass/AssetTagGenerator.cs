using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using ThinkITAM.UserControls.Asset;

namespace ThinkITAM.Functions.FunctionClass
{
    public class AssetTagGenerator
    {

        private UserControl CreateAssetTag(ViewModels.AssetManage.AssetViewModel assetInfo)
        {
            var assetTag = new AssetTagTemplateUserControl();

            assetTag.DataContext = assetInfo;


          //  assetTag.BarcodeImage.Source = GenerateBarcode(assetInfo.AssetQrCode);

            return assetTag;
        }

        private BitmapImage GenerateBarcode(string content)
        {
            // 在这里生成条形码图像
            // 为简单起见，使用占位图像
            BitmapImage barcodeImage = new BitmapImage(new Uri("pack://application:,,,/Resources/placeholder-barcode.png"));
            return barcodeImage;
        }

        private void SaveTagToFile(UserControl tag, string filePath)
        {
            // 渲染UserControl为位图
            RenderTargetBitmap bitmap = new RenderTargetBitmap(300, 100, 96, 96, PixelFormats.Pbgra32);
            tag.Measure(new Size(300, 100));
            tag.Arrange(new Rect(new Size(300, 100)));
            bitmap.Render(tag);

            // 保存为PNG文件
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                encoder.Save(fileStream);
            }
        }

    }
}
