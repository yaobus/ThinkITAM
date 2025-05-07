using System;
using System.Security.Cryptography;
using System.Text;
using System.Management;

namespace ThinkITAM.Functions.Protector
{
    public static class PasswordProtector
    {
        public static string GetHardwareKey()
        {
            var cpuId = GetCpuId();
            var boardId = GetMotherboardSerialNumber();
            return cpuId + boardId;
        }

        private static string GetCpuId()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT ProcessorId FROM Win32_Processor"))
                {
                    foreach (var item in searcher.Get())
                    {
                        var id = item["ProcessorId"]?.ToString() ?? "unknown";
                        return id;
                    }
                }
            }
            catch { }

            return "unknown";
        }

        private static string GetMotherboardSerialNumber()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BaseBoard"))
                {
                    foreach (var item in searcher.Get())
                    {
                        var serial = item["SerialNumber"]?.ToString()?.Trim() ?? "unknown";
                        return serial;
                    }
                }
            }
            catch { }

            return "unknown";
        }

        /// <summary>
        /// 获取硬盘序列号
        /// </summary>
        /// <returns></returns>
        private static string GetDiskSerialNumber()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_PhysicalMedia"))
                {
                    foreach (var item in searcher.Get())
                    {
                        var serial = item["SerialNumber"]?.ToString()?.Trim() ?? "unknown";
                        return serial;
                    }
                }
            }
            catch { }

            return "unknown";
        }

        private static void GetAesKeyAndIV(out byte[] key, out byte[] iv)
        {
            using var sha256 = SHA256.Create();
            var hwKey = Encoding.UTF8.GetBytes(GetHardwareKey());
            var hash = sha256.ComputeHash(hwKey);

            key = hash.Take(16).ToArray(); // AES-128 Key
            iv = hash.Skip(16).Take(16).ToArray(); // AES IV
        }

        public static string Encrypt(string plainText)
        {
            GetAesKeyAndIV(out var key, out var iv);

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

        public static string Decrypt(string base64CipherText)
        {
            GetAesKeyAndIV(out var key, out var iv);

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