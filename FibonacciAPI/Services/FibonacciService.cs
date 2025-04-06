using System.Collections.Concurrent;
using System.Diagnostics;
using System.Numerics;
using FibonacciAPI.Interfaces;
using FibonacciAPI.Models;

public class FibonacciService : IFibonacciService
{
    private readonly IFibonacciCache _cache;
    private List<long> MemoryRecord = new();
    private long MemoryLimit;
    private TimeSpan TimeoutLimit;

    public FibonacciService(IFibonacciCache cache)
    {
        _cache = cache;
    }

    public async Task<FibonacciResult> GenerateFibonacciSubsequenceAsync(int startIndex, int endIndex, bool useCache, int timeoutMs, long maxMemoryBytes)
    {
        var result = new FibonacciResult();
        var startTime = Stopwatch.GetTimestamp();
        TimeoutLimit = TimeSpan.FromMilliseconds(timeoutMs);
        MemoryLimit = maxMemoryBytes / 1024; // Store memory limit in KB for comparison

        try
        {
            for (int i = startIndex; i <= endIndex; i++)
            {
                RecordCurrentTimeUsage(startTime);
                RecordCurrentMemoryUsage();

                long value;
                if (useCache && _cache.TryGet(i, out value))
                {
                    result.Fibonacci.Add(value);
                    continue;
                }

                value = await ComputeFibonacciAsync(i);

                if (useCache) _cache.Set(i, value);

                result.Fibonacci.Add(value);
            }
        }
        catch (TimeoutException)
        {
            result.IsPartial = true;
            result.TimeoutOccurred = true;
        }
        catch (Exception ex) when (ex.Message.Contains("Memory Limit"))
        {
            result.IsPartial = true;
            result.MemoryLimitReached = true;
        }

        result.MemoryLog = GetTotalProcessMemory();
        return result;
    }

    private async Task<long> ComputeFibonacciAsync(int n)
    {
        if (n == 0) return 0;
        if (n == 1) return 1;

        long a = 0, b = 1;
        for (int i = 2; i <= n; i++)
        {
            long temp = a + b;
            a = b;
            b = temp;
            await Task.Delay(500);
        }

        return b;
    }

    private TimeSpan GetCurrentElapsedTime(long startTime)
    {
        var currTime = Stopwatch.GetTimestamp();
        return Stopwatch.GetElapsedTime(startTime, currTime);
    }

    private void RecordCurrentMemoryUsage()
    {
        Process currentProcess = Process.GetCurrentProcess();
        var sizeInKB = currentProcess.PeakWorkingSet64 / 1024;
        MemoryRecord.Add(sizeInKB);
        if (sizeInKB > MemoryLimit)
        {
            throw new Exception($"Memory Limit of {MemoryLimit} Exceeded by {sizeInKB - MemoryLimit} KB");
        }
    }

    private void RecordCurrentTimeUsage(Int64 startTime)
    {
        if (GetCurrentElapsedTime(startTime) >= TimeoutLimit)
        {
            throw new TimeoutException($"Time Limit of {TimeoutLimit.TotalMilliseconds} ms exceeded.");
        }
    }

    public string GetTotalProcessMemory()
    {
        var memResult = "Memory Consumptions for Every Number Generated (KB): " +
        string.Join(",", MemoryRecord);
        return memResult;
    }
}
