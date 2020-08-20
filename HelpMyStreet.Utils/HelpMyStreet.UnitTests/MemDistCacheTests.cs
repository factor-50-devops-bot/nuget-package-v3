using Microsoft.Extensions.Internal;
using Moq;
using NUnit.Framework;
using Polly.Caching;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Cache;
using HelpMyStreet.Cache.MemCache;
using HelpMyStreet.Cache.MemDistCache;
using HelpMyStreet.Cache.Models;
using HelpMyStreet.Utils.Utils;

namespace HelpMyStreet.UnitTests
{
    public class MemDistCacheTests
    {
        private Mock<ISyncCacheProvider> _pollySyncCacheProvider;
        private Mock<IDistributedCacheWrapper> _distributedCacheWrapper;
        private Mock<ISystemClock> _mockableDateTime;

        private Mock<ILoggerWrapper<MemDistCache<string>>> _logger;
        
        private int _numberOfTimesDataGetterDelegate1Called;
        private Func<CancellationToken, Task<string>> _dataGetterDelegate1;


        private int _waitForBackgroundThreadToCompleteMs = 100;

        private readonly TimeSpan _defaultCacheDuration = new TimeSpan(28, 0, 0, 0);

        private string _key = "key";

        [SetUp]
        public void SetUp()
        {
            _numberOfTimesDataGetterDelegate1Called = 0;
            _dataGetterDelegate1 = async (token) =>
            {
                Interlocked.Increment(ref _numberOfTimesDataGetterDelegate1Called);
                await Task.Delay(25);
                return await Task.FromResult("dataFromBackendGet");
            };

            _mockableDateTime = new Mock<ISystemClock>();

            _pollySyncCacheProvider = new Mock<ISyncCacheProvider>();

            _pollySyncCacheProvider.Setup(x => x.TryGet(It.IsAny<string>()));
            _pollySyncCacheProvider.Setup(x => x.Put(It.IsAny<string>(), It.IsAny<CachedItemWrapper<string>>(), It.IsAny<Ttl>()));

            _distributedCacheWrapper = new Mock<IDistributedCacheWrapper>();
            _distributedCacheWrapper.Setup(x => x.TryGetAsync<string>(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()));

            _distributedCacheWrapper.Setup(x => x.PutAsync(It.IsAny<string>(), It.IsAny<CachedItemWrapper<string>>(), It.IsAny<Ttl>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()));

            _logger = new Mock<ILoggerWrapper<MemDistCache<string>>>();
            _logger.SetupAllProperties();
        }

        [Test]
        public async Task DataInMemoryCacheAndFresh()
        {
            CachedItemWrapper<string> memoryCacheItem = new CachedItemWrapper<string>("fromMemDistCache", new DateTimeOffset(2020, 05, 17, 20, 00, 00, 00, new TimeSpan(0, 0, 0)));

            _pollySyncCacheProvider.Setup(x => x.TryGet(It.IsAny<string>())).Returns((true, memoryCacheItem));

            _mockableDateTime.Setup(x => x.UtcNow).Returns(new DateTimeOffset(2020, 05, 17, 20, 00, 00, 00, new TimeSpan(0, 0, 0)));
            
            MemDistCacheFactory<string> memDistCacheFactory = new MemDistCacheFactory<string>(_pollySyncCacheProvider.Object, _distributedCacheWrapper.Object, _mockableDateTime.Object, _logger.Object);
            IMemDistCache<string> memDistCache = memDistCacheFactory.GetCache(_defaultCacheDuration, Cache.ResetTimeFactory.OnHour);

            string result = await memDistCache.GetCachedDataAsync(_dataGetterDelegate1, _key, RefreshBehaviour.DontWaitForFreshData, CancellationToken.None);


            Assert.AreEqual("fromMemDistCache", result);
            Assert.AreEqual(0, _numberOfTimesDataGetterDelegate1Called);

            _pollySyncCacheProvider.Verify(x => x.TryGet(It.Is<string>(y => y == _key)), Times.Once);
            _pollySyncCacheProvider.Verify(x => x.Put(It.IsAny<string>(), It.IsAny<CachedItemWrapper<string>>(), It.IsAny<Ttl>()), Times.Never);

            _distributedCacheWrapper.Verify(x => x.TryGetAsync<string>(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()), Times.Never);
            _distributedCacheWrapper.Verify(x => x.PutAsync(It.IsAny<string>(), It.IsAny<CachedItemWrapper<string>>(), It.IsAny<Ttl>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()), Times.Never);
        }

