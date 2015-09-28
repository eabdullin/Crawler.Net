// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TengriNewsHandler.cs" company="Ingenius Systems">
//   Copyright (c) Ingenius Systems
//   Create on 19:30:35 by Еламан Абдуллин
// </copyright>
// <summary>
//   Defines the TengriNewsHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CrawlerResultHandler.Implementation
{
    public class TengriNewsHandler:AbstractHtmlHandler
    {
        protected override string GetName()
        {
            return "TengriNews";
        }

        protected override string GetPath()
        {
            return "//div[@class = \"data clearAfter\"]";
        }
    }
}