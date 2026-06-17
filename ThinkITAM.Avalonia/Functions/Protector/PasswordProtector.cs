using System.Security.Cryptography;
using System.Text;

namespace ThinkITAM.Functions.Protector
{
    /// <summary>
    /// 密码保护器 — 基于机器指纹的 AES 加解密
    /// 使用 MachineName + UserName 作为硬件指纹
    /// </summary>
    public static class PasswordProtector
    {
        /// <summary>
        /// 获取机器唯一标识
        /// </summary>
        public static string GetHardwareKey()
        {
            return Environment.MachineName + Environment.UserName;
        }

        /// <summary>
        /// 使用硬件ID生成 AES 密钥和 IV
        /// </summary>
        private static void GetAesKeyAndIV(out byte[] key, out byte[] iv)
        {
            using var sha256 = SHA256.Create();
            var hwKey = Encoding.UTF8.GetBytes(GetHardwareKey());
            var hash = sha256.ComputeHash(hwKey);
            key = hash.Take(16).ToArray();
            iv = hash.Skip(16).Take(16).ToArray();
        }

        /// <summary>
        /// 使用硬件ID加密
        /// </summary>
        public static string Encrypt(string plainText)
        {
            GetAesKeyAndIV(out var key, out var iv);
            return AesEncrypt(plainText, key, iv);
        }

        /// <summary>
        /// 使用硬件ID解密
        /// </summary>
        public static string Decrypt(string base64CipherText)
        {
            GetAesKeyAndIV(out var key, out var iv);
            return AesDecrypt(base64CipherText, key, iv);
        }

        /// <summary>
        /// AES CBC 加密
        /// </summary>
        private static string AesEncrypt(string plainText, byte[] key, byte[] iv)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            using var encryptor = aes.CreateEncryptor();
            var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            return Convert.ToBase64String(cipherBytes);
        }

        /// <summary>
        /// AES CBC 解密
        /// </summary>
        private static string AesDecrypt(string base64CipherText, byte[] key, byte[] iv)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            var cipherBytes = Convert.FromBase64String(base64CipherText);
            using var decryptor = aes.CreateDecryptor();
            var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            return Encoding.UTF8.GetString(plainBytes);
        }
    }
}
