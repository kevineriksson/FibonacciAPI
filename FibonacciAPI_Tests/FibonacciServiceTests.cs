using FibonacciAPI.Interfaces;
using FibonacciAPI.Models;
using Moq;
using Xunit;
using Assert = NUnit.Framework.Assert;

namespace FibonacciAPI_Tests;

public class FibonacciServiceTests
{
    private readonly Mock<IFibonacciCache> _mockCache;
    private readonly FibonacciService _service;

    public FibonacciServiceTests()
    {
        _mockCache = new Mock<IFibonacciCache>();
        _service = new FibonacciService(_mockCache.Object);
    }

    public static IEnumerable<object[]> FibonacciTestData =>
        new List<object[]>
        {
            new object[] { 0, 5, new List<long> { 0, 1, 1, 2, 3, 5 }, false },
            new object[] { 0, 3, new List<long> { 0, 1, 1, 2 }, true },
            new object[] { 2, 4, new List<long> { 1, 2, 3 }, true },
        };

    [Xunit.Theory]
    [MemberData(nameof(FibonacciTestData))]
    public async Task GenerateFibonacciSubsequenceAsync_ShouldReturnExpectedValues(
        int start, int end, List<long> expected, bool useCache)
    {
        var request = new FibonacciRequest
        {
            StartIndex = start,
            EndIndex = end,
            TimeoutMs = 10000,
            MaxMemoryMB = 100,
            UseCache = useCache
        };

        _mockCache.Setup(c => c.TryGet(It.IsAny<int>(), out It.Ref<long>.IsAny))
            .Returns(false); 

        var result = await _service.GenerateFibonacciSubsequenceAsync(request);

        Assert.That(result.Fibonacci, Is.EqualTo(expected));
        Assert.False(result.IsPartial);
        Assert.False(result.TimeoutOccurred);
        Assert.False(result.MemoryLimitReached);
    }

    [Fact]
    public async Task GenerateFibonacciSubsequenceAsync_ShouldRespectTimeout()
    {
        var request = new FibonacciRequest
        {
            StartIndex = 0,
            EndIndex = 5,
            TimeoutMs = 100,
            MaxMemoryMB = 100,
            UseCache = false
        };

        var result = await _service.GenerateFibonacciSubsequenceAsync(request);

        Assert.True(result.IsPartial);
        Assert.True(result.TimeoutOccurred);
    }

    [Fact]
    public async Task GenerateFibonacciSubsequenceAsync_ShouldRespectMemoryLimit()
    {
        var request = new FibonacciRequest
        {
            StartIndex = 0,
            EndIndex = 20, 
            TimeoutMs = 10000,
            MaxMemoryMB = 1, 
            UseCache = false
        };

        var result = await _service.GenerateFibonacciSubsequenceAsync(request);

        Assert.True(result.IsPartial);
        Assert.True(result.MemoryLimitReached);
    }
}