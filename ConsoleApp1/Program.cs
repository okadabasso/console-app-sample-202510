
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

var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";
Env.Load();
Env.Load($".env.{environment}");

Directory.SetCurrentDirectory(AppContext.BaseDirectory);


var host = Host.CreateDefaultBuilder(args)
        // Configurationの追加・上書き
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            config
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables();
        })
        // Loggingの設定
        .ConfigureLogging((hostingContext, logging) =>
        {
            logging.ClearProviders();
            logging.AddNLog();
            logging.SetMinimumLevel(LogLevel.Information);
        })
        .UseServiceProviderFactory(new DryIocServiceProviderFactory(
            new DryIoc.Container(rules => rules.With(propertiesAndFields: PropertiesAndFields.Auto))
        ))
        // DIコンテナにサービス登録
        .ConfigureServices((hostContext, services) =>
        {
            services.AddSingleton<IConfiguration>(hostContext.Configuration);
            services.Configure<SampleSetting>(hostContext.Configuration.GetSection("SampleSettings"));


            services.AddDbContext<SampleDbContext>(options =>
            {
                options.UseSqlite(hostContext.Configuration.GetConnectionString("default"));
                options.UseLoggerFactory(new NLogLoggerFactory());
            })
            ;

        })
        .ConfigureContainer<IContainer>((hostContext, container) =>
        {
            // Register configuration instance for DryIoc
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterServices(container, Assembly.GetExecutingAssembly());
            containerBuilder.RegisterInterceptors(container);
    
        })
        .Build();
ConsoleApp.ServiceProvider = host.Services;

var app = ConsoleApp.Create();

  
app.UseFilter<LoggingFilter>();
app.Run(args);


