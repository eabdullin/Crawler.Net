// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IVisitor.cs" company="Ingenius Systems">
//   Copyright (c) Ingenius Systems
//   Create on 16:38:37 by Еламан Абдуллин
// </copyright>
// <summary>
//   Defines the IVisitor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace CrawlerResultHandler
{
    public interface IVisitor
    {
        void Visit();
    }
}