using HelpMyStreet.Cache.MemDistCache;
using HelpMyStreet.Utils.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using NUnit.Framework;
using Polly.Caching;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Utf8Json.Resolvers;

namespace HelpMyStreet.UnitTests
{
    public class DistributedCacheWrapperWithCompressionTests
    {
        private Mock<IDistributedCache> _distributedCache;

        private Ttl _ttl;

        private TestClass _testClass = new TestClass()
        {
            Property = "prop"
        };

        private string _key = "key";

        [SetUp]
        public void SetUp()
        {
            _distributedCache = new Mock<IDistributedCache>();
            _distributedCache.SetupAllProperties();
            _ttl = new Ttl(TimeSpan.FromMinutes(5));
        }

        [Test]
        public async Task PutAsync()
        {
            DistributedCacheWrapperWithCompression distributedCacheWrapperWithCompression = new DistributedCacheWrapperWithCompression(_distributedCache.Object);

            await distributedCacheWrapperWithCompression.PutAsync(_key, _testClass, _ttl, CancellationToken.None, true);

            byte[] result = Utf8Json.JsonSerializer.Serialize(_testClass, StandardResolver.AllowPrivate);

            byte[] compressedBytes = CompressionUtils.Gzip(result);

            _distributedCache.Verify(x => x.SetAsync(It.Is<string>(y => y == _key), It.Is<byte[]>(y => y.SequenceEqual(compressedBytes)), It.Is<DistributedCacheEntryOptions>(y => y.AbsoluteExpirationRelativeToNow.Value == _ttl.Timespan), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task TryGetAsync_Compressed()
        {
            byte[] serialisedTestClass = Utf8Json.JsonSerializer.Serialize(_testClass, StandardResolver.AllowPrivate);

            byte[] compressedSerialisedTestClasss = CompressionUtils.Gzip(serialisedTestClass);

            _distributedCache.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(compressedSerialisedTestClasss);

            DistributedCacheWrapperWithCompression distributedCacheWrapperWithCompression = new DistributedCacheWrapperWithCompression(_distributedCache.Object);

            (bool, object) result = await distributedCacheWrapperWithCompression.TryGetAsync<TestClass>(_key, CancellationToken.None, true);

            Assert.IsTrue(result.Item1);
            Assert.AreEqual(_testClass.Property, ((TestClass)result.Item2).Property);
        }


        [Test]
        public async Task TryGetAsync_Uncompressed()
        {
            byte[] serialisedTestClass = Utf8Json.JsonSerializer.Serialize(_testClass, StandardResolver.AllowPrivate);


            _distributedCache.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(serialisedTestClass);

            DistributedCacheWrapperWithCompression distributedCacheWrapperWithCompression = new DistributedCacheWrapperWithCompression(_distributedCache.Object);

            (bool, object) result = await distributedCacheWrapperWithCompression.TryGetAsync<TestClass>(_key, CancellationToken.None, true);

            Assert.IsTrue(result.Item1);
            Assert.AreEqual(_testClass.Property, ((TestClass)result.Item2).Property);
        }

        [Test]
        public async Task TryGetAsync_NoData()
        {
            _distributedCache.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(default(byte[]));

            DistributedCacheWrapperWithCompression distributedCacheWrapperWithCompression = new DistributedCacheWrapperWithCompression(_distributedCache.Object);

            (bool, object) result = await distributedCacheWrapperWithCompression.TryGetAsync<TestClass>(_key, CancellationToken.None, true);

            Assert.IsFalse(result.Item1);
            Assert.IsNull(result.Item2);
        }


        [Test]
        public void TryGetAsync_RetryTwiceOnRedisException()
        {
            RedisException redisException = (RedisException)FormatterServices.GetUninitializedObject(typeof(RedisException));

            _distributedCache.SetupSequence(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Throws(redisException)
                .Throws(redisException)
                .Throws(redisException)
                .Throws(redisException);

            DistributedCacheWrapperWithCompression distributedCacheWrapperWithCompression = new DistributedCacheWrapperWithCompression(_distributedCache.Object);


            RedisException ex = Assert.ThrowsAsync<RedisException>(async () =>
              {
                  await distributedCacheWrapperWithCompression.TryGetAsync<TestClass>(_key, CancellationToken.None, true);
              });


            _distributedCache.Verify(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
        }

        [Test]
        public void PutAsync_RetryTwiceOnRedisException()
        {
            RedisException redisException = (RedisException)FormatterServices.GetUninitializedObject(typeof(RedisException));

            _distributedCache.SetupSequence(x=>x.SetAsync(It.Is<string>(y => y == _key), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
                .Throws(redisException)
                .Throws(redisException)
                .Throws(redisException)
                .Throws(redisException);

            DistributedCacheWrapperWithCompression distributedCacheWrapperWithCompression = new DistributedCacheWrapperWithCompression(_distributedCache.Object);
            
            RedisException ex = Assert.ThrowsAsync<RedisException>(async () =>
            {
                await distributedCacheWrapperWithCompression.PutAsync(_key, _testClass, _ttl, CancellationToken.None, true);
            });
            
            _distributedCache.Verify(x => x.SetAsync(It.Is<string>(y => y == _key), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()),Times.Exactly(3));
        }
    }
}
