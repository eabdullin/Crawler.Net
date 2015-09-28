using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Crawler.Net
{
    public class Crawler
    {
        private string _startUrl;
        private readonly bool _keepSameUrl;
        private string _exludeExtensions;
        private readonly int _threadCount;
        private readonly int _connectionTimeOut;
        private bool _keepAlive;
        private readonly int _threadSleepIfQueueEmpty;
        private Queue<Uri> _urlQueue;
        private List<Uri> _processedUris;
        private readonly ICrawlerRepository _crawlerRepository = new FileCrawlerRepository();
        private readonly object _repositoryMonitor = new object();
        private readonly object _pauseMonitor = new object();
        private readonly object _uriProcessingMonitor = new object();
        private Thread[] _threads;
        private bool _started;
        private bool _paused;
        private string _strRef = @"(href|HREF|src|SRC)[ ]*=[ ]*[""'][^""'#>]+[""']";
        private string[] _extArray = { ".gif", ".jpg", ".css", ".zip", ".exe" };
        private string _defaultpage = "_default.html";
        private readonly EventHandler<UrlProcessingStartedEventArgs> _urlProcessingStarted;
        private readonly EventHandler<UrlProcessedEventArgs> _urlProcessed;

        public Crawler(string startUrl, bool keepSameUrl, string exludeExtensions, int threadCount,
            int connectionTimeOut, bool keepAlive, ICrawlerRepository crawlerRepository,
            EventHandler<UrlProcessingStartedEventArgs> urlProcessingStarted, int threadSleepIfQueueEmpty, EventHandler<UrlProcessedEventArgs> urlProcessed)
        {
            _startUrl = startUrl;
            _keepSameUrl = keepSameUrl;
            _exludeExtensions = exludeExtensions;
            _threadCount = threadCount;
            _connectionTimeOut = connectionTimeOut;
            _keepAlive = keepAlive;
            _crawlerRepository = crawlerRepository;
            _urlProcessingStarted = urlProcessingStarted;
            _threadSleepIfQueueEmpty = threadSleepIfQueueEmpty;
            _urlProcessed = urlProcessed;
        }

        public int FileCount { get; private set; }

        public int UrlCount { get; private set; }

        private void UrlToNormal(ref string url)
        {
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                url = string.Concat("http://", url);
            }
            //if (!url.Substring(url.LastIndexOf("/", StringComparison.Ordinal)).Contains("."))
            //    url = string.Concat(url, "/");
        }

        private void InitStartValues()
        {
            FileCount = 0;
            UrlCount = 0;
            _started = false;
            if (_urlQueue == null || _urlQueue.Count > 0)
            {
                _urlQueue = new Queue<Uri>();
            }
            if (_processedUris == null || _processedUris.Count > 0)
            {
                _processedUris = new List<Uri>();
            }
            UrlToNormal(ref _startUrl);
            Enqueue(new Uri(_startUrl));
        }

        public void Start()
        {
            if (!_started)
            {
                InitStartValues();
                _threads = new Thread[_threadCount];
                for (int i = 0; i < _threadCount; i++)
                {
                    _threads[i] = new Thread(ThreadStartFunction) {Name = i.ToString()};
                    _threads[i].Start();
                }
                _started = true;
                return;
            }
            if (_paused)
            {
                _paused = false;
                lock (_pauseMonitor)
                {
                    
                }
                Monitor.PulseAll(_pauseMonitor);
            }
        }

        public void Stop()
        {
            _started = false;
            _paused = false;
            foreach (Thread t in _threads)
            {
                t.Join();
            }
        }

        public void Pause()
        {
            _paused = true;
        }

        private void ThreadStartFunction()
        {
            WebClient client = new CrawlerWebClient(_connectionTimeOut);
            string threadName = Thread.CurrentThread.Name + "";
            int tryCount = 0;
            while (_started)
            {
                if (_paused)
                {
                    lock (_pauseMonitor)
                    {
                        Monitor.Wait(_pauseMonitor);
                    }
                }
                Uri uri;
                if (Dequeue(out uri))
                {
                    if (uri != null)
                    {
                        if (_urlProcessingStarted != null)
                        {
                            lock (_uriProcessingMonitor)
                            {
                                _urlProcessingStarted(this, new UrlProcessingStartedEventArgs
                                {
                                    ThreadName = threadName,
                                    Url = uri.AbsoluteUri
                                });
                            }
                        }
                        ParseUri(uri, client);
                    }
                }
                else if(tryCount < 100)
                {
                    tryCount++;
                }
                else
                {
                    Thread.Sleep(_threadSleepIfQueueEmpty);
                    tryCount = 0;
                }
            }
        }

        private void ParseUri(Uri uri, WebClient client)
        {
            if (client == null) throw new ArgumentNullException("client");
            if (uri == null) return;
            try
            {

                string urlDecode = HttpUtility.UrlDecode(uri.LocalPath);
                if (urlDecode == null) return;

                string strLocalPath = urlDecode.Replace("%20", " ").Replace(":", "_");
                if (strLocalPath.EndsWith("/") || !uri.Segments.Last().Contains("."))
                    // check if there is no query like (.asp?i=32&j=212)
                    if (uri.Query == "")
                        // add a default name for / ended pathes
                        strLocalPath += _defaultpage;
                if (uri.Query != "")
                    // construct the name from the query hash value to be the same if we download it again
                    strLocalPath += uri.Query.GetHashCode() + ".html";

                byte[] bytesResponse = client.DownloadData(uri);
                UrlCount++;
                if (!_started) return;
                string strResponse = Encoding.UTF8.GetString(bytesResponse);
                if (_paused)
                {
                    Monitor.Wait(_pauseMonitor);
                }

                MatchCollection matches = Regex.Matches(strResponse, _strRef, RegexOptions.IgnoreCase);
                foreach (Match match in matches)
                {
                    _strRef = match.Value.Substring(match.Value.IndexOf('=') + 1).Trim('"', '\'', '#', ' ', '>');
                    try
                    {
                        if (_strRef.IndexOf("..", StringComparison.Ordinal) != -1 || _strRef.StartsWith("/") ||
                            !_strRef.StartsWith("http://") || !_strRef.StartsWith("https://"))
                            _strRef = new Uri(uri, _strRef).AbsoluteUri;
                        UrlToNormal(ref _strRef);
                        Uri newUri = new Uri(_strRef);
                        if (newUri.Scheme != Uri.UriSchemeHttp && newUri.Scheme != Uri.UriSchemeHttps)
                            continue;
                        if (newUri.Host != uri.Host && _keepSameUrl)
                            continue;
                        _urlQueue.Enqueue(newUri);
                        Enqueue(newUri);
                    }
                    catch (Exception e)
                    {
                        int x;
                    }
                }

                lock (_repositoryMonitor)
                {
                    _crawlerRepository.Save(uri.Host + strLocalPath, bytesResponse);
                    FileCount++;
                }
                string threadName = Thread.CurrentThread.Name;
                if (_urlProcessed != null)
                {
                    _urlProcessed(this, new UrlProcessedEventArgs()
                    {
                        FileCount = FileCount,
                        ThreadName = threadName,
                        Url = uri.AbsoluteUri
                    });
                }
            }
            catch (Exception)
            {
            }
            // check for response extention
        }

        private void Enqueue(Uri uri)
        {
            Monitor.Enter(_processedUris);
            if (!_processedUris.Contains(uri))
            {
                Monitor.Enter(_urlQueue);
                try
                {
                    _urlQueue.Enqueue(uri);
                    _processedUris.Add(uri);
                }
                catch (Exception)
                {
                }
                
                Monitor.Exit(_urlQueue);
                
            }
            Monitor.Exit(_processedUris);
        }
        private bool  Dequeue(out Uri uri)
        {
            Monitor.Enter(_urlQueue);
            try
            {
                if (_urlQueue.Count > 0)
                {
                    uri = _urlQueue.Dequeue();
                    return true;
                    
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                Monitor.Exit(_urlQueue);
            }
            uri = null;
            return false;
        }
    }

    public class UrlProcessingStartedEventArgs : EventArgs
    {
        public string Url { get; set; }
        public string ThreadName { get; set; }
    }
    public class UrlProcessedEventArgs : EventArgs
    {
        public string Url { get; set; }
        public string ThreadName { get; set; }
        public int FileCount { get; set; }
    }
}
