using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using HtmlAgilityPack;

namespace CrawlerResultHandler.Implementation
{
    class XmlHandler : AbstractHanlder
    {
        private int _wordCount = 0;
        protected override string GetName()
        {
            return "XmlRus";
        }

        protected override bool HandleFile(FileInfo fileInfo, StreamWriter writer, ref int sum)
        {
            XmlDocument document = new XmlDocument();
            document.Load(fileInfo.FullName);
            string xpath = "//column[@name = \"txt\"]";
            XmlNodeList nodes = document.SelectNodes(xpath);
            if (nodes != null)
            {
                foreach (XmlNode node in nodes)
                {
                    if (node != null && !string.IsNullOrEmpty(node.InnerText.Trim()))
                    {
                        string text = TextHelper.StripNewlines(node.InnerText);
                        text = TextHelper.ReplaceCases(text);
                        _wordCount += Regex.Matches(text, @"[\S]+").Count;
                        lock (Monitor)
                        {
                            writer.Write(text);
                        }
                        sum++;
                        Console.Write("\r{0}: {1} files   ", GetName(), sum);
                    }
                    if (_wordCount > 1500000) return false;
                }
            }
            
            
            if (_wordCount > 1200000) return false;
            return true;
        }
    }
}
