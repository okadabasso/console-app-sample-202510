using Microsoft.Extensions.Logging;
using ConsoleAppFramework;
using Microsoft.Extensions.DependencyInjection;
namespace ConsoleApp1.Filters;

internal class LoggingFilter(ConsoleAppFilter next) : ConsoleAppFilter(next) // ctor needs `ConsoleAppFilter next` and call base(next)
{
    // implement InvokeAsync as filter body
    public override async Task InvokeAsync(ConsoleAppContext context, CancellationToken cancellationToken)
    {
        var logger = ConsoleApp.ServiceProvider?.GetRequiredService<ILogger<LoggingFilter>>();
        // You can access the logger from the context
        logger?.LogInformation(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff ") + context.CommandName + " start");
        try
        {
            /* on before */
            await Next.InvokeAsync(context, cancellationToken); // invoke next filter or command body
            /* on after */
        }
        catch
        {
            /* on error */
            throw;
        }
        finally
        {
            logger?.LogInformation(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff ") + context.CommandName + " end");
        }
    }
}