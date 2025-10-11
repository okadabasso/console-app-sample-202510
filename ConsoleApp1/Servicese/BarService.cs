using ConsoleApp1.DependencyInjection.Attributes;
using Microsoft.Extensions.Logging;

namespace ConsoleApp1.Services;

[Service]
public class BarService : IBarService
{
    public ILogger<BarService> _logger = null!;

    public BarService(ILogger<BarService> logger)
    {
        _logger = logger;
    }

    [Trace]
    public virtual void DoSomething()
    {
        _logger.LogInformation($"{nameof(BarService)} - {nameof(DoSomething)} called.");
        // Simulate some work
        _logger.LogInformation($"{nameof(BarService)} - {nameof(DoSomething)} complete.");
    }
}