        [Test]
        public async Task DataInMemoryCacheButNotFresh()
        {
            CachedItemWrapper<string> memoryCacheItem = new CachedItemWrapper<string>("fromMemDistCache", new DateTimeOffset(2020, 05, 17, 19, 59, 59, 00, new TimeSpan(0, 0, 0)));

            _pollySyncCacheProvider.Setup(x => x.TryGet(It.IsAny<string>())).Returns((true, memoryCacheItem));

            _mockableDateTime.Setup(x => x.UtcNow).Returns(new DateTimeOffset(2020, 05, 17, 20, 00, 00, 00, new TimeSpan(0, 0, 0)));

            MemDistCacheFactory<string> memDistCacheFactory = new MemDistCacheFactory<string>(_pollySyncCacheProvider.Object, _distributedCacheWrapper.Object, _mockableDateTime.Object, _logger.Object);
            IMemDistCache<string> memDistCache = memDistCacheFactory.GetCache(_defaultCacheDuration, Cache.ResetTimeFactory.OnHour);

            string result = await memDistCache.GetCachedDataAsync(_dataGetterDelegate1, _key, RefreshBehaviour.DontWaitForFreshData, CancellationToken.None);

            Assert.AreEqual("fromMemDistCache", result);

            _pollySyncCacheProvider.Verify(x => x.TryGet(It.Is<string>(y => y == _key)), Times.Once);

            _distributedCacheWrapper.Verify(x => x.TryGetAsync<string>(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()), Times.Never);

            await Task.Delay(_waitForBackgroundThreadToCompleteMs); // wait for background thread

            Assert.AreEqual(1, _numberOfTimesDataGetterDelegate1Called);

            DateTimeOffset whenDataWillNotBeFresh = new DateTimeOffset(2020, 05, 17, 21, 00, 00, 00, new TimeSpan(0, 0, 0));

            _pollySyncCacheProvider.Verify(x => x.Put(It.Is<string>(y => y == _key), It.Is<CachedItemWrapper<string>>(y => y.Content == "dataFromBackendGet" && y.IsFreshUntil == whenDataWillNotBeFresh), It.Is<Ttl>(y => y.Timespan == _defaultCacheDuration)), Times.Once);

            _distributedCacheWrapper.Verify(x => x.PutAsync(It.Is<string>(y => y == _key), It.Is<CachedItemWrapper<string>>(y => y.Content == "dataFromBackendGet" && y.IsFreshUntil == whenDataWillNotBeFresh), It.Is<Ttl>(y => y.Timespan == _defaultCacheDuration), It.IsAny<CancellationToken>(), It.IsAny<bool>()), Times.Once);
        }

        [Test]
        public async Task DataInMemoryCacheButNotFresh_WaitForFreshData()
        {
            CachedItemWrapper<string> memoryCacheItem = new CachedItemWrapper<string>("fromMemDistCache", new DateTimeOffset(2020, 05, 17, 19, 59, 59, 00, new TimeSpan(0, 0, 0)));

            _pollySyncCacheProvider.Setup(x => x.TryGet(It.IsAny<string>())).Returns((true, memoryCacheItem));

            _mockableDateTime.Setup(x => x.UtcNow).Returns(new DateTimeOffset(2020, 05, 17, 20, 00, 00, 00, new TimeSpan(0, 0, 0)));

            MemDistCacheFactory<string> memDistCacheFactory = new MemDistCacheFactory<string>(_pollySyncCacheProvider.Object, _distributedCacheWrapper.Object, _mockableDateTime.Object, _logger.Object);
            IMemDistCache<string> memDistCache = memDistCacheFactory.GetCache(_defaultCacheDuration, Cache.ResetTimeFactory.OnHour);

            string result = await memDistCache.GetCachedDataAsync(_dataGetterDelegate1, _key, RefreshBehaviour.WaitForFreshData, CancellationToken.None);

            Assert.AreEqual("dataFromBackendGet", result);

            _pollySyncCacheProvider.Verify(x => x.TryGet(It.Is<string>(y => y == _key)), Times.Once);

            _distributedCacheWrapper.Verify(x => x.TryGetAsync<string>(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()), Times.Never);

            Assert.AreEqual(1, _numberOfTimesDataGetterDelegate1Called);

            DateTimeOffset whenDataWillNotBeFresh = new DateTimeOffset(2020, 05, 17, 21, 00, 00, 00, new TimeSpan(0, 0, 0));

            _pollySyncCacheProvider.Verify(x => x.Put(It.Is<string>(y => y == _key), It.Is<CachedItemWrapper<string>>(y => y.Content == "dataFromBackendGet" && y.IsFreshUntil == whenDataWillNotBeFresh), It.Is<Ttl>(y => y.Timespan == _defaultCacheDuration)), Times.Once);

            _distributedCacheWrapper.Verify(x => x.PutAsync(It.Is<string>(y => y == _key), It.Is<CachedItemWrapper<string>>(y => y.Content == "dataFromBackendGet" && y.IsFreshUntil == whenDataWillNotBeFresh), It.Is<Ttl>(y => y.Timespan == _defaultCacheDuration), It.IsAny<CancellationToken>(), It.IsAny<bool>()), Times.Once);
        }

