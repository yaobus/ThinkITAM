using SharedImpl = ThinkITAM.Shared.Calculations.StorageCalculator;

namespace ThinkITAM.Functions.FunctionClass;

/// <summary>
/// 监控存储计算器
/// </summary>
public class StorageCalculator
{
    /// <summary>
    /// 计算存储需求
    /// </summary>
    public static double CalculateStorageRequirement(string codec, string resolution, int cameraCount, int days)
        => SharedImpl.CalculateStorageRequirement(codec, resolution, cameraCount, days);

    /// <summary>
    /// 根据码率计算存储需求
    /// </summary>
    public static double CalculateStorageRequirementByBitrate(double bitrateKbps, int cameraCount, int days)
        => SharedImpl.CalculateStorageRequirementByBitrate(bitrateKbps, cameraCount, days);
}
