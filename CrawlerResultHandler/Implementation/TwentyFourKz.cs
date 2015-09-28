// --------------------------------------------------------------------------------------------------------------------
// <copyright file="24Kzhandler.cs" company="Ingenius Systems">
//   Copyright (c) Ingenius Systems
//   Create on 19:09:27 by Еламан Абдуллин
// </copyright>
// <summary>
//   Defines the 24Kzhandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CrawlerResultHandler.Implementation
{
    public class TwentyFourKz : AbstractHtmlHandler
    {
        protected override string GetName()
        {
            return "24.kz";
        }

        protected override string GetPath()
        {
            return "//div[id = \"gkMainbodyWrap\"]";
        }
    }
}