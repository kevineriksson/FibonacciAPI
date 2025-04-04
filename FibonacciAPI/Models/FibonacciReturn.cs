namespace FibonacciAPI.Models;

public class FibonacciReturn
{
    public IEnumerable<long> FibonacciSequence { get; set; }  = new List<long>();
    public TimeSpan ElapsedTime { get; set; }
    public long MemoryUsedKb { get; set; }
}