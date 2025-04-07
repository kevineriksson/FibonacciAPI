using FibonacciAPI.Models;

namespace FibonacciAPI.Interfaces;

public interface IFibonacciService
{
    Task<FibonacciResult> GenerateFibonacciSubsequenceAsync(FibonacciRequest request);
}