        [Test]
        public async Task DataInMemoryCacheButNotFresh_DontRefreshData()
        {
            CachedItemWrapper<string> memoryCacheItem = new CachedItemWrapper<string>("fromMemDistCache", new DateTimeOffset(2020, 05, 17, 19, 59, 59, 00, new TimeSpan(0, 0, 0)));

            _pollySyncCacheProvider.Setup(x => x.TryGet(It.IsAny<string>())).Returns((true, memoryCacheItem));

            _mockableDateTime.Setup(x => x.UtcNow).Returns(new DateTimeOffset(2020, 05, 17, 20, 00, 00, 00, new TimeSpan(0, 0, 0)));

            MemDistCacheFactory<string> memDistCacheFactory = new MemDistCacheFactory<string>(_pollySyncCacheProvider.Object, _distributedCacheWrapper.Object, _mockableDateTime.Object, _logger.Object);
            IMemDistCache<string> memDistCache = memDistCacheFactory.GetCache(_defaultCacheDuration, Cache.ResetTimeFactory.OnHour);

            string result = await memDistCache.GetCachedDataAsync(_dataGetterDelegate1, _key, RefreshBehaviour.DontRefreshData, CancellationToken.None);

            Assert.AreEqual("fromMemDistCache", result);

            _pollySyncCacheProvider.Verify(x => x.TryGet(It.Is<string>(y => y == _key)), Times.Once);

            _distributedCacheWrapper.Verify(x => x.TryGetAsync<string>(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()), Times.Never);

            await Task.Delay(_waitForBackgroundThreadToCompleteMs); // wait for background thread

            Assert.AreEqual(0, _numberOfTimesDataGetterDelegate1Called);
            
            _pollySyncCacheProvider.Verify(x => x.Put(It.IsAny<string>(), It.IsAny<CachedItemWrapper<string>>(), It.Is<Ttl>(y => y.Timespan == _defaultCacheDuration)), Times.Never);

            _distributedCacheWrapper.Verify(x => x.PutAsync(It.IsAny<string>(), It.IsAny<CachedItemWrapper<string>>(), It.Is<Ttl>(y => y.Timespan == _defaultCacheDuration), It.IsAny<CancellationToken>(), It.IsAny<bool>()), Times.Never);
        }

