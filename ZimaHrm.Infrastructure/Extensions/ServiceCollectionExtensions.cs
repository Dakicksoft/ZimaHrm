using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace ZimaHrm.Core.Infrastructure.Extensions
{
  public static class ServiceCollectionExtensions
  {
    public static void AddTransientImplementations<T>(this IServiceCollection services)
    {
      foreach (Type type in typeof(T).Assembly.GetTypes().Where(Implements<T>))
        services.AddTransient(type.GetInterface("I" + type.Name), type);
    }

    public static void AddScopedImplementations<T>(this IServiceCollection services)
    {
      foreach (Type type in typeof(T).Assembly.GetTypes().Where(Implements<T>))
        services.AddScoped(type.GetInterface("I" + type.Name), type);
    }

    private static Boolean Implements<T>(Type type)
    {
      return typeof(T).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract;
    }
  }
}
