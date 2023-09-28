namespace Finbourne.CacheManager.Library.Contracts
{
    public interface ICache
    {
        void Add(string key, object value);
        object Get(string key);
        bool Remove(string key);
        int Count { get; }
    }
}
