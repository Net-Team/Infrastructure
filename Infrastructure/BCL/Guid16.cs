namespace System
{
    /// <summary>
    /// 表示16位GUID
    /// </summary>
    public struct Guid16
    {
        /// <summary>
        /// 值
        /// </summary>
        private readonly string value;

        /// <summary>
        /// 16位GUID
        /// </summary>
        /// <param name="value">值</param>
        private Guid16(string value)
        {
            this.value = value;
        }

        /// <summary>
        /// 从字符串转换为Guid16
        /// </summary>
        /// <param name="value">值</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public static Guid16 Parse(string value)
        {
            if (value.IsNullOrEmpty())
            {
                throw new ArgumentNullException();
            }
            if (value.Length != 16)
            {
                throw new ArgumentException();
            }
            return new Guid16(value);
        }

        /// <summary>
        /// 生成新的Guid16
        /// </summary>
        /// <returns></returns>
        public static Guid16 NewGuid()
        {
            var val = GetGuid16();
            return new Guid16(val);
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.value.IsNullOrEmpty())
            {
                return "0000000000000000";
            }
            return this.value;
        }

        /// <summary>
        /// 获取哈希值
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.ToString().ToLower().GetHashCode();
        }

        /// <summary>
        /// 是否相等
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            var isguid = obj is Guid16;
            if (isguid == false)
            {
                return false;
            }
            return string.Equals(obj.ToString(), this.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 获取guid 16
        /// </summary>
        /// <returns></returns>
        private static string GetGuid16()
        {
            var i = 1L;
            foreach (var b in Guid.NewGuid().ToByteArray())
            {
                i *= ((int)b + 1);
            }
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }
    }
}
