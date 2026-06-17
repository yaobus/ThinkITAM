using SharedImpl = ThinkITAM.Shared.Encryption.EncryptionDecryption;

namespace ThinkITAM.Functions.EncryptionDecryption
{
    public class EncryptionDecryption
    {
        /// <summary>
        /// 计算密码的MD5值，加盐,只取前8位
        /// </summary>
        public static string CalculateMD5(string input)
            => SharedImpl.CalculateMD5(input);

        /// <summary>
        /// 加密字符串
        /// </summary>
        public static string EncryptString(string plainText, string password)
            => SharedImpl.EncryptString(plainText, password);

        /// <summary>
        /// 解密字符串
        /// </summary>
        public static string DecryptString(string encryptedText, string password)
            => SharedImpl.DecryptString(encryptedText, password);
    }
}
