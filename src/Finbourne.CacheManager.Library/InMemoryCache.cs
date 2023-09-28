using Finbourne.CacheManager.Library.Contracts;

namespace Finbourne.CacheManager.Library
{
    public class InMemoryCache : ICache
    {
        public event EventHandler<CacheItemEventArgs>? ItemLeastRecentRemoved;
        public event EventHandler<CacheItemEventArgs>? ItemRemoved;
        public event EventHandler<CacheItemEventArgs>? ItemAdded;

        private readonly Dictionary<string, object> _cache;
        private readonly LinkedList<string> _leastRecentlyUsedQueue;
        private readonly int _capacity;

        public InMemoryCache(int capacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentException("Cache capacity must be greater than 0.");
            }

            _capacity = capacity;
            _cache = new Dictionary<string, object>(capacity);
            _leastRecentlyUsedQueue = new LinkedList<string>();
        }

        public int Count => _cache.Count;

        public void Add(string key, object value)
        {
            lock (_cache)
            {
                if (_cache.ContainsKey(key))
                    throw new InvalidOperationException($"Key '{key}' already exists in the cache.");

                if (_cache.Count >= _capacity)
                {
                    RemoveOldestItemFromQueue();
                }

                ItemAdded?.Invoke(this, new CacheItemEventArgs(key));
                _cache.Add(key, value);
                UpdateLeastRecentlyUsedQueue(key);
            }
        }

        public object Get(string key)
        {
            lock (_cache)
            {
                if (_cache.TryGetValue(key, out var value))
                {
                    UpdateLeastRecentlyUsedQueue(key);
                    return value;
                }

                throw new KeyNotFoundException($"Key '{key}' not found in the cache.");
            }
        }

        public bool Remove(string key)
        {
            lock (_cache)
            {
                if (_cache.Remove(key))
                {
                    ItemRemoved?.Invoke(this, new CacheItemEventArgs(key));
                    _leastRecentlyUsedQueue.Remove(key);
                    return true;
                }

                return false;
            }
        }

        private void RemoveOldestItemFromQueue()
        {
            if (_leastRecentlyUsedQueue.Count > 0)
            {
                var key = _leastRecentlyUsedQueue.Last?.Value;

                if (key != null)
                {
                    var value = _cache[key];
                    ItemLeastRecentRemoved?.Invoke(this, new CacheItemEventArgs(key));
                    _cache.Remove(key);
                    _leastRecentlyUsedQueue.RemoveLast();
                }
            }
        }

        private void UpdateLeastRecentlyUsedQueue(string key)
        {
            lock (_cache)
            {
                if (_leastRecentlyUsedQueue.Contains(key))
                {
                    _leastRecentlyUsedQueue.Remove(key);
                }

                _leastRecentlyUsedQueue.AddFirst(key);
            }
        }
    }
}
