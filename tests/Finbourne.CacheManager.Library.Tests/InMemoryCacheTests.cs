namespace Finbourne.CacheManager.Library.Tests
{
    public class InMemoryCacheTests
    {
        [Fact]
        public void GIVEN_InvalidCapacity_WHEN_Instantiate__THEN_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new InMemoryCache(0));
            Assert.Throws<ArgumentException>(() => new InMemoryCache(-1));
        }

        [Fact]
        public void GIVEN_ValidKeyAndValue_WHEN_AddAndGet_THEN__ReturnsAddedValue()
        {
            // Arrange
            var sut = new InMemoryCache(2);
            string key = "Key1";
            string value = "Value1";

            // Act
            sut.Add(key, value);
            var responseValue = sut.Get(key);

            // Assert
            Assert.Equal(value, responseValue);
        }

        [Fact]
        public void GIVEN_ExistingKey_WHEN_Add_ThenThrowsInvalidOperationsException()
        {
            // Arrange
            var sut = new InMemoryCache(2);
            string key = "Key1";
            string originalValue = "Value1";
            string newValue = "NewValue";

            // Act
            sut.Add(key, originalValue);
            var responseValue = sut.Get(key);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => sut.Add(key, newValue));
        }

        [Fact]
        public void GIVEN_CacheIsFull_WHEN_AddNewItem_THEN_RemoveLeastRecentlyUsedItem()
        {
            // Arrange
            var sut = new InMemoryCache(2);
            string key1 = "Key1";
            string key2 = "Key2";
            string key3 = "Key3";
            string value1 = "Value1";
            string value2 = "Value2";
            string value3 = "Value3";

            // Act
            sut.Add(key1, value1);
            sut.Add(key2, value2);
            sut.Add(key3, value3);

            // Assert
            Assert.Throws<KeyNotFoundException>(() => sut.Get(key1));
            Assert.Equal(value2, sut.Get(key2));
            Assert.Equal(value3, sut.Get(key3));
        }

        [Fact]
        public void GIVEN_ExistingKey_WHEN_Remove_THEN_RemovesKeyAndValue()
        {
            // Arrange
            var sut = new InMemoryCache(2);
            string key = "Key1";
            string value = "Value1";

            // Act
            sut.Add(key, value);
            bool removed = sut.Remove(key);

            // Assert
            Assert.True(removed);
            Assert.Throws<KeyNotFoundException>(() => sut.Get(key));
        }

        [Fact]
        public void GIVEN_KeyDoesNotExists_WHEN_Remove_THEN_ReturnsFalse()
        {
            // Arrange
            var sut = new InMemoryCache(2);
            string key = "Key1";

            // Act
            bool removed = sut.Remove(key);

            // Assert
            Assert.False(removed);
        }

        [Fact]
        public void GIVEN_DifferentTypesAsValues_WHEN_Add_THEN_SuccessfullyStored()
        {
            // Arrange
            var cache = new InMemoryCache(5);
            Guid key1 = Guid.NewGuid();
            string value1 = "This is a string value.";
            Guid key2 = Guid.NewGuid();
            int value2 = 42;
            Guid key3 = Guid.NewGuid();
            double value3 = 3.14;

            // Act
            cache.Add(key1.ToString(), value1);
            cache.Add(key2.ToString(), value2);
            cache.Add(key3.ToString(), value3);

            // Assert
            Assert.Equal(value1, cache.Get(key1.ToString()));
            Assert.Equal(value2, cache.Get(key2.ToString()));
            Assert.Equal(value3, cache.Get(key3.ToString()));
        }

        [Fact]
        public void GIVEN_ComplexTypeAsValue_WHEN_Add_THEN_SuccessfullyStored()
        {
            // Arrange
            var cache = new InMemoryCache(5);
            Guid key = Guid.NewGuid();
            var complexObject = new ComplexType
            {
                Name = "John Doe",
                Age = 30,
                Address = "123 Main St",
            };

            // Act
            cache.Add(key.ToString(), complexObject);

            // Assert
            var retrievedObject = cache.Get(key.ToString()) as ComplexType;

            Assert.NotNull(retrievedObject);
            Assert.Equal(complexObject.Name, retrievedObject?.Name);
            Assert.Equal(complexObject.Age, retrievedObject?.Age);
            Assert.Equal(complexObject.Address, retrievedObject?.Address);
        }

        [Fact]
        public void GIVEN_ConcurrentAccess_WHEN_AddAndGet_THEN_ThreadSafe()
        {
            // Arrange
            var cache = new InMemoryCache(100);
            int expectedTotalItems = 100;
            int threadCount = 10;

            var tasks = new List<Task>();

            // Act
            for (int i = 0; i < threadCount; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    for (int j = 0; j < expectedTotalItems / threadCount; j++)
                    {
                        string key = Guid.NewGuid().ToString();
                        cache.Add(key, Guid.NewGuid().ToString());
                        cache.Get(key);
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());

            // Assert
            Assert.Equal(expectedTotalItems, cache.Count);
        }

        [Fact]
        public void WHEN_LeastRecentItemIsRemoved_EventIsRaised()
        {
            // Arrange
            var cache = new InMemoryCache(1);
            string key1 = "Key1";
            string key2 = "Key2";
            string value1 = "Value1";
            string value2 = "Value2";

            bool eventRaised = false;
            cache.ItemLeastRecentRemoved += (sender, e) =>
            {
                eventRaised = true;
            };

            // Act
            cache.Add(key1, value1);
            cache.Add(key2, value2);

            // Assert
            Assert.True(eventRaised);
        }


        private class ComplexType
        {
            public string? Name { get; set; }
            public int Age { get; set; }
            public string? Address { get; set; }
        }
    }
}
