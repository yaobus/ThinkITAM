using System.Windows.Media;

namespace ThinkITAM.Functions.Converters
{
    public class ColorConverterClass
    {

        //public static Brush ConvertColorCodeToBrush(string colorCode)
        //{
        //    var brushConverter = new BrushConverter();
        //    return (Brush)brushConverter.ConvertFromString(colorCode);
        //}


        /// <summary>
        /// 颜色代码转成颜色
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static SolidColorBrush ConvertHexToBrush(string hex)
        {

            if (hex.StartsWith("#"))
                hex = hex.Substring(1);


            Color color = (Color)ColorConverter.ConvertFromString("#" + hex);


            return new SolidColorBrush(color);
        }



        /// <summary>
        /// 颜色代码转换为BRUSH
        /// </summary>
        /// <param name="colorCode"></param>
        /// <returns></returns>
        public static Brush ColorToBrush(string colorCode)
        {
            if (colorCode != "")
            {

                SolidColorBrush brush = (SolidColorBrush)(new BrushConverter().ConvertFrom(colorCode));

                return brush;
            }

            return null;
        }

        /// <summary>
        /// 颜色转换为BRUSH
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static SolidColorBrush ConvertColorToBrush(Color color)
        {
            return new SolidColorBrush(color);
        }
    }


}
