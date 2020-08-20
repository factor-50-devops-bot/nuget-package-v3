using HelpMyStreet.Cache;
using HelpMyStreet.Cache.MemCache;
using HelpMyStreet.Cache.Models;
using HelpMyStreet.Utils.Utils;
using Microsoft.Extensions.Internal;
using Moq;
using NUnit.Framework;
using Polly.Caching;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace HelpMyStreet.UnitTests
{
    public class MemCacheTests
    {
        private Mock<ISyncCacheProvider> _pollySyncCacheProvider;
        private Mock<ISystemClock> _mockableDateTime;

        private int _numberOfTimesDataGetterDelegate1Called;
        private Func<CancellationToken, Task<string>> _dataGetterDelegate1;


        private Mock<ILoggerWrapper<MemCache<string>>> _logger;

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

            _logger = new Mock<ILoggerWrapper<MemCache<string>>>();
            _logger.SetupAllProperties();
        }

        [Test]
        public async Task DataNotInMemoryCache()
        {
            _pollySyncCacheProvider.Setup(x => x.TryGet(It.IsAny<string>())).Returns((false, null));

            _mockableDateTime.Setup(x => x.UtcNow).Returns(new DateTimeOffset(2020, 05, 17, 20, 00, 00, 00, new TimeSpan(0, 0, 0)));

            MemCacheFactory<string> memCacheFactory = new MemCacheFactory<string>(_pollySyncCacheProvider.Object, _mockableDateTime.Object, _logger.Object);
            IMemDistCache<string> memCache = memCacheFactory.GetCache(_defaultCacheDuration, ResetTimeFactory.OnHour);

            string result = await memCache.GetCachedDataAsync(_dataGetterDelegate1, _key, RefreshBehaviour.DontWaitForFreshData, CancellationToken.None);

            Assert.AreEqual("dataFromBackendGet", result);

            _pollySyncCacheProvider.Verify(x => x.TryGet(It.Is<string>(y => y == _key)), Times.Once);
            
            Assert.AreEqual(1, _numberOfTimesDataGetterDelegate1Called);

            DateTimeOffset whenDataWillNotBeFresh = new DateTimeOffset(2020, 05, 17, 21, 00, 00, 00, new TimeSpan(0, 0, 0));

            _pollySyncCacheProvider.Verify(x => x.Put(It.Is<string>(y => y == _key), It.Is<CachedItemWrapper<string>>(y => y.Content == "dataFromBackendGet" && y.IsFreshUntil == whenDataWillNotBeFresh), It.Is<Ttl>(y => y.Timespan == _defaultCacheDuration)), Times.Once);
        }

        [Test]
        public async Task DataInMemoryCacheAndFresh()
        {
            CachedItemWrapper<string> memoryCacheItem = new CachedItemWrapper<string>("fromMemCache", new DateTimeOffset(2020, 05, 17, 20, 00, 00, 00, new TimeSpan(0, 0, 0)));

            _pollySyncCacheProvider.Setup(x => x.TryGet(It.IsAny<string>())).Returns((true, memoryCacheItem));

            _mockableDateTime.Setup(x => x.UtcNow).Returns(new DateTimeOffset(2020, 05, 17, 20, 00, 00, 00, new TimeSpan(0, 0, 0)));

            MemCacheFactory<string> memCacheFactory = new MemCacheFactory<string>(_pollySyncCacheProvider.Object, _mockableDateTime.Object, _logger.Object);
            IMemDistCache<string> memCache = memCacheFactory.GetCache(_defaultCacheDuration, ResetTimeFactory.OnHour);

            string result = await memCache.GetCachedDataAsync(_dataGetterDelegate1, _key, RefreshBehaviour.DontWaitForFreshData, CancellationToken.None);

            Assert.AreEqual("fromMemCache", result);
            Assert.AreEqual(0, _numberOfTimesDataGetterDelegate1Called);

            _pollySyncCacheProvider.Verify(x => x.TryGet(It.Is<string>(y => y == _key)), Times.Once);
            _pollySyncCacheProvider.Verify(x => x.Put(It.IsAny<string>(), It.IsAny<CachedItemWrapper<string>>(), It.IsAny<Ttl>()), Times.Never);

        }

        [Test]
        public async Task DataInMemoryCacheButNotFresh()
        {
            CachedItemWrapper<string> memoryCacheItem = new CachedItemWrapper<string>("fromMemCache", new DateTimeOffset(2020, 05, 17, 19, 59, 59, 00, new TimeSpan(0, 0, 0)));

            _pollySyncCacheProvider.Setup(x => x.TryGet(It.IsAny<string>())).Returns((true, memoryCacheItem));

            _mockableDateTime.Setup(x => x.UtcNow).Returns(new DateTimeOffset(2020, 05, 17, 20, 00, 00, 00, new TimeSpan(0, 0, 0)));

            MemCacheFactory<string> memCacheFactory = new MemCacheFactory<string>(_pollySyncCacheProvider.Object, _mockableDateTime.Object, _logger.Object);
            IMemDistCache<string> memCache = memCacheFactory.GetCache(_defaultCacheDuration, ResetTimeFactory.OnHour);

            string result = await memCache.GetCachedDataAsync(_dataGetterDelegate1, _key, RefreshBehaviour.DontWaitForFreshData, CancellationToken.None);

            Assert.AreEqual("fromMemCache", result);

            _pollySyncCacheProvider.Verify(x => x.TryGet(It.Is<string>(y => y == _key)), Times.Once);


            await Task.Delay(_waitForBackgroundThreadToCompleteMs); // wait for background thread

            Assert.AreEqual(1, _numberOfTimesDataGetterDelegate1Called);

            DateTimeOffset whenDataWillNotBeFresh = new DateTimeOffset(2020, 05, 17, 21, 00, 00, 00, new TimeSpan(0, 0, 0));

            _pollySyncCacheProvider.Verify(x => x.Put(It.Is<string>(y => y == _key), It.Is<CachedItemWrapper<string>>(y => y.Content == "dataFromBackendGet" && y.IsFreshUntil == whenDataWillNotBeFresh), It.Is<Ttl>(y => y.Timespan == _defaultCacheDuration)), Times.Once);
        }

        [Test]
        public async Task DataInMemoryCacheButNotFresh_WaitForFreshData()
        {
            CachedItemWrapper<string> memoryCacheItem = new CachedItemWrapper<string>("fromMemCache", new DateTimeOffset(2020, 05, 17, 19, 59, 59, 00, new TimeSpan(0, 0, 0)));

            _pollySyncCacheProvider.Setup(x => x.TryGet(It.IsAny<string>())).Returns((true, memoryCacheItem));

            _mockableDateTime.Setup(x => x.UtcNow).Returns(new DateTimeOffset(2020, 05, 17, 20, 00, 00, 00, new TimeSpan(0, 0, 0)));

            MemCacheFactory<string> memCacheFactory = new MemCacheFactory<string>(_pollySyncCacheProvider.Object, _mockableDateTime.Object, _logger.Object);
            IMemDistCache<string> memCache = memCacheFactory.GetCache(_defaultCacheDuration, ResetTimeFactory.OnHour);

            string result = await memCache.GetCachedDataAsync(_dataGetterDelegate1, _key, RefreshBehaviour.WaitForFreshData, CancellationToken.None);

            Assert.AreEqual("dataFromBackendGet", result);

            _pollySyncCacheProvider.Verify(x => x.TryGet(It.Is<string>(y => y == _key)), Times.Once);


            Assert.AreEqual(1, _numberOfTimesDataGetterDelegate1Called);

            DateTimeOffset whenDataWillNotBeFresh = new DateTimeOffset(2020, 05, 17, 21, 00, 00, 00, new TimeSpan(0, 0, 0));

            _pollySyncCacheProvider.Verify(x => x.Put(It.Is<string>(y => y == _key), It.Is<CachedItemWrapper<string>>(y => y.Content == "dataFromBackendGet" && y.IsFreshUntil == whenDataWillNotBeFresh), It.Is<Ttl>(y => y.Timespan == _defaultCacheDuration)), Times.Once);

        }

        [Test]
        public async Task DataInMemoryCacheButNotFresh_DontRefreshData()
        {
            CachedItemWrapper<string> memoryCacheItem = new CachedItemWrapper<string>("fromMemCache", new DateTimeOffset(2020, 05, 17, 19, 59, 59, 00, new TimeSpan(0, 0, 0)));

            _pollySyncCacheProvider.Setup(x => x.TryGet(It.IsAny<string>())).Returns((true, memoryCacheItem));

            _mockableDateTime.Setup(x => x.UtcNow).Returns(new DateTimeOffset(2020, 05, 17, 20, 00, 00, 00, new TimeSpan(0, 0, 0)));

            MemCacheFactory<string> memCacheFactory = new MemCacheFactory<string>(_pollySyncCacheProvider.Object, _mockableDateTime.Object, _logger.Object);
            IMemDistCache<string> memCache = memCacheFactory.GetCache(_defaultCacheDuration, ResetTimeFactory.OnHour);

            string result = await memCache.GetCachedDataAsync(_dataGetterDelegate1, _key, RefreshBehaviour.DontRefreshData, CancellationToken.None);

            Assert.AreEqual("fromMemCache", result);

            _pollySyncCacheProvider.Verify(x => x.TryGet(It.Is<string>(y => y == _key)), Times.Once);


            await Task.Delay(_waitForBackgroundThreadToCompleteMs); // wait for background thread

            Assert.AreEqual(0, _numberOfTimesDataGetterDelegate1Called);


            _pollySyncCacheProvider.Verify(x => x.Put(It.IsAny<string>(), It.IsAny<CachedItemWrapper<string>>(), It.Is<Ttl>(y => y.Timespan == _defaultCacheDuration)), Times.Never);
        }

        [Test]
        public async Task DataInMemoryCacheButNotFresh_BackendGetErrors()
        {
            CachedItemWrapper<string> memoryCacheItem = new CachedItemWrapper<string>("fromMemCache", new DateTimeOffset(2020, 05, 17, 19, 59, 59, 00, new TimeSpan(0, 0, 0)));

            _pollySyncCacheProvider.Setup(x => x.TryGet(It.IsAny<string>())).Returns((true, memoryCacheItem));

            _mockableDateTime.Setup(x => x.UtcNow).Returns(new DateTimeOffset(2020, 05, 17, 20, 00, 00, 00, new TimeSpan(0, 0, 0)));

            Func<CancellationToken, Task<string>> _dataGetterDelegate1 = (token) =>
           {
               _numberOfTimesDataGetterDelegate1Called++;
               throw new Exception("An error");
           };

            MemCacheFactory<string> memCacheFactory = new MemCacheFactory<string>(_pollySyncCacheProvider.Object, _mockableDateTime.Object, _logger.Object);
            IMemDistCache<string> memCache = memCacheFactory.GetCache(_defaultCacheDuration, ResetTimeFactory.OnHour);

            string result = await memCache.GetCachedDataAsync(_dataGetterDelegate1, _key, RefreshBehaviour.DontWaitForFreshData, CancellationToken.None);

            Assert.AreEqual("fromMemCache", result);

            _pollySyncCacheProvider.Verify(x => x.TryGet(It.Is<string>(y => y == _key)), Times.Once);


            await Task.Delay(_waitForBackgroundThreadToCompleteMs); // wait for background thread

            _logger.Verify(x => x.LogError(It.Is<string>(y => y == $"Error executing data getter for key: {_key}"), It.IsAny<Exception>()));

            Assert.AreEqual(1, _numberOfTimesDataGetterDelegate1Called);

            DateTimeOffset whenDataWillNotBeFresh = new DateTimeOffset(2020, 05, 17, 21, 00, 00, 00, new TimeSpan(0, 0, 0));

            _pollySyncCacheProvider.Verify(x => x.Put(It.IsAny<string>(), It.Is<CachedItemWrapper<string>>(y => y.Content == "dataFromBackendGet" && y.IsFreshUntil == whenDataWillNotBeFresh), It.Is<Ttl>(y => y.Timespan == _defaultCacheDuration)), Times.Never);

        }

        [Test]
        public async Task RefreshData()
        {
            _mockableDateTime.Setup(x => x.UtcNow).Returns(new DateTimeOffset(2020, 05, 17, 20, 00, 00, 00, new TimeSpan(0, 0, 0)));
            
            MemCacheFactory<string> memCacheFactory = new MemCacheFactory<string>(_pollySyncCacheProvider.Object, _mockableDateTime.Object, _logger.Object);
            IMemDistCache<string> memCache = memCacheFactory.GetCache(_defaultCacheDuration, ResetTimeFactory.OnHour);

            string result = await memCache.RefreshDataAsync(_dataGetterDelegate1, _key, CancellationToken.None);

            Assert.AreEqual("dataFromBackendGet", result);
            
            Assert.AreEqual(1, _numberOfTimesDataGetterDelegate1Called);

            DateTimeOffset whenDataWillNotBeFresh = new DateTimeOffset(2020, 05, 17, 21, 00, 00, 00, new TimeSpan(0, 0, 0));

            _pollySyncCacheProvider.Verify(x => x.Put(It.Is<string>(y => y == _key), It.Is<CachedItemWrapper<string>>(y => y.Content == "dataFromBackendGet" && y.IsFreshUntil == whenDataWillNotBeFresh), It.Is<Ttl>(y => y.Timespan == _defaultCacheDuration)), Times.Once);

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

            MemCacheFactory<string> memCacheFactory = new MemCacheFactory<string>(_pollySyncCacheProvider.Object, _mockableDateTime.Object, _logger.Object);
            IMemDistCache<string> memCache = memCacheFactory.GetCache(_defaultCacheDuration, ResetTimeFactory.OnHour);

            ConcurrentBag<Task<string>> results1 = new ConcurrentBag<Task<string>>();
            ConcurrentBag<Task<string>> results2 = new ConcurrentBag<Task<string>>();

            Stopwatch stopwatch = Stopwatch.StartNew();

            Task task1 = Task.Factory.StartNew(() =>
            {
                Parallel.For(0, 50, i =>
                {
                    Task<string> result = memCache.GetCachedDataAsync(dataGetterDelegate1, "key1", RefreshBehaviour.WaitForFreshData, CancellationToken.None);
                    results1.Add(result);
                });
            });


            Task task2 = Task.Factory.StartNew(() =>
            {
                Parallel.For(0, 50, i =>
                {
                    Task<string> result = memCache.GetCachedDataAsync(dataGetterDelegate2, "key2", RefreshBehaviour.WaitForFreshData, CancellationToken.None);
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


            _pollySyncCacheProvider.Verify(x => x.Put(It.IsAny<string>(), It.Is<CachedItemWrapper<string>>(y => y.Content == "dataFromBackendGet2" && y.IsFreshUntil == whenDataWillNotBeFresh), It.Is<Ttl>(y => y.Timespan == _defaultCacheDuration)), Times.Once);

        }
    }
}
