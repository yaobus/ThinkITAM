using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MacAddressVendorLookup;
using ZXing;

namespace ThinkITAM.Functions.FunctionClass;

public class VendorInfoFetcher
{
    public async Task<string> GetVendorInfo(string macAddress)
    {
        if (macAddress != "N/A" && macAddress != null)
        {

            var vendorInfoProvider = new MacAddressVendorLookup.MacVendorBinaryReader();

            await InitVendorInfoProviderAsync(vendorInfoProvider);

            var addressMatcher = new MacAddressVendorLookup.AddressMatcher(vendorInfoProvider);

            PhysicalAddress physicalAddress = ConvertToPhysicalAddress(macAddress);

            var vendorInfo = addressMatcher.FindInfo(physicalAddress);

            if (vendorInfo != null)
            {
                return vendorInfo.Organization;
            }
            else
            {
                return "";
            }

        }
        else
        {
            return "";
        }


    }


    /// <summary>
    /// 初始化MAC地址信息库
    /// </summary>
    /// <param name="vendorInfoProvider"></param>
    /// <returns></returns>
    public async Task InitVendorInfoProviderAsync(MacAddressVendorLookup.MacVendorBinaryReader vendorInfoProvider)
    {
        using (var resourceStream = await MacAddressVendorLookup.ManufBinResource.GetStream())
        {
            await vendorInfoProvider.Init(resourceStream);
        }
    }

    private static PhysicalAddress ConvertToPhysicalAddress(string macAddress)
    {
        // 清除可能的分隔符，如 '-', ':'
        string cleanMac = macAddress.Replace("-", "").Replace(":", "");

        // 将清理后的MAC地址字符串转换为字节数组
        byte[] macBytes = new byte[cleanMac.Length / 2];
        for (int i = 0; i < macBytes.Length; i++)
        {
            macBytes[i] = Convert.ToByte(cleanMac.Substring(i * 2, 2), 16);
        }

        // 使用PhysicalAddress.Parse或者PhysicalAddress(byte[])
        // 注意：Parse方法要求格式严格
        return new PhysicalAddress(macBytes);
        // 或者直接使用：
        // return PhysicalAddress.Parse(cleanMac); // 这种方式需要确保cleanMac是无分隔符的格式


    }
}
