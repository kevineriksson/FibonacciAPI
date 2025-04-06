namespace FibonacciAPI.Models;

public class FibonacciRequest
{
    public int StartIndex { get; set; }
    public int EndIndex { get; set; }
    public bool UseCache { get; set; }
    public int TimeoutMs { get; set; }
    public long MaxMemoryBytes { get; set; }
}