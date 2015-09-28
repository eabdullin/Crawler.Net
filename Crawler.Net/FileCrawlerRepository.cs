using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Crawler.Net
{
    public class FileCrawlerRepository:ICrawlerRepository
    {
        private FileMode _fileMode = FileMode.Create;
        private string _downloadFolder = ".";

        public string DownloadFolder
        {
            get { return _downloadFolder; }
            set { _downloadFolder = value; }
        }

        public FileMode FileMode
        {
            get { return _fileMode; }
            set { _fileMode = value; }
        }


        public void Save(string url, byte[] value)
        {
            string filePath = this._downloadFolder + @"\" + url;
            string directoryPath = filePath.Substring(0, filePath.LastIndexOf("/", StringComparison.Ordinal));
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath); ;
            using (FileStream streamOut = File.Open(filePath, _fileMode, FileAccess.Write, FileShare.ReadWrite))
            {
                streamOut.Write(value, 0, value.Length);
            }
        }
    }
}
