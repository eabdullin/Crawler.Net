using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CrawlerResultHandler
{
    public static class TextHelper
    {
        public static string StripNewlines(string inputString)
        {
            return Regex.Replace(inputString, @"((\n|^)(\r|\t|\f)*(\S\s)?(\r|\t|\f)*)+", "\n").Trim();
        }

        public static string ReplaceCases(string inputString)
        {
            return Regex.Replace(inputString, @"[\;\.\,\?\!\:](\s)","$1").ToLower();
            //return inputString.Replace(", ", " ").Replace("©", String.Empty).Replace("? ", " ").Replace("! ", " ").Replace("; ", " ").Replace(". ", " ").ToLower();
        }
    }
}