        [Test]
        public async Task DataInMemoryCacheButNotFresh_BackendGetErrors()
        {
            CachedItemWrapper<string> memoryCacheItem = new CachedItemWrapper<string>("fromMemDistCache", new DateTimeOffset(2020, 05, 17, 19, 59, 59, 00, new TimeSpan(0, 0, 0)));

            _pollySyncCacheProvider.Setup(x => x.TryGet(It.IsAny<string>())).Returns((true, memoryCacheItem));

            _mockableDateTime.Setup(x => x.UtcNow).Returns(new DateTimeOffset(2020, 05, 17, 20, 00, 00, 00, new TimeSpan(0, 0, 0)));

            Func<CancellationToken, Task<string>> _dataGetterDelegate1 =  (token) =>
            {
                _numberOfTimesDataGetterDelegate1Called++;
                throw new Exception("An error");
            };

            MemDistCacheFactory<string> memDistCacheFactory = new MemDistCacheFactory<string>(_pollySyncCacheProvider.Object, _distributedCacheWrapper.Object, _mockableDateTime.Object, _logger.Object);
            IMemDistCache<string> memDistCache = memDistCacheFactory.GetCache(_defaultCacheDuration, Cache.ResetTimeFactory.OnHour);

            string result = await memDistCache.GetCachedDataAsync(_dataGetterDelegate1, _key, RefreshBehaviour.DontWaitForFreshData, CancellationToken.None);

            Assert.AreEqual("fromMemDistCache", result);

            _pollySyncCacheProvider.Verify(x => x.TryGet(It.IsAny<string>()), Times.Once);

            _distributedCacheWrapper.Verify(x => x.TryGetAsync<string>(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<bool>()), Times.Never);

            await Task.Delay(_waitForBackgroundThreadToCompleteMs); // wait for background thread

            _logger.Verify(x=>x.LogError(It.Is<string>(y=> y== $"Error executing data getter for key: {_key}"), It.IsAny<Exception>()));
            
            Assert.AreEqual(1, _numberOfTimesDataGetterDelegate1Called);

            DateTimeOffset whenDataWillNotBeFresh = new DateTimeOffset(2020, 05, 17, 21, 00, 00, 00, new TimeSpan(0, 0, 0));

            _pollySyncCacheProvider.Verify(x => x.Put(It.IsAny<string>(), It.Is<CachedItemWrapper<string>>(y => y.Content == "dataFromBackendGet" && y.IsFreshUntil == whenDataWillNotBeFresh), It.Is<Ttl>(y => y.Timespan == _defaultCacheDuration)), Times.Never);

            _distributedCacheWrapper.Verify(x => x.PutAsync(It.IsAny<string>(), It.Is<CachedItemWrapper<string>>(y => y.Content == "dataFromBackendGet" && y.IsFreshUntil == whenDataWillNotBeFresh), It.Is<Ttl>(y => y.Timespan == _defaultCacheDuration), It.IsAny<CancellationToken>(), It.IsAny<bool>()), Times.Never);
        }


        [Test]
        public async Task DataNotInMemoryCache_DataInDistCacheFresh()
        {
            CachedItemWrapper<string> distCacheItem = new CachedItemWrapper<string>("fromDistCache", new DateTimeOffset(2020, 05, 17, 20, 00, 00, 00, new TimeSpan(0, 0, 0)));

            _pollySyncCacheProvider.Setup(x => x.TryGet(It.IsAny<string>())).Returns((false, null));

            _distributedCacheWrapper.Setup(x => x.TryGetAsync<CachedItemWrapper<string>>(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<bool>())).ReturnsAsync((true, distCacheItem));


            _mockableDateTime.Setup(x => x.UtcNow).Returns(new DateTimeOffset(2020, 05, 17, 20, 00, 00, 00, new TimeSpan(0, 0, 0)));

            MemDistCacheFactory<string> memDistCacheFactory = new MemDistCacheFactory<string>(_pollySyncCacheProvider.Object, _distributedCacheWrapper.Object, _mockableDateTime.Object, _logger.Object);
            IMemDistCache<string> memDistCache = memDistCacheFactory.GetCache(_defaultCacheDuration, Cache.ResetTimeFactory.OnHour);

            string result = await memDistCache.GetCachedDataAsync(_dataGetterDelegate1, _key, RefreshBehaviour.DontWaitForFreshData, CancellationToken.None);

            Assert.AreEqual("fromDistCache", result);
            Assert.AreEqual(0, _numberOfTimesDataGetterDelegate1Called);

            _pollySyncCacheProvider.Verify(x => x.TryGet(It.Is<string>(y => y == _key)), Times.Once);

            _distributedCacheWrapper.Verify(x => x.TryGetAsync<CachedItemWrapper<string>>(It.Is<string>(y => y == _key), It.IsAny<CancellationToken>(), It.IsAny<bool>()), Times.Once);

            await Task.Delay(_waitForBackgroundThreadToCompleteMs); // wait for background thread

            Assert.AreEqual(0, _numberOfTimesDataGetterDelegate1Called);

            _pollySyncCacheProvider.Verify(x => x.Put(It.Is<string>(y => y == _key), It.Is<CachedItemWrapper<string>>(y => y.Content == "fromDistCache" && y.IsFreshUntil == distCacheItem.IsFreshUntil), It.Is<Ttl>(y => y.Timespan == _defaultCacheDuration)), Times.Once);

            _distributedCacheWrapper.Verify(x => x.PutAsync(It.IsAny<string>(), It.IsAny<CachedItemWrapper<string>>(), It.Is<Ttl>(y => y.Timespan == _defaultCacheDuration), It.IsAny<CancellationToken>(), It.IsAny<bool>()), Times.Never);
        }


