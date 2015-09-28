using System;

namespace Crawler.Net
{
    public class CrawlerBuilder
    {
        private string _startUrl;
        private bool _keepSameUrl;
        private string _exludeExtensions;
        private int _threadCount;
        private int _connectionTimeOut;
        private bool _keepAlive;
        private int _threadSleepIfQueueEmpty = 1000;
        private ICrawlerRepository _crawlerRepository;
        private EventHandler<UrlProcessedEventArgs> _urlProcessed;
        private event EventHandler<UrlProcessingStartedEventArgs> _urlProcessingStarted;
        private CrawlerBuilder()
        {
        }

        public static CrawlerBuilder Create()
        {
            return new CrawlerBuilder();
        }

        public CrawlerBuilder WithRepository(ICrawlerRepository repository)
        {
            _crawlerRepository = repository;
            return this;
        }

        public CrawlerBuilder WithRepository(Action<FileCrawlerRepository> configureRepository)
        {
            FileCrawlerRepository repository = new FileCrawlerRepository();
            if (configureRepository != null)
            {
                configureRepository(repository);
            }
            _crawlerRepository = repository;
            return this;
        }

        public CrawlerBuilder WithKeepSameUrl(bool value)
        {
            _keepSameUrl = value;
            return this;
        }

        public CrawlerBuilder WithStartUrl(string url)
        {
            _startUrl = url;
            return this;
        }

        public CrawlerBuilder WithThreadCount(int count)
        {
            _threadCount = count;
            return this;
        }
        public CrawlerBuilder WithThreadSleepTimeIfQueueEmpty(int count)
        {
            _threadSleepIfQueueEmpty = count;
            return this;
        }

        public CrawlerBuilder WithConnectionTimeOut(int timeout)
        {
            _connectionTimeOut = timeout;
            return this;
        }

        public CrawlerBuilder AddUrlChangeProcessinStartedHadler(EventHandler<UrlProcessingStartedEventArgs> handler)
        {
            if (handler != null) _urlProcessingStarted += handler;
            return this;
        }
        public CrawlerBuilder AddUrlProcessedHadler(EventHandler<UrlProcessedEventArgs> handler)
        {
            if (handler != null) _urlProcessed += handler;
            return this;
        }

        public Crawler Build()
        {
            return new Crawler(
                _startUrl, 
                _keepSameUrl, 
                _exludeExtensions, 
                _threadCount, 
                _connectionTimeOut, 
                _keepAlive,
                _crawlerRepository,
                _urlProcessingStarted,
                _threadSleepIfQueueEmpty,
                _urlProcessed
                );
        }

    }
}
