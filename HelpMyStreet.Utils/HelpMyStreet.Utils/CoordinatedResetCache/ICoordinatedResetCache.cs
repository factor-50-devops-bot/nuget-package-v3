using System;
using System.Threading;
using System.Threading.Tasks;

namespace HelpMyStreet.Utils.CoordinatedResetCache
{
    public interface ICoordinatedResetCache
    {
        /// <summary>
        ///  Get data from cache. Cache expires on the hour or minute so all servers are kept in sync. IPollyMemoryCacheProvider and ISystemClock must be registered in DI (can use PollyMemoryCacheProvider and MockableDateTime implementations).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataGetter">Delegate that return data</param>
        /// <param name="key">The key to store data under</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="resetCacheTime">When cache should reset</param>
        /// <returns></returns>
        Task<T> GetCachedDataAsync<T>(Func<CancellationToken, Task<T>> dataGetter, string key, CancellationToken cancellationToken, CoordinatedResetCacheTime resetCacheTime = CoordinatedResetCacheTime.OnHour);

        /// <summary>
        ///  Get data from cache. Cache expires on the hour or minute so all servers are kept in sync. IPollyMemoryCacheProvider and ISystemClock must be registered in DI (can use PollyMemoryCacheProvider and MockableDateTime implementations).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataGetter">Delegate that return data</param>
        /// <param name="key">The key to store data under</param>
        /// <param name="resetCacheTime">When cache should reset</param>
        /// <returns></returns>
        Task<T> GetCachedDataAsync<T>(Func<Task<T>> dataGetter, string key, CoordinatedResetCacheTime resetCacheTime);

        /// <summary>
        ///  Get data from cache. Cache expires on the hour or minute so all servers are kept in sync. IPollyMemoryCacheProvider and ISystemClock must be registered in DI (can use PollyMemoryCacheProvider and MockableDateTime implementations).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataGetter">Delegate that return data</param>
        /// <param name="key">The key to store data under</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="resetCacheTime">When cache should reset</param>
        /// <returns></returns>
        T GetCachedData<T>(Func<CancellationToken, T> dataGetter, string key, CancellationToken cancellationToken, CoordinatedResetCacheTime resetCacheTime = CoordinatedResetCacheTime.OnHour);

        /// <summary>
        ///  Get data from cache. Cache expires on the hour or minute so all servers are kept in sync. IPollyMemoryCacheProvider and ISystemClock must be registered in DI (can use PollyMemoryCacheProvider and MockableDateTime implementations).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataGetter">Delegate that return data</param>
        /// <param name="key">The key to store data under</param>
        /// <param name="resetCacheTime">When cache should reset</param>
        /// <returns></returns>
        T GetCachedData<T>(Func<T> dataGetter, string key, CoordinatedResetCacheTime resetCacheTime);
    }
}