# InMemoryCache 

## Implementation:

- Developed as a .Net 6 Class Library
- Implements the ICache interface, providing methods for adding, retrieving, and removing items from the cache.
- Includes events ItemLeastRecentRemoved, ItemRemoved, and ItemAdded to notify consumers of cache-related events.
- Enforces capacity limits and handles evictions using the least recently used (LRU) strategy.
- Uses locks for thread safety in methods that modify the cache.

## Unit Tests:

- Developed as a .Net 6 XUnit application
- Covers different scenarios, including valid and invalid cache capacity, adding and retrieving items, removing items, concurrent access, and more.
- Tests cover various types of values, including strings, integers, doubles, and complex custom objects.
- Tests the thread safety of the cache under concurrent access.

## Demo Application:

- Developed as a .Net 6 Console Application
- Demonstrates how to use the InMemoryCache class.
- Simulates a scenario where multiple threads add items to the cache and showcases the thread safety of the cache.
- Subscribes to cache events to log cache-related actions.
- Provides insights into cache capacity, items removed due to least recent eviction, intentionally removed items, and the final cache count.


## License

[MIT](https://choosealicense.com/licenses/mit/)
