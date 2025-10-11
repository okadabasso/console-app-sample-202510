namespace ConsoleApp1.DependencyInjection;

using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using ConsoleApp1.Data;
using ConsoleApp1.DependencyInjection.Attributes;
using ConsoleApp1.DependencyInjection.Interceptors;
using ConsoleApp1.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System.Reflection;

class ContainerBuilder
{
    public IContainer BuildContainer(IServiceCollection services, params Assembly[] assemblies)
    {
        var container = new DryIoc.Container(rules => rules.With(propertiesAndFields: PropertiesAndFields.Auto))
            .WithDependencyInjectionAdapter(services)
            ;

        // Register services and interceptors
        RegisterServices(container, assemblies);
        RegisterInterceptors(container);

        return container;
    }
    
    public void RegisterServices(IContainer container, params Assembly[] assemblies)
    {
        if (assemblies.Length == 0)
        {
            assemblies = new[] { Assembly.GetExecutingAssembly() };
        }
        foreach (var assembly in assemblies)
        {
            RegisterAssemblyService(container, assembly);
        }
    }
    
    private void RegisterAssemblyService(IContainer container, Assembly assembly)
    {
        var types = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.GetCustomAttribute<ServiceAttribute>(inherit: true) != null)
            .ToArray();
        
        foreach (var type in types)
        {
            var serviceAttribute = type.GetCustomAttribute<ServiceAttribute>(inherit: true);
            if (serviceAttribute == null)
            {
                continue; // Skip if no ServiceAttribute is present
            }
            if (serviceAttribute.Implements != null && serviceAttribute.Implements.Length > 0)
            {
                foreach (var serviceType in serviceAttribute.Implements)
                {
                    container.Register(serviceType, type, Reuse.Transient, ifAlreadyRegistered: IfAlreadyRegistered.Replace);
                }
                continue; // Skip registering the type itself if it has specific implementations
            }
            var interfaces = type.GetInterfaces().Where(t => t.GetCustomAttribute<ServiceAttribute>() != null).ToArray();
            if (interfaces.Length > 0)
            {
                foreach (var @interface in interfaces)
                {
                    container.Register(@interface, type, Reuse.Scoped, ifAlreadyRegistered: IfAlreadyRegistered.Replace);
                }
            }
            else
            {
                container.Register(type, Reuse.Scoped, ifAlreadyRegistered: IfAlreadyRegistered.Replace);
            }
        }
    }
    
    public void RegisterInterceptors(IContainer container)
    {
        if(container.IsRegistered<TraceInterceptor>() == false)
        {
            container.Register<TraceInterceptor>(Reuse.Transient);
        }
        if (container.IsRegistered<TraceInterceptorAsync>() == false)
        {
            container.Register<TraceInterceptorAsync>(Reuse.Transient);
        }
        container.Intercept(t =>  !(t.GetCustomAttribute<ServiceAttribute>()?.Transactional) ?? false, new Type[] { typeof(TraceInterceptor) });
    }
}