using Polly.Caching.Memory;

namespace HelpMyStreet.Utils.CoordinatedResetCache
{
    public interface IPollyMemoryCacheProvider
    {
        MemoryCacheProvider MemoryCacheProvider { get; }
    }
}