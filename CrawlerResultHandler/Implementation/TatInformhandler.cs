// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KhabarHandler.cs" company="Ingenius Systems">
//   Copyright (c) Ingenius Systems
//   Create on 18:46:35 by Еламан Абдуллин
// </copyright>
// <summary>
//   Defines the KhabarHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.IO;
using CrawlerResultHandler.Interfaces;

namespace CrawlerResultHandler.Implementation
{
    public class TatInformhandler : AbstractHtmlHandler
    {
        protected override string GetName()
        {
            return "tatiform.ru";
        }

        protected override string GetPath()
        {
            return "//div[@class = \"news_padd\"]";
        }
    }
}