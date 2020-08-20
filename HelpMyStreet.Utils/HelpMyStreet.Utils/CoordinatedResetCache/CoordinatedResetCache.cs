using Microsoft.Extensions.Internal;
using Polly;
using Polly.Caching;
using Polly.Contrib.DuplicateRequestCollapser;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HelpMyStreet.Utils.CoordinatedResetCache
{
    public class CoordinatedResetCache : ICoordinatedResetCache
    {
        private readonly IPollyMemoryCacheProvider _pollyMemoryCacheProvider;
        private readonly ISystemClock _mockableDateTime;

        private static readonly IAsyncRequestCollapserPolicy _collapserPolicy = AsyncRequestCollapserPolicy.Create();


        [Obsolete("Use MemDistCache in HelpMyStreet.Cache package")]
        public CoordinatedResetCache(IPollyMemoryCacheProvider pollyMemoryCacheProvider, ISystemClock mockableDateTime)
        {
            _pollyMemoryCacheProvider = pollyMemoryCacheProvider;
            _mockableDateTime = mockableDateTime;
        }

        /// <inheritdoc />>
        [Obsolete("Use MemDistCache in HelpMyStreet.Cache package")]
        public async Task<T> GetCachedDataAsync<T>(Func<CancellationToken, Task<T>> dataGetter, string key, CancellationToken cancellationToken, CoordinatedResetCacheTime resetCacheTime = CoordinatedResetCacheTime.OnHour)
        {
            TimeSpan timeToReset = GetLengthOfTimeUntilNextHour();

            switch (resetCacheTime)
            {
                case CoordinatedResetCacheTime.OnHour:
                    timeToReset = GetLengthOfTimeUntilNextHour();
                    break;
                case CoordinatedResetCacheTime.OnMinute:
                    timeToReset = GetLengthOfTimeUntilNextMinute();
                    break;
            }

            AsyncCachePolicy cachePolicy = Policy.CacheAsync(_pollyMemoryCacheProvider.MemoryCacheProvider, timeToReset);

            Context context = new Context($"{nameof(CoordinatedResetCache)}_{key}");

            // collapser policy used to prevent concurrent calls retrieving the same data twice
            T result = await _collapserPolicy.WrapAsync(cachePolicy).ExecuteAsync(_ => dataGetter.Invoke(cancellationToken), context);

            return result;
        }

        // async without token
        /// <inheritdoc />>
        /// 
        [Obsolete("Use MemDistCache in HelpMyStreet.Cache package")]
        public async Task<T> GetCachedDataAsync<T>(Func<Task<T>> dataGetter, string key, CoordinatedResetCacheTime resetCacheTime = CoordinatedResetCacheTime.OnHour)
        {
            return await GetCachedDataAsync(token => dataGetter.Invoke(), key, CancellationToken.None, resetCacheTime);
        }

        // sync with token
        /// <inheritdoc />>
        /// 
        [Obsolete("Use MemDistCache in HelpMyStreet.Cache package")]
        public T GetCachedData<T>(Func<CancellationToken, T> dataGetter, string key, CancellationToken cancellationToken, CoordinatedResetCacheTime resetCacheTime = CoordinatedResetCacheTime.OnHour)
        {
            return GetCachedDataAsync(token => Task.FromResult(dataGetter.Invoke(token)), key, cancellationToken, resetCacheTime).Result;
        }

        // sync without token
        /// <inheritdoc />>
        /// 
        [Obsolete("Use MemDistCache in HelpMyStreet.Cache package")]
        public T GetCachedData<T>(Func<T> dataGetter, string key, CoordinatedResetCacheTime resetCacheTime = CoordinatedResetCacheTime.OnHour)
        {
            return GetCachedDataAsync(() => Task.FromResult(dataGetter.Invoke()), key, resetCacheTime).Result;
        }
        
        private TimeSpan GetLengthOfTimeUntilNextHour()
        {
            DateTimeOffset timeNow = _mockableDateTime.UtcNow;
            DateTimeOffset nowPlusOneMinute = timeNow.AddHours(1);
            DateTimeOffset theNextMinuteWithoutSeconds = new DateTime(nowPlusOneMinute.Year, nowPlusOneMinute.Month, nowPlusOneMinute.Day, nowPlusOneMinute.Hour, 0, 0, DateTimeKind.Utc);
            TimeSpan timeSpanUntilNextMinute = theNextMinuteWithoutSeconds - timeNow;
            return timeSpanUntilNextMinute;
        }

        private TimeSpan GetLengthOfTimeUntilNextMinute()
        {
            DateTimeOffset timeNow = _mockableDateTime.UtcNow;
            DateTimeOffset nowPlusOneMinute = timeNow.AddMinutes(1);
            DateTimeOffset theNextMinuteWithoutSeconds = new DateTime(nowPlusOneMinute.Year, nowPlusOneMinute.Month, nowPlusOneMinute.Day, nowPlusOneMinute.Hour, nowPlusOneMinute.Minute, 0, DateTimeKind.Utc);
            TimeSpan timeSpanUntilNextMinute = theNextMinuteWithoutSeconds - timeNow;
            return timeSpanUntilNextMinute;
        }
    }
}
