using FibonacciAPI.Models;

public interface IFibonacciService
{
    Task<FibonacciResult> GenerateFibonacciSubsequenceAsync(int startIndex, int endIndex, bool useCache, int timeoutMs, long maxMemoryBytes);
}