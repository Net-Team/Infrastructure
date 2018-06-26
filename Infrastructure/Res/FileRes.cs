using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Res
{
    /// <summary>
    /// 表示文件资源
    /// </summary>
    public class FileRes
    {
        /// <summary>
        /// 获取文件名称
        /// </summary>
        public string FileName { get; private set; }


        /// <summary>
        /// 获取数据流
        /// </summary>
        public Stream Stream { get; private set; }


        /// <summary>
        /// 获取扩展名
        /// </summary>
        public string Extension
        {
            get
            {
                return Path.GetExtension(this.FileName);
            }
        }

        /// <summary>
        /// 文件资源
        /// </summary>
        /// <param name="fileRes">文件名</param>
        private FileRes(string fileRes)
        {
            this.FileName = fileRes;
        }

        /// <summary>
        /// 文件资源
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="stream">数据流</param>
        public FileRes(string fileName, Stream stream)
        {
            this.FileName = GetUrlFileName(fileName);
            this.Stream = stream;
        }


        ///// <summary>
        ///// 文件资源
        ///// core 暂不支持  HttpPostedFileBase
        ///// </summary>
        ///// <param name="file">上传的文件</param>
        //public FileRes(HttpPostedFileBase file)
        //    : this(file.FileName, file.InputStream)
        //{
        //}

        /// <summary>
        /// 从文件资源地址解析
        /// </summary>
        /// <param name="fileRes">文件资源地址</param>
        /// <returns></returns>
        public static FileRes Parse(string fileRes)
        {
            return new FileRes(fileRes);
        }

        /// <summary>
        /// 修复文件名可以下载的
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        private static string GetUrlFileName(string fileName)
        {
            if (fileName.IsNullOrEmpty())
            {
                return fileName;
            }

            var dir = Path.GetDirectoryName(fileName);
            var file = string.Join("_", Path.GetFileName(fileName).Matches(@"(\w|\.)+"));
            return Path.Combine(dir, file);
        }

        /// <summary>
        /// 设置路径
        /// </summary>
        /// <param name="paths">路径</param>
        /// <returns></returns>
        private FileRes SetPath(params string[] path)
        {
            var fileName = Path.GetFileName(this.FileName);
            var paths = path.Concat(new[] { fileName }).ToArray();
            this.FileName = Path.Combine(paths);
            return this;
        }

        /// <summary>
        /// 设置路径
        /// </summary>
        /// <param name="typePath">类别目录</param>
        /// <param name="timePath">时间目录</param>
        /// <returns></returns>
        public FileRes SetPath(string typePath, DateTime timePath)
        {
            return this.SetPath(typePath, timePath.ToString("yyyy-MM"));
        }

        /// <summary>
        /// 设置路径
        /// </summary>
        /// <param name="typePath">类别目录</param>
        /// <param name="timePath">时间目录</param>
        /// <param name="ranPath">随机目录</param>
        /// <returns></returns>
        public FileRes SetPath(string typePath, DateTime timePath, Guid16 ranPath)
        {
            return this.SetPath(typePath, timePath.ToString("yyyy-MM"), ranPath.ToString());
        }

        /// <summary>
        /// 保存到ftp
        /// </summary>
        /// <param name="ftpRoot">ftp服务根目录</param>
        /// <returns></returns>
        public virtual async Task<Uri> SaveAsync(Uri ftpRoot)
        {
            var client = new FtpClient(ftpRoot);
            return await client.UploadFileAsync(this.Stream, this.FileName);
        }

        /// <summary>
        /// 从ftp删除
        /// </summary>
        /// <param name="ftpRoot">ftp服务根目录</param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(Uri ftpRoot)
        {
            var client = new FtpClient(ftpRoot);
            return await client.DeleteFileAsync(this.FileName);
        }

        /// <summary>
        /// 字符串显示
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.FileName;
        }
    }
}
