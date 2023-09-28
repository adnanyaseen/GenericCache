using Finbourne.CacheManager.Library;

namespace InMemoryCacheDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create an instance of the cache with a capacity of 6 items
            int totalCapacity = 10;
            var cache = new InMemoryCache(totalCapacity);
            int totalLeastRecentItemRemoved = 0, totalRemoved = 0, totalAdded = 0;

            // Subscribe to the ItemLeastRecentRemoved event
            cache.ItemLeastRecentRemoved += (sender, e) =>
            {
                totalLeastRecentItemRemoved++;
                Console.WriteLine($"Least recent item with key '{e.Key}' was evicted.");
                Console.WriteLine();
            };

            // Subscribe to the ItemRemoved event
            cache.ItemRemoved += (sender, e) =>
            {
                totalRemoved++;
                Console.WriteLine($"Item with key '{e.Key}' was removed from the cache.");
                Console.WriteLine();
            };

            // Subscribe to the ItemAdded event
            cache.ItemAdded += (sender, e) =>
            {
                totalAdded++;
                Console.WriteLine($"Item with key '{e.Key}' was added to the cache.");
                Console.WriteLine();
            };

            // Add items to the cache more then the cache's capacity asynchronously
            // This is to trigger the ItemRemoved event and check the thread safety
            int expectedTotalItems = 20;
            int threadCount = 5;
            var tasks = new List<Task>();

            for (int i = 0; i < threadCount; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    for (int j = 0; j < expectedTotalItems / threadCount; j++)
                    {
                        string key = Guid.NewGuid().ToString();
                        cache.Add(key, Guid.NewGuid());
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());

            string key = Guid.NewGuid().ToString();
            cache.Add(key, Guid.NewGuid());
            cache.Remove(key);

            Console.WriteLine($"Cache Capacity: {totalCapacity}");
            Console.WriteLine($"Total Items tried to be added: {totalAdded}");
            Console.WriteLine($"Total Least Recent Items Removed: {totalLeastRecentItemRemoved}");
            Console.WriteLine($"Total Items Intentionally Removed: {totalRemoved}");
            Console.WriteLine("Cache Count:" + cache.Count);
            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}