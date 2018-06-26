namespace System
{
    /// <summary>
    /// double类型扩展
    /// </summary>
    public static partial class DoubleExtend
    {
        /// <summary>
        /// 转换为人民币格式
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToRMBString(this double value, string unit = "元")
        {
            var precision = "f" + Math.Max(2, value.GetPrecision());
            return value.ToString(precision) + unit;
        }

        /// <summary>
        /// 获取精度 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static int GetPrecision(this double value)
        {
            var str = value.ToString();
            var dotIndex = str.IndexOf('.');
            if (dotIndex < 0)
            {
                return 0;
            }
            return str.Length - dotIndex - 1;
        }
    }
}
