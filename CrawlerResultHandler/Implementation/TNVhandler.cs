// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TNVhandler.cs" company="Ingenius Systems">
//   Copyright (c) Ingenius Systems
//   Create on 15:48:54 by Еламан Абдуллин
// </copyright>
// <summary>
//   Defines the TNVhandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CrawlerResultHandler.Implementation
{
    public class TNVhandler : AbstractHtmlHandler
    {
        protected override string GetName()
        {
            return "tnv.ru";
        }

        protected override string GetPath()
        {
            return "//div[@class = \"txt_content\"]";
        }
    }
}