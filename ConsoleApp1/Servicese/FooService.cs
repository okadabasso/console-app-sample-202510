using ConsoleApp1.DependencyInjection.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using ConsoleApp1.Data;
using ConsoleApp1.Settings;
using Microsoft.Extensions.Options;
namespace ConsoleApp1.Services;

[Service]
public class FooService : IDisposable
{
    public  ILogger<FooService> _logger { get; set; } = null!;
    public IBarService _barService {get; set; } = null!;
    private IConfiguration _configuration { get; set; } = null!;
    private SampleDbContext _context { get; set; } = null!;
    private readonly SampleDataSeed _sampleDataSeed;

    private readonly SampleSetting _sampleSetting;
    public FooService(SampleDbContext context, IConfiguration configuration, IOptions<SampleSetting> sampleSetting, SampleDataSeed sampleDataSeed, ILogger<FooService> logger, IBarService barService)
    {
        _context = context;
        _configuration = configuration;
        _sampleSetting = sampleSetting.Value;
        _sampleDataSeed = sampleDataSeed;
        _logger = logger;
        _barService = barService;
    }
    [Trace]
    public virtual void DoSomething( int id, string name)
    {
        _logger.LogInformation($"{nameof(FooService)} - {nameof(DoSomething)} called. id: {id}, name: {name}");
        _logger.LogInformation($"environment {_sampleSetting.Description}");
        _sampleDataSeed.Seed();

        _barService.DoSomething();

        foreach (var record in _context.SampleData)
        {
            _logger.LogInformation($"Record: {record.Name}, CreatedAt: {record.CreatedAt:U}");
        }

        _logger.LogInformation($"$env:envvalue = {_configuration.GetValue<string>("envvalue")}");
        _logger.LogInformation($"SampleSetting.Name {_sampleSetting.Name}");
        _logger.LogInformation($"SampleSetting.Age {_sampleSetting.Age}");
        _logger.LogInformation($"SampleSetting.EnvValue {_sampleSetting.EnvValue}");
        _logger.LogInformation($"{nameof(FooService)} - {nameof(DoSomething)} complete.");
    }
    [Trace]
    public virtual async Task DoSomethingAsync()
    {
        _logger.LogInformation($"{nameof(FooService)} - {nameof(DoSomethingAsync)} called.");

        await Task.Delay(1000);
        _logger.LogInformation($"{nameof(FooService)} - {nameof(DoSomethingAsync)} complete.");
    }

    public void Dispose()
    {
        _logger.LogInformation($"{nameof(FooService)} - {nameof(Dispose)} called.");
    }
}