        [Test]
        public async Task DataNotInMemoryCache_DataInDistCacheNotFresh()
        {
            CachedItemWrapper<string> distCacheItem = new CachedItemWrapper<string>("fromDistCache", new DateTimeOffset(2020, 05, 17, 19, 59, 59, 00, new TimeSpan(0, 0, 0)));

            _pollySyncCacheProvider.Setup(x => x.TryGet(It.IsAny<string>())).Returns((false, null));

            _distributedCacheWrapper.Setup(x => x.TryGetAsync<CachedItemWrapper<string>>(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<bool>())).ReturnsAsync((true, distCacheItem));


            _mockableDateTime.Setup(x => x.UtcNow).Returns(new DateTimeOffset(2020, 05, 17, 20, 00, 00, 00, new TimeSpan(0, 0, 0)));

            MemDistCacheFactory<string> memDistCacheFactory = new MemDistCacheFactory<string>(_pollySyncCacheProvider.Object, _distributedCacheWrapper.Object, _mockableDateTime.Object, _logger.Object);
            IMemDistCache<string> memDistCache = memDistCacheFactory.GetCache(_defaultCacheDuration, Cache.ResetTimeFactory.OnHour);

            string result = await memDistCache.GetCachedDataAsync(_dataGetterDelegate1, _key, RefreshBehaviour.DontWaitForFreshData, CancellationToken.None);

            Assert.AreEqual("fromDistCache", result);

            _pollySyncCacheProvider.Verify(x => x.TryGet(It.Is<string>(y => y == _key)), Times.Once);

            _distributedCacheWrapper.Verify(x => x.TryGetAsync<CachedItemWrapper<string>>(It.Is<string>(y => y == _key), It.IsAny<CancellationToken>(), It.IsAny<bool>()), Times.Once);

            await Task.Delay(_waitForBackgroundThreadToCompleteMs); // wait for background thread

            Assert.AreEqual(1, _numberOfTimesDataGetterDelegate1Called);


            DateTimeOffset whenDataWillNotBeFresh = new DateTimeOffset(2020, 05, 17, 21, 00, 00, 00, new TimeSpan(0, 0, 0));

            _pollySyncCacheProvider.Verify(x => x.Put(It.Is<string>(y => y == _key), It.Is<CachedItemWrapper<string>>(y => y.Content == "dataFromBackendGet" && y.IsFreshUntil == whenDataWillNotBeFresh), It.Is<Ttl>(y => y.Timespan == _defaultCacheDuration)), Times.Once);

            _distributedCacheWrapper.Verify(x => x.PutAsync(It.Is<string>(y => y == _key), It.Is<CachedItemWrapper<string>>(y => y.Content == "dataFromBackendGet" && y.IsFreshUntil == whenDataWillNotBeFresh), It.Is<Ttl>(y => y.Timespan == _defaultCacheDuration), It.IsAny<CancellationToken>(), It.IsAny<bool>()), Times.Once);
        }

        [Test]
        public async Task DataNotInMemoryCache_DataInDistCacheNotFresh_WaitForFreshData()
        {
            CachedItemWrapper<string> distCacheItem = new CachedItemWrapper<string>("fromDistCache", new DateTimeOffset(2020, 05, 17, 19, 59, 59, 00, new TimeSpan(0, 0, 0)));

            _pollySyncCacheProvider.Setup(x => x.TryGet(It.IsAny<string>())).Returns((false, null));

            _distributedCacheWrapper.Setup(x => x.TryGetAsync<CachedItemWrapper<string>>(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<bool>())).ReturnsAsync((true, distCacheItem));


            _mockableDateTime.Setup(x => x.UtcNow).Returns(new DateTimeOffset(2020, 05, 17, 20, 00, 00, 00, new TimeSpan(0, 0, 0)));

            MemDistCacheFactory<string> memDistCacheFactory = new MemDistCacheFactory<string>(_pollySyncCacheProvider.Object, _distributedCacheWrapper.Object, _mockableDateTime.Object, _logger.Object);
            IMemDistCache<string> memDistCache = memDistCacheFactory.GetCache(_defaultCacheDuration, Cache.ResetTimeFactory.OnHour);

            string result = await memDistCache.GetCachedDataAsync(_dataGetterDelegate1, _key, RefreshBehaviour.WaitForFreshData, CancellationToken.None);

            Assert.AreEqual("dataFromBackendGet", result);

            _pollySyncCacheProvider.Verify(x => x.TryGet(It.Is<string>(y => y == _key)), Times.Once);

            _distributedCacheWrapper.Verify(x => x.TryGetAsync<CachedItemWrapper<string>>(It.Is<string>(y => y == _key), It.IsAny<CancellationToken>(), It.IsAny<bool>()), Times.Once);

            Assert.AreEqual(1, _numberOfTimesDataGetterDelegate1Called);


            DateTimeOffset whenDataWillNotBeFresh = new DateTimeOffset(2020, 05, 17, 21, 00, 00, 00, new TimeSpan(0, 0, 0));

            _pollySyncCacheProvider.Verify(x => x.Put(It.Is<string>(y => y == _key), It.Is<CachedItemWrapper<string>>(y => y.Content == "dataFromBackendGet" && y.IsFreshUntil == whenDataWillNotBeFresh), It.Is<Ttl>(y => y.Timespan == _defaultCacheDuration)), Times.Once);

            _distributedCacheWrapper.Verify(x => x.PutAsync(It.Is<string>(y => y == _key), It.Is<CachedItemWrapper<string>>(y => y.Content == "dataFromBackendGet" && y.IsFreshUntil == whenDataWillNotBeFresh), It.Is<Ttl>(y => y.Timespan == _defaultCacheDuration), It.IsAny<CancellationToken>(), It.IsAny<bool>()), Times.Once);
        }

