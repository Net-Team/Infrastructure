using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Infrastructure.Res
{
    /// <summary>
    /// 表示Ftp客户端
    /// </summary>
    public class FtpClient
    {
        /// <summary>
        /// 获取根目录
        /// </summary>
        public Uri FtpRoot { get; private set; }

        /// <summary>
        /// Ftp客户端
        /// </summary>
        /// <param name="ftpRoot">根目录</param>
        public FtpClient(Uri ftpRoot)
        {
            this.FtpRoot = ftpRoot;
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public async Task<bool> DeleteFileAsync(string fileName)
        {
            if (Path.IsPathRooted(fileName))
            {
                fileName = fileName.Replace(Path.GetPathRoot(fileName), null);
            }

            var fileURL = default(Uri);
            if (Uri.TryCreate(fileName, UriKind.Absolute, out fileURL))
            {
                fileName = fileURL.PathAndQuery;
            }

            var ftpFileUri = new Uri(this.FtpRoot, fileName);
            var ftpRequest = (FtpWebRequest)FtpWebRequest.Create(ftpFileUri);
            ftpRequest.Method = WebRequestMethods.Ftp.DeleteFile;
            ftpRequest.UseBinary = true;

            try
            {
                using (await ftpRequest.GetResponseAsync())
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }


        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="stream">数据流</param>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public async Task<Uri> UploadFileAsync(Stream stream, string fileName)
        {
            if (Path.IsPathRooted(fileName))
            {
                fileName = fileName.Replace(Path.GetPathRoot(fileName), null);
            }

            var fileURL = default(Uri);
            if (Uri.TryCreate(fileName, UriKind.Absolute, out fileURL))
            {
                fileName = fileURL.PathAndQuery;
            }

            var path = Path.GetDirectoryName(fileName);
            await this.CreatePathAsync(path);

            var ftpFileUri = new Uri(this.FtpRoot, fileName);
            var ftpRequest = (FtpWebRequest)FtpWebRequest.Create(ftpFileUri);
            ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
            ftpRequest.UseBinary = true;

            var buffer = new byte[8 * 1024];
            using (var requestStream = await ftpRequest.GetRequestStreamAsync())
            {
                var len = 0;
                stream.Position = 0;
                while ((len = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await requestStream.WriteAsync(buffer, 0, len);
                }
            }

            var httpUri = new Uri(string.Format("http://{0}", this.FtpRoot.Host));
            return new Uri(httpUri, fileName);
        }

        /// <summary>
        /// 创建Ftp目录
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        private async Task CreatePathAsync(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            var dirs = new List<string> { path };
            while (string.IsNullOrEmpty((path = Path.GetDirectoryName(path))) == false)
            {
                dirs.Add(path);
            }
            dirs.Reverse();

            foreach (var dir in dirs)
            {
                var ftpPath = new Uri(this.FtpRoot, dir);
                await this.TryCreateDirectoryAsync(ftpPath);
            }
        }

        /// <summary>
        /// 创建Ftp目录
        /// </summary>
        /// <param name="ftpPath">ftp路径</param>
        /// <returns></returns>
        private async Task TryCreateDirectoryAsync(Uri ftpPath)
        {
            var ftpRequest = (FtpWebRequest)FtpWebRequest.Create(ftpPath);
            ftpRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
            try
            {
                using (await ftpRequest.GetResponseAsync()) { }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.FtpRoot.ToString();
        }
    }
}
