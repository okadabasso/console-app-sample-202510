using System.Threading.Tasks;
using ConsoleApp1;
using ConsoleApp1.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DryIoc;
namespace ConsoleApp1.Commands;

[ConsoleAppFramework.RegisterCommands("foo")]
public class FooCommand
{
    private readonly IServiceProvider _provider;
    private readonly ILogger<FooCommand> _logger;
    public FooCommand(IServiceProvider provider, ILogger<FooCommand> logger)
    {
        // Use DryIoc to resolve the container
        _provider = provider;
        _logger = logger;
    }

    [ConsoleAppFramework.Command("execute")]
    public async Task ExecuteAsync()
    {
        _logger.LogInformation("Executing SampleCommand...");
        using (var scope = _provider.CreateScope())
        {
            // Resolve the required services within the scope
            var fooService = scope.ServiceProvider.GetRequiredService<FooService>();
            if (fooService == null)
            {
                _logger.LogError("FooService is not registered.");
                throw new InvalidOperationException("FooService is not registered.");
            }
            fooService.DoSomething(1, "test");
            await fooService.DoSomethingAsync();
        }
        _logger.LogInformation("SampleCommand executed successfully.");
        return;
    }
}