        [Test]
        public async Task DataNotInMemoryCache_DataInDistCacheNotFresh_DontRefreshData()
        {
            CachedItemWrapper<string> distCacheItem = new CachedItemWrapper<string>("fromDistCache", new DateTimeOffset(2020, 05, 17, 19, 59, 59, 00, new TimeSpan(0, 0, 0)));

            _pollySyncCacheProvider.Setup(x => x.TryGet(It.IsAny<string>())).Returns((false, null));

            _distributedCacheWrapper.Setup(x => x.TryGetAsync<CachedItemWrapper<string>>(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<bool>())).ReturnsAsync((true, distCacheItem));


            _mockableDateTime.Setup(x => x.UtcNow).Returns(new DateTimeOffset(2020, 05, 17, 20, 00, 00, 00, new TimeSpan(0, 0, 0)));

            MemDistCacheFactory<string> memDistCacheFactory = new MemDistCacheFactory<string>(_pollySyncCacheProvider.Object, _distributedCacheWrapper.Object, _mockableDateTime.Object, _logger.Object);
            IMemDistCache<string> memDistCache = memDistCacheFactory.GetCache(_defaultCacheDuration, Cache.ResetTimeFactory.OnHour);

            string result = await memDistCache.GetCachedDataAsync(_dataGetterDelegate1, _key, RefreshBehaviour.DontRefreshData, CancellationToken.None);

            Assert.AreEqual("fromDistCache", result);

            _pollySyncCacheProvider.Verify(x => x.TryGet(It.Is<string>(y => y == _key)), Times.Once);

            _distributedCacheWrapper.Verify(x => x.TryGetAsync<CachedItemWrapper<string>>(It.Is<string>(y => y == _key), It.IsAny<CancellationToken>(), It.IsAny<bool>()), Times.Once);

            await Task.Delay(_waitForBackgroundThreadToCompleteMs); // wait for background thread

            Assert.AreEqual(0, _numberOfTimesDataGetterDelegate1Called);


            _pollySyncCacheProvider.Verify(x => x.Put(It.IsAny<string>(), It.IsAny<CachedItemWrapper<string>>(), It.Is<Ttl>(y => y.Timespan == _defaultCacheDuration)), Times.Never);

            _distributedCacheWrapper.Verify(x => x.PutAsync(It.IsAny<string>(), It.IsAny<CachedItemWrapper<string>>(), It.Is<Ttl>(y => y.Timespan == _defaultCacheDuration), It.IsAny<CancellationToken>(), It.IsAny<bool>()), Times.Never);
        }



