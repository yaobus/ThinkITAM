using System.Security.Cryptography;
using System.Text;

namespace ThinkITAM.Shared.Encryption
{
    public class EncryptionDecryption
    {


        /// <summary>
        /// 计算密码的MD5值，加盐,只取前8位
        /// </summary>
        /// <param name="str">密码字符串</param>
        /// <param name="num">16</param>
        /// <returns></returns>
        public static string CalculateMD5(string input)
        {
            // 将输入字符串转换为字节数组
            byte[] inputBytes = Encoding.UTF8.GetBytes(input + "p@s5w0d");

            // 创建一个 MD5 实例
            using (MD5 md5 = MD5.Create())
            {
                // 计算输入字节数组的 MD5 哈希值
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // 将字节数组转换为十六进制字符串
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                string str = sb.ToString();
                // 返回 MD5 哈希值的字符串表示形式
                return str.Substring(0, 8);
            }
        }





        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string EncryptString(string plainText, string password)
        {
            byte[] encryptedBytes;

            using (Aes aesAlg = Aes.Create())
            {
                // 生成符合 AES 要求的密钥
                aesAlg.Key = GenerateKey(password, aesAlg.KeySize);
                aesAlg.IV = new byte[16]; // 使用默认的 IV（Initialization Vector）

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encryptedBytes = msEncrypt.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(encryptedBytes);
        }


        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="encryptedText"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string DecryptString(string encryptedText, string password)
        {
            byte[] cipherBytes = Convert.FromBase64String(encryptedText);
            string plainText = null;

            using (Aes aesAlg = Aes.Create())
            {
                // 生成符合 AES 要求的密钥
                aesAlg.Key = GenerateKey(password, aesAlg.KeySize);
                aesAlg.IV = new byte[16]; // 使用默认的 IV（Initialization Vector）

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plainText = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plainText;
        }

        private static byte[] GenerateKey(string password, int keySize)
        {
            // 使用密码的哈希值作为密钥
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(passwordBytes);

                // 截取满足密钥长度要求的部分
                byte[] key = new byte[keySize / 8];
                Array.Copy(hash, key, key.Length);
                return key;
            }
        }
    }
}
