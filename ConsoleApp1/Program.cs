
using ConsoleApp1;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using NLog.Extensions.Logging;
using DotNetEnv;
using System.Reflection;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using ConsoleAppFramework;
using ConsoleApp1.DependencyInjection;
using ConsoleApp1.Filters;
using ConsoleApp1.Data;
using ConsoleApp1.Settings;
using ConsoleApp1.Services;
using ConsoleApp1.DependencyInjection.Interceptors;
using ConsoleApp1.DependencyInjection.Attributes;

var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";
Env.Load();
Env.Load($".env.{environment}");

Directory.SetCurrentDirectory(AppContext.BaseDirectory);

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

var services = new ServiceCollection();
services.AddSingleton<IConfiguration>(configuration);
services.Configure<SampleSetting>(configuration.GetSection("SampleSettings"));
services.AddDbContext<SampleDbContext>(options =>
{
    options.UseSqlite(configuration.GetConnectionString("default"));
    options.UseLoggerFactory(new NLogLoggerFactory());
})
.AddLogging(builder =>
{
    builder.ClearProviders();
    builder.AddNLog();
    builder.SetMinimumLevel(LogLevel.Information);
});
var container = new DryIoc.Container(rules => rules.With(propertiesAndFields: PropertiesAndFields.Auto))
    .WithDependencyInjectionAdapter(services)
;
container.Register<SampleDataSeed>(Reuse.Singleton);
container.Register<FooService>(Reuse.Transient);
container.Register<IBarService, BarService>(Reuse.Transient);
container.Register<TraceInterceptor>(Reuse.Transient);
container.Register<TraceInterceptorAsync>(Reuse.Transient);

container.Intercept<TraceInterceptor>(type => type.GetMethods().Any(m => m.GetCustomAttribute<TraceAttribute>() != null));

var serviceProvider = container.BuildServiceProvider();

ConsoleApp.ServiceProvider = serviceProvider;

var app = ConsoleApp.Create();

  
app.UseFilter<LoggingFilter>();
app.Run(args);