        [Test]
        public async Task DataNotInMemoryCache_DataNotInDistCache()
        {
            _pollySyncCacheProvider.Setup(x => x.TryGet(It.IsAny<string>())).Returns((false, null));

            _distributedCacheWrapper.Setup(x => x.TryGetAsync<CachedItemWrapper<string>>(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<bool>())).ReturnsAsync((false, null));


            _mockableDateTime.Setup(x => x.UtcNow).Returns(new DateTimeOffset(2020, 05, 17, 20, 00, 00, 00, new TimeSpan(0, 0, 0)));

            MemDistCacheFactory<string> memDistCacheFactory = new MemDistCacheFactory<string>(_pollySyncCacheProvider.Object, _distributedCacheWrapper.Object, _mockableDateTime.Object, _logger.Object);
            IMemDistCache<string> memDistCache = memDistCacheFactory.GetCache(_defaultCacheDuration, Cache.ResetTimeFactory.OnHour);

            string result = await memDistCache.GetCachedDataAsync(_dataGetterDelegate1, _key, RefreshBehaviour.DontWaitForFreshData, CancellationToken.None);

            Assert.AreEqual("dataFromBackendGet", result);

            _pollySyncCacheProvider.Verify(x => x.TryGet(It.Is<string>(y => y == _key)), Times.Once);

            _distributedCacheWrapper.Verify(x => x.TryGetAsync<CachedItemWrapper<string>>(It.Is<string>(y => y == _key), It.IsAny<CancellationToken>(), It.IsAny<bool>()), Times.Once);

            await Task.Delay(_waitForBackgroundThreadToCompleteMs); // wait for background thread

            Assert.AreEqual(1, _numberOfTimesDataGetterDelegate1Called);


            DateTimeOffset whenDataWillNotBeFresh = new DateTimeOffset(2020, 05, 17, 21, 00, 00, 00, new TimeSpan(0, 0, 0));

            _pollySyncCacheProvider.Verify(x => x.Put(It.Is<string>(y => y == _key), It.Is<CachedItemWrapper<string>>(y => y.Content == "dataFromBackendGet" && y.IsFreshUntil == whenDataWillNotBeFresh), It.Is<Ttl>(y => y.Timespan == _defaultCacheDuration)), Times.Once);

            _distributedCacheWrapper.Verify(x => x.PutAsync(It.Is<string>(y => y == _key), It.Is<CachedItemWrapper<string>>(y => y.Content == "dataFromBackendGet" && y.IsFreshUntil == whenDataWillNotBeFresh), It.Is<Ttl>(y => y.Timespan == _defaultCacheDuration), It.IsAny<CancellationToken>(), It.IsAny<bool>()), Times.Once);
        }

        [Test]
        public async Task RefreshData()
        {

            _mockableDateTime.Setup(x => x.UtcNow).Returns(new DateTimeOffset(2020, 05, 17, 20, 00, 00, 00, new TimeSpan(0, 0, 0)));

            MemDistCacheFactory<string> memDistCacheFactory = new MemDistCacheFactory<string>(_pollySyncCacheProvider.Object, _distributedCacheWrapper.Object, _mockableDateTime.Object, _logger.Object);
            IMemDistCache<string> memDistCache = memDistCacheFactory.GetCache(_defaultCacheDuration, Cache.ResetTimeFactory.OnHour);

            string result = await memDistCache.RefreshDataAsync(_dataGetterDelegate1, _key,  CancellationToken.None);

            Assert.AreEqual("dataFromBackendGet", result);

            Assert.AreEqual(1, _numberOfTimesDataGetterDelegate1Called);

            DateTimeOffset whenDataWillNotBeFresh = new DateTimeOffset(2020, 05, 17, 21, 00, 00, 00, new TimeSpan(0, 0, 0));

            _pollySyncCacheProvider.Verify(x => x.Put(It.Is<string>(y => y == _key), It.Is<CachedItemWrapper<string>>(y => y.Content == "dataFromBackendGet" && y.IsFreshUntil == whenDataWillNotBeFresh), It.Is<Ttl>(y => y.Timespan == _defaultCacheDuration)), Times.Once);

            _distributedCacheWrapper.Verify(x => x.PutAsync(It.Is<string>(y => y == _key), It.Is<CachedItemWrapper<string>>(y => y.Content == "dataFromBackendGet" && y.IsFreshUntil == whenDataWillNotBeFresh), It.Is<Ttl>(y => y.Timespan == _defaultCacheDuration), It.IsAny<CancellationToken>(), It.IsAny<bool>()), Times.Once);
        }

