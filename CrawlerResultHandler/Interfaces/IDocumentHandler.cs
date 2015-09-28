using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CrawlerResultHandler.Interfaces
{
    interface IDocumentHandler
    {
        Task<int> HandleAsync(string folderName, FileStream output);
    }

}
