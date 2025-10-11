using ConsoleApp1.DependencyInjection.Attributes;
using Microsoft.Extensions.Logging;

namespace ConsoleApp1.Services;

[Service]
public class BarServiceDerived : IBarService
{
    public ILogger<BarServiceDerived> _logger { get; set; } = null!;

    public void DoSomething()
    {
        _logger.LogInformation($"{nameof(BarServiceDerived)} - {nameof(DoSomething)} called.");
        // Simulate some work
        _logger.LogInformation($"{nameof(BarServiceDerived)} - {nameof(DoSomething)} complete.");
    }
}