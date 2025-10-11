using ConsoleApp1.DependencyInjection.Attributes;

namespace ConsoleApp1.Services;

[Service]
public interface IBarService
{
    [Trace]
    public void DoSomething();
}