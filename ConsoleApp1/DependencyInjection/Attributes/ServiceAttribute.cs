namespace ConsoleApp1.DependencyInjection.Attributes;
using System;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]

public class ServiceAttribute : Attribute
{
    public bool Transactional { get; set; } = false;
    public Type[] Implements { get; set; } = Array.Empty<Type>();

    public ServiceAttribute()
    {
    }
    public ServiceAttribute(bool transactional, params Type[] implements)
    {
        Transactional = transactional;
        if (implements == null || implements.Length == 0)
        {
            Implements = Array.Empty<Type>();
            return;
        }

        Implements = implements;
    }
}

