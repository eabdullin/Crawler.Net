// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbstractHandler.cs" company="Ingenius Systems">
//   Copyright (c) Ingenius Systems
//   Create on 18:47:04 by Еламан Абдуллин
// </copyright>
// <summary>
//   Defines the AbstractHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CrawlerResultHandler.Interfaces;
using HtmlAgilityPack;

namespace CrawlerResultHandler.Implementation
{
    public abstract class AbstractHtmlHandler : AbstractHanlder
    {
        
        private static string StripHTML(string inputString)
        {
            return inputString.Replace("&nbsp;", " ").Replace("&#160;", " ");
        }

        protected abstract string GetPath();

        protected override bool HandleFile(FileInfo fileInfo,StreamWriter writer, ref int sum)
        {
            HtmlDocument document = new HtmlDocument();
            document.Load(fileInfo.FullName, Encoding.UTF8);
            string xpath = GetPath();
            HtmlNode node = document.DocumentNode.SelectSingleNode(xpath);
            if (node != null && !string.IsNullOrEmpty(node.InnerText.Trim()))
            {
                string text = StripHTML(node.InnerText);
                //text = StripTabs(text);
                text = TextHelper.StripNewlines(text);
                text = TextHelper.ReplaceCases(text);
                lock (Monitor)
                {
                    writer.Write(text);
                }

                sum++;
                Console.Write("\r{0}: {1} files   ", GetName(), sum);
            }
            return true;
        }
    }
}