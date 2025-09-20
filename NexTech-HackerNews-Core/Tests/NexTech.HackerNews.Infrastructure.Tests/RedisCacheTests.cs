using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Nextech.HackerNews.Infrastructure.Caching;
using System.Text;
using System.Text.Json;

/// <summary>
/// Defines the <see cref="RedisCacheTests" />
/// </summary>
public class RedisCacheTests
{
    /// <summary>
    /// Defines the _cacheMock
    /// </summary>
    private readonly Mock<IDistributedCache> _cacheMock;

    /// <summary>
    /// Defines the _redisCache
    /// </summary>
    private readonly RedisCache _redisCache;

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisCacheTests"/> class.
    /// </summary>
    public RedisCacheTests()
    {
        _cacheMock = new Mock<IDistributedCache>();
        _redisCache = new RedisCache(_cacheMock.Object);
    }

    /// <summary>
    /// The GetAsync_ReturnsDeserializedObject_WhenCacheHasValue
    /// </summary>
    /// <returns>The <see cref="Task"/></returns>
    [Fact]
    public async Task GetAsync_ReturnsDeserializedObject_WhenCacheHasValue()
    {
        // Arrange
        var key = "test-key";
        var expected = new TestObject { Id = 1, Name = "Test" };
        var serialized = JsonSerializer.Serialize(expected);
        var bytes = Encoding.UTF8.GetBytes(serialized);

        // Mock GetAsync instead of GetStringAsync
        _cacheMock.Setup(x => x.GetAsync(key, It.IsAny<CancellationToken>()))
                  .ReturnsAsync(bytes);

        // Act
        var result = await _redisCache.GetAsync<TestObject>(key);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expected.Id, result!.Id);
        Assert.Equal(expected.Name, result.Name);
    }

    /// <summary>
    /// The GetAsync_ReturnsNull_WhenCacheIsEmpty
    /// </summary>
    /// <returns>The <see cref="Task"/></returns>
    [Fact]
    public async Task GetAsync_ReturnsNull_WhenCacheIsEmpty()
    {
        var key = "missing-key";

        _cacheMock.Setup(x => x.GetAsync(key, It.IsAny<CancellationToken>()))
                  .ReturnsAsync((byte[]?)null);

        var result = await _redisCache.GetAsync<TestObject>(key);

        Assert.Null(result);
    }

    /// <summary>
    /// The SetAsync_SerializesAndStoresObject
    /// </summary>
    /// <returns>The <see cref="Task"/></returns>
    [Fact]
    public async Task SetAsync_SerializesAndStoresObject()
    {
        var key = "set-key";
        var value = new TestObject { Id = 2, Name = "SetTest" };
        DistributedCacheEntryOptions? capturedOptions = null;
        byte[]? capturedBytes = null;

        _cacheMock.Setup(x => x.SetAsync(
            key,
            It.IsAny<byte[]>(),
            It.IsAny<DistributedCacheEntryOptions>(),
            It.IsAny<CancellationToken>()))
        .Callback<string, byte[], DistributedCacheEntryOptions, CancellationToken>((k, v, o, t) =>
        {
            capturedBytes = v;
            capturedOptions = o;
        })
        .Returns(Task.CompletedTask);

        await _redisCache.SetAsync(key, value, TimeSpan.FromMinutes(5));

        var expectedBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value));
        Assert.Equal(expectedBytes, capturedBytes);
        Assert.NotNull(capturedOptions);
        Assert.Equal(TimeSpan.FromMinutes(5), capturedOptions!.AbsoluteExpirationRelativeToNow);
    }

    /// <summary>
    /// Defines the <see cref="TestObject" />
    /// </summary>
    private class TestObject
    {
        /// <summary>
        /// Gets or sets the Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }
}
