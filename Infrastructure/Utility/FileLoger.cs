using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Utility
{
    /// <summary>
    /// 文件日志
    /// </summary>
    public class FileLoger
    {
        /// <summary>
        /// 默认文件夹
        /// </summary>
        private static readonly string rootPath = "Logs";

        /// <summary>
        /// 获取今天的日志文件
        /// </summary>
        public static FileLoger Today(string name = "Log")
        {
            var fileName = string.Format("{0}_{1}", name, DateTime.Today.ToString("yyyyMMdd"));
            return new FileLoger(fileName);
        }

        /// <summary>
        /// 按文件夹分类存存储
        /// </summary>
        /// <returns></returns>
        public static FileLoger Folder(string name = "Info")
        {
            var fileName = string.Format("{0}\\{1}", name, DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            return new FileLoger(fileName);
        }

        /// <summary>
        /// 获取现在的日志文件
        /// </summary>
        public static FileLoger Now(string name = "Log")
        {
            var fileName = string.Format("{0}_{1}", name, DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            return new FileLoger(fileName);
        }

        /// <summary>
        /// 日志文件同步锁
        /// </summary>
        private static readonly object syncRoot = new object();

        /// <summary>
        /// 文件路径
        /// </summary>
        private readonly string fileFullName;

        /// <summary>
        /// 文件名称
        /// </summary>
        private readonly string filename;

        /// <summary>
        /// 内容
        /// </summary>
        private readonly StringBuilder sb = new StringBuilder();

        /// <summary>
        /// 日志
        /// </summary>
        /// <param name="Name">文件名</param>
        public FileLoger(string Name)
        {
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }

            this.filename = Name;
            if (Name == null)
            {
                throw new ArgumentNullException();
            }

            if (Path.GetExtension(Name).IsNullOrEmpty())
            {
                Name = Name + ".txt";
            }

            //if (HttpContext.Current != null)
            //{
            //    Name = HttpContext.Current.Server.MapPath("~/" + Name);
            //}

            var dir = Path.GetDirectoryName(Name);
            if (dir.IsNullOrEmpty() == false)
            {
                Directory.CreateDirectory(Path.Combine(rootPath, dir));
            }
            this.fileFullName = Path.Combine(rootPath, Name);
        }

        /// <summary>
        /// 追加日志
        /// </summary>
        /// <param name="log">日志</param>
        /// <returns></returns>
        public FileLoger Append(object log)
        {
            if (log != null)
            {
                this.sb.Append(log.ToString());
            }
            return this;
        }

        /// <summary>
        /// 追加日志
        /// </summary>
        /// <param name="log">日志</param>
        /// <param name="args">参数</param>
        /// <returns></returns>
        public FileLoger Append(object log, params object[] args)
        {
            if (log != null)
            {
                var logTxt = string.Format(log.ToString(), args);
                this.sb.Append(logTxt);
            }
            return this;
        }

        /// <summary>
        /// 追加日志
        /// </summary>
        /// <param name="log">日志</param>
        /// <param name="args">参数</param>
        /// <returns></returns>
        public FileLoger AppendLine()
        {
            this.sb.AppendLine();
            return this;
        }

        /// <summary>
        /// 追加日志
        /// </summary>
        /// <param name="log">日志</param>
        /// <returns></returns>
        public FileLoger AppendLine(object log)
        {
            if (log != null)
            {
                this.sb.AppendLine(log.ToString());
            }
            return this;
        }

        /// <summary>
        /// 追加日志
        /// </summary>
        /// <param name="log">日志</param>
        /// <param name="args">参数</param>
        /// <returns></returns>
        public FileLoger AppendLine(object log, params object[] args)
        {
            if (log != null)
            {
                var logTxt = string.Format(log.ToString(), args);
                this.sb.AppendLine(logTxt);
            }
            return this;
        }

        /// <summary>
        /// 追加日志
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model">模型</param>
        /// <returns></returns>
        public FileLoger AppendLineAsJson<T>(T model)
        {
            var json = JsonConvert.SerializeObject(model);
            return this.AppendLine(json);
        }

        /// <summary>
        /// 保存
        /// </summary>
        public void Save()
        {
            lock (syncRoot)
            {
                File.AppendAllText(this.fileFullName, this.sb.ToString());
                sb.Clear();
            }
        }
    }
}
