using System.Windows;

namespace ThinkITAM.Functions.Language
{
    public class LanguageSet
    {

        //应用语言显示设置
        /// <summary>
        /// 设置要显示的语言索引,用于设置页面中的语言下拉框
        /// </summary>
        /// <param name="num">0为zh-CN,1为en-US</param>
        public static void LanguageSelect(int num)
        {
            switch (num)
            {
                case 0:
                    SetLanguage("zh-CN");
                    break;
                case 1:
                    SetLanguage("en-US");
                    break;
            }
        }

        /// <summary>
        /// 设置程序显示语言
        /// </summary>
        /// <param name="languageTag">程序中已包含的语言字典名称，例如zh-CN</param>
        //设置语言
        public static void SetLanguage(string languageTag)
        {
            List<ResourceDictionary> dictionaryList = new List<ResourceDictionary>();
            foreach (ResourceDictionary dictionary in Application.Current.Resources.MergedDictionaries)
            {
                dictionaryList.Add(dictionary);
            }

            string requestedCulture = string.Format(@"Language\{0}.xaml", languageTag);

            Uri reUri = new Uri(requestedCulture, UriKind.Relative);
            ResourceDictionary rdDictionary = (ResourceDictionary)Application.LoadComponent(reUri);
            Application.Current.Resources.MergedDictionaries.Remove(rdDictionary);
            Application.Current.Resources.MergedDictionaries.Add(rdDictionary);



        }

    }
}
