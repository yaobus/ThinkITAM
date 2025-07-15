using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ThinkITAM.Functions.FunctionClass;

/// <summary>
/// 判断协议类型
/// </summary>
public class ProtocolDetector
{

    public static ProtocolType DetectProtocol(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return ProtocolType.Unknown;

        input = input.Trim();

        // 正则表达式匹配各种协议前缀
        if (Regex.IsMatch(input, @"^https?://", RegexOptions.IgnoreCase))
        {
            return input.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
                ? ProtocolType.HTTPS
                : ProtocolType.HTTP;
        }

        if (input.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
        {
            return ProtocolType.FILE;
        }

        if (input.StartsWith("ftp://", StringComparison.OrdinalIgnoreCase))
        {
            return ProtocolType.FTP;
        }

        // SMB/UNC 路径：以 \\ 开头 或者长路径格式 \\?\UNC\...
        if (input.StartsWith(@"\\", StringComparison.Ordinal))
        {
            return ProtocolType.SMB;
        }

        if (input.StartsWith(@"\\?\")) // 长路径格式
        {
            string longPath = input.Substring(4);
            if (longPath.StartsWith(@"UNC\", StringComparison.OrdinalIgnoreCase))
            {
                return ProtocolType.SMB;
            }
        }

        // 可选：识别其他常见协议，如 smb://、nfs:// 等
        var protocolMatch = Regex.Match(input, @"^([a-zA-Z][a-zA-Z0-9+\.-]+)://");
        if (protocolMatch.Success)
        {
            string scheme = protocolMatch.Groups[1].Value.ToLower();
            return scheme switch
            {
                "http" => ProtocolType.HTTP,
                "https" => ProtocolType.HTTPS,
                "file" => ProtocolType.FILE,
                "ftp" => ProtocolType.FTP,
                _ => ProtocolType.OTHER
            };
        }

        return ProtocolType.Unknown;
    }


}



public enum ProtocolType
{
    Unknown,
    HTTP,
    HTTPS,
    FILE,
    SMB,      // 包括 UNC 路径
    FTP,
    OTHER     // 其他支持的协议
}