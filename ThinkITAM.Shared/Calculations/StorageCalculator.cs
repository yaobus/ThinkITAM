namespace ThinkITAM.Shared.Calculations;

/// <summary>
/// 监控存储计算器
/// </summary>
public class StorageCalculator
{

    // 假设每种编码类型和分辨率组合的平均比特率(Mbps)
    private static double GetAverageBitrate(string codec, string resolution)
    {
        var bitrateDict = new System.Collections.Generic.Dictionary<(string, string), double>
        {
            { ("H.264", "720P"), 2.2 },
            { ("H.264", "2MP"), 4.3 },
            { ("H.264", "3MP"), 6.4 },
            { ("H.264", "4MP"), 8.7 },
            { ("H.264", "5MP"), 10 },
            { ("H.264", "6MP"), 11 },
            { ("H.264", "8MP"), 12.9 },

            { ("Smart264", "720P"), 1.1 },
            { ("Smart264", "2MP"), 2.2 },
            { ("Smart264", "3MP"), 3.3 },
            { ("Smart264", "4MP"), 4.4 },
            { ("Smart264", "5MP"), 5 },
            { ("Smart264", "6MP"), 5.5 },
            { ("Smart264", "8MP"), 6.45 },

            { ("H.265", "720P"), 1.1 },
            { ("H.265", "2MP"), 2.2 },
            { ("H.265", "3MP"), 3.3 },
            { ("H.265", "4MP"), 4.4 },
            { ("H.265", "5MP"), 5 },
            { ("H.265", "6MP"), 5.5 },
            { ("H.265", "8MP"), 6.45 },

            { ("Smart265", "720P"), 0.77 },
            { ("Smart265", "2MP"), 1.32 },
            { ("Smart265", "3MP"), 1.65 },
            { ("Smart265", "4MP"), 2.2 },
            { ("Smart265", "5MP"), 2.5 },
            { ("Smart265", "6MP"), 2.75 },
            { ("Smart265", "8MP"), 3.3 }
        };

        if (bitrateDict.TryGetValue((codec, resolution), out double bitrate))
        {
            return bitrate;
        }

        throw new ArgumentException("不支持的编码类型或分辨率");
    }

    /// <summary>
    /// 计算存储需求
    /// </summary>
    /// <param name="codec">编码方式</param>
    /// <param name="resolution">分辨率</param>
    /// <param name="cameraCount">摄像头路数</param>
    /// <param name="days">存储天数</param>
    /// <returns></returns>
    public static double CalculateStorageRequirement(string codec, string resolution, int cameraCount, int days)
    {
        double bitrateMbps = GetAverageBitrate(codec, resolution);
        double storagePerCameraPerDayGB = (bitrateMbps * 3600 * 24) / 8 / 1024; // Mbps转GB
        return storagePerCameraPerDayGB * cameraCount * days;
    }


    /// <summary>
    /// 根据码率计算存储需求
    /// </summary>
    /// <param name="bitrateKbps">码率</param>
    /// <param name="cameraCount">摄像头路数</param>
    /// <param name="days">天数</param>
    /// <returns></returns>
    public static double CalculateStorageRequirementByBitrate(double bitrateKbps, int cameraCount, int days)
    {
        // 将码率从Kbps转换为Mbps
        double bitrateMbps = bitrateKbps / 1024;

        // 计算每天每路摄像头需要的存储空间(GB)，公式: (bitrateMbps * 3600秒 * 24小时) / 8比特每字节 / 1024MB每GB
        double storagePerCameraPerDayGB = (bitrateMbps * 3600 * 24) / 8 / 1024;

        // 总存储需求(GB)
        double totalStorageGB = storagePerCameraPerDayGB * cameraCount * days;

        return totalStorageGB;
    }

}
