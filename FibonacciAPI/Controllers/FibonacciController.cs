using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class FibonacciController : ControllerBase
{
    private readonly IFibonacciService _fibonacciService;

    public FibonacciController(IFibonacciService fibonacciService)
    {
        _fibonacciService = fibonacciService;
    }

    [HttpGet]
    public async Task<IActionResult> GetFibonacciSubsequence(int startIndex, int endIndex, bool useCache, int timeoutMs, long maxMemoryBytes)
    {
        try
        {
            var result = await _fibonacciService.GenerateFibonacciSubsequenceAsync(startIndex, endIndex, useCache, timeoutMs, maxMemoryBytes);
            return Ok(result);
        }
        catch (TimeoutException)
        {
            return StatusCode(408, "Timeout occurred while generating Fibonacci numbers.");
        }
        catch (OutOfMemoryException)
        {
            return StatusCode(507, "Memory limit exceeded while generating Fibonacci numbers.");
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}