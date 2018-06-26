using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Infrastructure.Utility
{
    /// <summary>
    /// 验证码生成类
    /// </summary>
    public class ValidCode
    {
        /// <summary>
        /// 字体大小
        /// </summary>
        private int FontSize = 30;
        /// <summary>
        /// 内边距
        /// </summary>
        private int Padding = 8;

        /// <summary>
        /// 文字颜色
        /// </summary>
        private Color[] Colors = { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Orange, Color.Brown, Color.DarkCyan, Color.Purple };

        /// <summary>
        /// 字体
        /// </summary>
        private string[] Fonts = { "Arial", "Georgia" };

        /// <summary>
        /// 验证码辅助类
        /// </summary>
        private ValidCode()
        {
        }

        /// <summary>
        /// 返回一个新的验证码
        /// 默认4位数
        /// </summary>
        /// <returns></returns>
        public static string NewValidCode()
        {
            return NewValidCode(false);
        }

        /// <summary>
        /// 返回一个新的验证码
        /// 默认4位数
        /// </summary>
        /// <param name="numOnly">是否只是字母</param>
        /// <param name="Length">验证码长度</param>
        /// <returns></returns>
        public static string NewValidCode(bool numOnly,int Length = 4)
        {
            var num = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            var letter = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

            var sources = numOnly ? num : num.Concat(letter).ToArray();
            var ran = new Random();
            var chars = Enumerable.Range(0, Length).Select(i => ran.Next(0, sources.Length - 1)).Select(i => sources[i]).ToArray();
            return new string(chars);
        }

        #region Core 暂时不支持 Bitmap

        ///// <summary>
        ///// 正弦曲线Wave扭曲图片（Edit By 51aspx.com）
        ///// </summary>
        ///// <param name="srcBmp">图片路径</param>
        ///// <param name="bXDir">如果扭曲则选择为True</param>
        ///// <param name="dMultValue">波形的幅度倍数，越大扭曲的程度越高，一般为3</param>
        ///// <param name="dPhase">波形的起始相位，取值区间[0-2*PI)</param>
        ///// <returns></returns>
        //private Bitmap TwistImage(Bitmap srcBmp, bool bXDir, double dMultValue, double dPhase)
        //{
        //    var destBmp = new Bitmap(srcBmp.Width, srcBmp.Height);
        //    using (var g = Graphics.FromImage(destBmp))
        //    {
        //        g.Clear(Color.White);
        //        double dBaseAxisLen = bXDir ? (double)destBmp.Height : (double)destBmp.Width;

        //        for (int i = 0; i < destBmp.Width; i++)
        //        {
        //            for (int j = 0; j < destBmp.Height; j++)
        //            {
        //                double dx = 0;
        //                dx = bXDir ? (Math.PI * 2 * (double)j) / dBaseAxisLen : (Math.PI * 2 * (double)i) / dBaseAxisLen;
        //                dx += dPhase;
        //                double dy = Math.Sin(dx);

        //                // 取得当前点的颜色
        //                int nOldX = 0, nOldY = 0;
        //                nOldX = bXDir ? i + (int)(dy * dMultValue) : i;
        //                nOldY = bXDir ? j : j + (int)(dy * dMultValue);

        //                var color = srcBmp.GetPixel(i, j);
        //                if (nOldX >= 0 && nOldX < destBmp.Width && nOldY >= 0 && nOldY < destBmp.Height)
        //                {
        //                    destBmp.SetPixel(nOldX, nOldY, color);
        //                }
        //            }
        //        }
        //        g.DrawRectangle(Pens.Gray, 0, 0, destBmp.Width - 1, destBmp.Height - 1);
        //    }
        //    return destBmp;
        //}

        ///// <summary>
        ///// 创建验证码图片
        ///// </summary>
        ///// <param name="validCode">验证码</param>
        ///// <returns></returns>
        //private Bitmap CreateImage(string validCode)
        //{
        //    var codeWidth = this.FontSize + this.Padding;
        //    var imageWidth = validCode.Length * codeWidth + this.Padding * 2;
        //    var imageHeight = this.FontSize * 2 + this.Padding;

        //    var image = new Bitmap(imageWidth, imageHeight);
        //    using (var g = Graphics.FromImage(image))
        //    {
        //        g.Clear(Color.White);
        //        var ran = new Random();


        //        int left = 0, top = 0, top1 = 1, top2 = 1;
        //        int n1 = (imageHeight - FontSize - Padding * 2);
        //        int n2 = n1 / 4;
        //        top1 = n2;
        //        top2 = n2 * 2;


        //        int cindex, findex;

        //        //随机字体和颜色的验证码字符
        //        for (int i = 0; i < validCode.Length; i++)
        //        {
        //            cindex = ran.Next(Colors.Length - 1);
        //            findex = ran.Next(Fonts.Length - 1);

        //            var f = new Font(Fonts[findex], this.FontSize, System.Drawing.FontStyle.Bold);
        //            var b = new SolidBrush(Colors[cindex]);

        //            if (i % 2 == 1)
        //            {
        //                top = top2;
        //            }
        //            else
        //            {
        //                top = top1;
        //            }

        //            left = i * codeWidth;
        //            g.DrawString(validCode.Substring(i, 1), f, b, left, top);
        //        }
        //    }
        //    return image;
        //}

        ///// <summary>
        ///// 将验证码生成验证图片
        ///// </summary>
        ///// <param name="validCode">验证码</param>
        ///// <param name="difficult">是否困难</param>
        ///// <returns></returns>
        //public static Bitmap ValidCodeToImage(string validCode)
        //{
        //    var helper = new ValidCode();
        //    using (var image = helper.CreateImage(validCode))
        //    {
        //        return helper.TwistImage(image, true, 8, 3);
        //    }
        //}

        #endregion
    }
}
