using ThinkITAM.Shared.Encryption;

namespace ThinkITAM.Shared.Asset
{
    public class AssetIdCreate
    {


        /// <summary>
        /// 生成AssetId
        /// </summary>
        /// <param name="assetTagNumber">资产字符串</param>
        /// <returns></returns>
        public static string CreateAssetId(string assetTagNumber)
        {
            string str = DateTime.Now.ToString() + assetTagNumber;

            return EncryptionDecryption.CalculateMD5(str).ToUpper();

        }


    }
}
