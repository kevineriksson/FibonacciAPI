using System.Collections.Concurrent;
using System.Diagnostics;
using System.Numerics;
using FibonacciAPI.Interfaces;
using FibonacciAPI.Models;

public class Fibonacci : IFibonacciService
{
    private readonly IFibonacciCache _cache;
    private List<long> MemoryRecord = new();
    private long MemoryLimit;
    private TimeSpan TimeoutLimit;

    public Fibonacci(IFibonacciCache cache)
    {
        _cache = cache;
    }

    public async Task<FibonacciResult> GenerateFibonacciSubsequenceAsync(FibonacciRequest request)
    {
        var result = new FibonacciResult();
        var startTime = Stopwatch.GetTimestamp();
        TimeoutLimit = TimeSpan.FromMilliseconds(request.TimeoutMs);
        MemoryLimit = request.MaxMemoryMB * 1_000_000;

        try
        {
            for (int i = request.StartIndex; i <= request.EndIndex; i++)
            {
                RecordCurrentTimeUsage(startTime);
                RecordCurrentMemoryUsage();

                long value;
                if (request.UseCache && _cache.TryGet(i, out value))
                {
                    result.Fibonacci.Add(value);
                    continue;
                }

                value = await ComputeFibonacciAsync(i);

                if (request.UseCache) _cache.Set(i, value);

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
        var sizeInBytes = currentProcess.WorkingSet64;

        MemoryRecord.Add(sizeInBytes);
        if (sizeInBytes > MemoryLimit)
        {
            throw new Exception($"Memory Limit of {MemoryLimit} bytes exceeded by {sizeInBytes - MemoryLimit} bytes");
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
        var memResult = "Memory Consumptions for Every Number Generated (bytes): " +
        string.Join(",", MemoryRecord);
        return memResult;
    }
}
