using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Infrastructure.Utility
{
    /// <summary>
    /// 提供调试内容输出
    /// </summary>
    public static class Debugger
    {
        /// <summary>
        /// 获取调试输出流
        /// </summary>
        public static readonly TextWriter Out = new DebugWriter();

        /// <summary>
        /// 输出
        /// </summary>
        public static void WriteLine()
        {
            Out.WriteLine();
        }

        /// <summary>
        /// 输出
        /// </summary>
        /// <param name="message">消息</param>
        public static void WriteLine(object message)
        {
            Out.WriteLine(message);
        }

        /// <summary>
        /// 输出
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="args">参数</param>
        public static void WriteLine(string message, params object[] args)
        {
            Out.WriteLine(message, args);
        }


        /// <summary>
        /// 调试流
        /// </summary>
        private class DebugWriter : TextWriter
        {
            /// <summary>
            /// 获取编码
            /// </summary>
            public override Encoding Encoding
            {
                get
                {
                    return Encoding.UTF8;
                }
            }

            /// <summary>
            /// 输出字符串
            /// </summary>
            /// <param name="buffer"></param>
            /// <param name="index"></param>
            /// <param name="count"></param>
            public override void Write(char[] buffer, int index, int count)
            {
                var value = new string(buffer, index, count);
                if (value.EndsWith(Environment.NewLine))
                {
                    value = new string(buffer, index, count - Environment.NewLine.Length);
                }
                System.Diagnostics.Debugger.Log(0, null, value);
            }
        }
    }
}