        [Test]
        public async Task MultipleConcurrentRequests()
        {

            int numberOfTimesDataGetterDelegate1Called = 0;
            Func<CancellationToken, Task<string>> dataGetterDelegate1 = async (token) =>
           {
               Interlocked.Increment(ref numberOfTimesDataGetterDelegate1Called);
               await Task.Delay(1000);
               return await Task.FromResult("dataFromBackendGet1");
           };

            int numberOfTimesDataGetterDelegate2Called = 0;
            Func<CancellationToken, Task<string>> dataGetterDelegate2 = async (token) =>
            {
                Interlocked.Increment(ref numberOfTimesDataGetterDelegate2Called);
                await Task.Delay(1000);
                return await Task.FromResult("dataFromBackendGet2");
            };


            _pollySyncCacheProvider.Setup(x => x.TryGet(It.IsAny<string>())).Returns((false, null));

            _mockableDateTime.Setup(x => x.UtcNow).Returns(new DateTimeOffset(2020, 05, 17, 20, 00, 00, 00, new TimeSpan(0, 0, 0)));

            MemDistCacheFactory<string> memDistCacheFactory = new MemDistCacheFactory<string>(_pollySyncCacheProvider.Object, _distributedCacheWrapper.Object, _mockableDateTime.Object, _logger.Object);
            IMemDistCache<string> memDistCache = memDistCacheFactory.GetCache(_defaultCacheDuration, Cache.ResetTimeFactory.OnHour);

            ConcurrentBag<Task<string>> results1 = new ConcurrentBag<Task<string>>();
            ConcurrentBag<Task<string>> results2 = new ConcurrentBag<Task<string>>();

            Stopwatch stopwatch = Stopwatch.StartNew();

            Task task1 = Task.Factory.StartNew(() =>
            {
                Parallel.For(0, 50, i =>
                {
                    Task<string> result = memDistCache.GetCachedDataAsync(dataGetterDelegate1, "key1", RefreshBehaviour.WaitForFreshData, CancellationToken.None);
                    results1.Add(result);
                });
            });


            Task task2 = Task.Factory.StartNew(() =>
            {
                Parallel.For(0, 50, i =>
                {
                    Task<string> result = memDistCache.GetCachedDataAsync(dataGetterDelegate2, "key2", RefreshBehaviour.WaitForFreshData, CancellationToken.None);
                    results2.Add(result);
                });
            });


            Task.WaitAll(task1, task2);

            await Task.WhenAll(results1);
            await Task.WhenAll(results2);

            stopwatch.Stop();

            Assert.Less(stopwatch.ElapsedMilliseconds, 2000); // shows the calls were processed concurrently

            foreach (Task<string> result in results1)
            {
                Assert.AreEqual("dataFromBackendGet1", await result);
            }
            foreach (Task<string> result in results2)
            {
                Assert.AreEqual("dataFromBackendGet2", await result);
            }

            Assert.AreEqual(1, numberOfTimesDataGetterDelegate1Called);
            Assert.AreEqual(1, numberOfTimesDataGetterDelegate2Called);

            await Task.Delay(_waitForBackgroundThreadToCompleteMs); // wait for background thread



            DateTimeOffset whenDataWillNotBeFresh = new DateTimeOffset(2020, 05, 17, 21, 00, 00, 00, new TimeSpan(0, 0, 0));

            _pollySyncCacheProvider.Verify(x => x.Put(It.IsAny<string>(), It.Is<CachedItemWrapper<string>>(y => y.Content == "dataFromBackendGet1" && y.IsFreshUntil == whenDataWillNotBeFresh), It.Is<Ttl>(y => y.Timespan == _defaultCacheDuration)), Times.Once);

            _distributedCacheWrapper.Verify(x => x.PutAsync(It.IsAny<string>(), It.Is<CachedItemWrapper<string>>(y => y.Content == "dataFromBackendGet1" && y.IsFreshUntil == whenDataWillNotBeFresh), It.Is<Ttl>(y => y.Timespan == _defaultCacheDuration), It.IsAny<CancellationToken>(), It.IsAny<bool>()), Times.Once);

            _pollySyncCacheProvider.Verify(x => x.Put(It.IsAny<string>(), It.Is<CachedItemWrapper<string>>(y => y.Content == "dataFromBackendGet2" && y.IsFreshUntil == whenDataWillNotBeFresh), It.Is<Ttl>(y => y.Timespan == _defaultCacheDuration)), Times.Once);

            _distributedCacheWrapper.Verify(x => x.PutAsync(It.IsAny<string>(), It.Is<CachedItemWrapper<string>>(y => y.Content == "dataFromBackendGet2" && y.IsFreshUntil == whenDataWillNotBeFresh), It.Is<Ttl>(y => y.Timespan == _defaultCacheDuration), It.IsAny<CancellationToken>(), It.IsAny<bool>()), Times.Once);
        }
    }
}
