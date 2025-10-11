namespace ConsoleApp1.DependencyInjection.Interceptors;

using Castle.DynamicProxy;

public class TraceInterceptor : IInterceptor
{
    private readonly IAsyncInterceptor _asyncInterceptor;
    public TraceInterceptor(TraceInterceptorAsync asyncInterceptor)
    {
        _asyncInterceptor = asyncInterceptor;
    }
    public void Intercept(IInvocation invocation)
    {
        _asyncInterceptor.ToInterceptor().Intercept(invocation);
    }
}