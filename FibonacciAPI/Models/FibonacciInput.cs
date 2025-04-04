namespace FibonacciAPI.Models;

public class FibonacciInput
{
    public int StartIndex { get; set; }
    public int EndIndex { get; set; }
    public bool UseCache { get; set; }
    public int TimeOut { get; set; }
    public long UseMemory { get; set; }
}