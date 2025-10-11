namespace ConsoleApp1.Services;

using ConsoleAppFramework;

[ConsoleAppFramework.RegisterCommands("sample")]
class SampleCommand
{
    public void Execute()
    {
        Console.WriteLine("SampleCommand executed.");
    }
}