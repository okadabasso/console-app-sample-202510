using Castle.Core.Internal;
using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text;
using ConsoleApp1.DependencyInjection.Attributes;

namespace ConsoleApp1.DependencyInjection.Interceptors;

public class TraceInterceptorAsync : IAsyncInterceptor
{
    private readonly ILoggerFactory _loggerFactory;

    public TraceInterceptorAsync(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }
    /// <summary>
    /// 非同期メソッドのインターセプト
    /// 
    /// 戻り値が Task の場合
    /// </summary>
    /// <param name="invocation"></param>
    public void InterceptAsynchronous(IInvocation invocation)
    {
        if (!CanIntercept(invocation))
        {
            invocation.Proceed();
            return;
        }
        var logger = _loggerFactory.CreateLogger(invocation.TargetType!);
        logger.LogTrace(FormatLogMessageWithParameter(invocation, "start"));
        invocation.ReturnValue = InternalInterceptAsynchronous(invocation, logger);
    }

    /// <summary>
    /// 非同期メソッドのインターセプト
    /// 
    /// 戻り値がTask<TResult>の場合
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="invocation"></param>
    public void InterceptAsynchronous<TResult>(IInvocation invocation)
    {
        if (!CanIntercept(invocation))
        {
            invocation.Proceed();
            return;
        }
        var logger = _loggerFactory.CreateLogger(invocation.TargetType!);
        logger.LogTrace(FormatLogMessageWithParameter(invocation, "start"));
        invocation.ReturnValue = InternalInterceptAsynchronous<TResult>(invocation, logger);
    }
    /// <summary>
    /// 同期メソッドのインターセプト
    /// </summary>
    /// <param name="invocation"></param>
    public void InterceptSynchronous(IInvocation invocation)
    {
        if (!CanIntercept(invocation))
        {
            invocation.Proceed();
            return;
        }

        var logger = _loggerFactory.CreateLogger(invocation.TargetType!);
        logger.LogTrace(FormatLogMessageWithParameter(invocation, "start"));
        try
        {
            invocation.Proceed();

        }
        catch (Exception ex)
        {
            logger.LogError(ex, FormatLogMessage(invocation, "error"));
            throw;
        }
        finally
        {
            logger.LogTrace(FormatLogMessage(invocation, "end"));
        }
    }
    private async Task InternalInterceptAsynchronous(IInvocation invocation, ILogger logger)
    {
        invocation.Proceed();

        var task = (Task)invocation.ReturnValue!;
        try
        {
            await task;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, FormatLogMessage(invocation, "error"));
            throw;
        }
        finally
        {
            logger.LogTrace(FormatLogMessage(invocation, "end"));
        }
    }

    private async Task<TResult> InternalInterceptAsynchronous<TResult>(IInvocation invocation, ILogger logger)
    {
        invocation.Proceed();
        var task = (Task<TResult>)invocation.ReturnValue!;
        try
        {
            TResult result = await task;
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, FormatLogMessage(invocation, "error"));
            throw;
        }
        finally
        {
            logger.LogTrace(FormatLogMessage(invocation, "end"));
        }
    }
    string FormatLogMessage(IInvocation invocation, string message)
    {

        return $"{invocation.TargetType?.Name}.{invocation.MethodInvocationTarget?.Name} {message}";
    }
    string FormatLogMessageWithParameter(IInvocation invocation, string message)
    {
        var messageBuilder = new StringBuilder();
        messageBuilder.Append($"{invocation.TargetType?.Name}.{invocation.MethodInvocationTarget?.Name} {message}");
        if (invocation.Arguments != null && invocation.Arguments.Length > 0)
        {
            messageBuilder.Append(" Arguments: ");
            for (int i = 0; i < invocation.Arguments.Length; i++)
            {
                messageBuilder.Append($"[{i}] {invocation.Arguments[i] ?? "null"} ");
            }
        }

        return messageBuilder.ToString();
    }

    private bool CanIntercept(IInvocation invocation)
    {
        if (invocation.MethodInvocationTarget == null)
        {
            return false;
        }
        // メソッドに Trace 属性があれば許可
        if (invocation.MethodInvocationTarget.GetCustomAttribute<TraceAttribute>() != null)
        {
            return true;
        }
        // インターフェースのメソッドに Trace 属性があれば許可
        if (invocation.TargetType != null)
        {
            foreach (var @interface in invocation.TargetType.GetInterfaces())
            {
                var map = invocation.TargetType.GetInterfaceMap(@interface);
                for (int i = 0; i < map.TargetMethods.Length; i++)
                {
                    if (map.TargetMethods[i] == invocation.MethodInvocationTarget)
                    {
                        var interfaceMethod = map.InterfaceMethods[i];
                        if (interfaceMethod.GetCustomAttribute<TraceAttribute>() != null)
                        {
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }
}