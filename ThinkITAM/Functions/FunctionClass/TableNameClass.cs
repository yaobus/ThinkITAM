using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkITAM.Functions.FunctionClass
{
    public class TableNameClass
    {

        /// <summary>
        /// 根据资产ID获取表名
        /// </summary>
        /// <param name="AssetId"></param>
        /// <returns></returns>
        public static string GetTableName(string AssetId)
        {
            if (string.IsNullOrEmpty(AssetId))
                return string.Empty;

            switch (AssetId[0])
            {
                case '3':
                    return $"Ra_{AssetId}";
                case '2':
                    return $"De_{AssetId}";
                case '8':
                    return $"Bu_{AssetId}";
                default:
                    return string.Empty; // 或者你可以返回一个默认值，如 null 或其他
            }
        }

    }
}
