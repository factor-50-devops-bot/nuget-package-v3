//using HelpMyStreet.Cache;
//using Microsoft.Extensions.DependencyInjection;
//using NUnit.Framework;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using HelpMyStreet.Cache.Extensions;

//namespace HelpMyStreet.UnitTests
//{
//    public class RandomTests
//    {
//        [Test]
//        public async Task Test()
//        {

//            IServiceCollection services = new ServiceCollection();


//            services.AddMemDistCache("Test", "will.redis.cache.windows.net:6380,password=ActVt+M4M4qtelj7aOA6VdjCYI99KNHqsvDE8DWl5dg=,ssl=True,abortConnect=False");

//            //services.AddMemCache();

//            var serviceProvider = services.BuildServiceProvider();


//            IMemDistCacheFactory<TestClass> memDistCacheFactory = serviceProvider.GetService<IMemDistCacheFactory<TestClass>>();
//            IMemDistCache<TestClass> cache = memDistCacheFactory.GetCache(new TimeSpan(2, 0, 0, 0), TimeLengths.OnHour);


//            var testClass = new TestClass()
//            {
//                Property = "hello"
//            };

//            Func<CancellationToken, Task<TestClass>> dataGetter = token =>
//            {

//                var res = new TestClass()
//                {
//                    Property = "hello"
//                };

//                return Task.FromResult(res);
//            };

//            var result = await cache.GetCachedDataAsync(dataGetter, "key3", false, CancellationToken.None);
//            var result2 = await cache.GetCachedDataAsync(dataGetter, "key3", false, CancellationToken.None);
//        }

//    }

//}
