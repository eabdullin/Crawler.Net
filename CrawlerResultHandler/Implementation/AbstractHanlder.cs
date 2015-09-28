using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace CrawlerResultHandler.Implementation
{
    public abstract class AbstractHanlder
    {
        protected static readonly object Monitor = new object();
        protected abstract string GetName();
        public Task<int> HandleAsync(string folderName, FileStream output)
        {

            Task<int> task = new Task<int>(() =>
            {
                Console.WriteLine("{0} handling started", GetName());
                StreamWriter writer = new StreamWriter(output);

                DirectoryInfo directoryInfo = new DirectoryInfo(folderName);
                int count = 0;
                HandleDirectory(directoryInfo, writer, ref count);
                Console.WriteLine("{0} handling end. Files count {1}", GetName(), count);
                return count;
            });
            task.Start();
            return task;
        }

        protected abstract bool HandleFile(FileInfo fileInfo, StreamWriter writer, ref int sum);
        private void HandleDirectory(DirectoryInfo directoryInfo, StreamWriter writer, ref int sum)
        {
            if (!directoryInfo.Exists) throw new ArgumentException("folderName is not folder or not exists");
            FileInfo[] files = directoryInfo.GetFiles();
            int progress = 0;
            foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
            {
                HandleDirectory(directory, writer, ref sum);
            }
            foreach (FileInfo file in files)
            {
                try
                {
                    if (!HandleFile(file, writer, ref sum))
                    {
                        break;
                    }
                }
                catch (Exception e)
                {
                    int x;
                }

            }

        }
    }
}
