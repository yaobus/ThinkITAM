namespace ThinkITAM.Functions.FunctionClass
{
    public class AssetNumberCutClass
    {

        /// <summary>
        /// 取出资产编号最后的数字
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetAssetNumberLastPart(string input)
        {
            try
            {
                // 使用 Split 方法分割字符串，取得最后一部分
                string[] parts = input.Split('-');
                string lastPart = parts[parts.Length - 1];

                // 返回最后一部分
                return lastPart;
            }
            catch (Exception e)
            {
                return null;
            }



        }


        /// <summary>
        /// 取出资产编号前缀部分
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetAssetNumberTagPart(string input)
        {
            try
            {
                // 使用 Split 方法分割字符串，取得最后一部分
                string[] parts = input.Split('-');
                string lastPart = parts[parts.Length - 1];

                // 使用 Substring 方法获取除最后的数字以外的部分
                string otherParts = input.Substring(0, input.Length - lastPart.Length);

                // 返回除最后的数字以外的部分
                return otherParts;
            }
            catch (Exception e)
            {
                return null;
            }


        }

    }
}
