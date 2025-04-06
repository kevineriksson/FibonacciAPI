namespace FibonacciAPI.Models;

public class FibonacciResult
{
    public List<long> Fibonacci { get; set; } = new();
    public bool IsPartial { get; set; }
    public bool TimeoutOccurred { get; set; }
    public bool MemoryLimitReached { get; set; }
    public string MemoryLog { get; set; }
}