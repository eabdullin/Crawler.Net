using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.Net
{
    internal class CrawlerWebClient: WebClient
    {
        private readonly int _timeOut;

        public CrawlerWebClient(int timeOut)
        {
            _timeOut = timeOut;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            request.Timeout = _timeOut;
            return base.GetWebResponse(request);
        }
    }
}
