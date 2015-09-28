using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using CrawlerResultHandler.Interfaces;
using HtmlAgilityPack;

namespace CrawlerResultHandler.Implementation
{
    internal class NurKzHandler : AbstractHtmlHandler
    {
        protected override string GetName()
        {
            return "Nur.Kz";
        }

        protected override string GetPath()
        {
            return "//div[@itemprop = \"articleBody\" and @class=\"c__article_text\"]";
        }
    }
}
