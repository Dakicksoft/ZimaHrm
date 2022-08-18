using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using System;
using System.Reflection;
using ZimaHrm.Core.Infrastructure.Logging;
using ZimaHrm.Core.Infrastructure.Security;


namespace ZimaHrm.Core.Infrastructure.IoC
{
    public static class ServiceCollectionExtensions
    {
       
        //public static void AddDbContextMemory<T>(this IServiceCollection services) where T : DbContext
        //{
        //    services.AddDbContextPool<T>(options => options.UseInMemoryDatabase(typeof(T).Name));
        //    services.GetService<T>().Database.EnsureCreated();
        //}

        public static void AddDbContextMigrate<T>(this IServiceCollection services, Action<DbContextOptionsBuilder> options) where T : DbContext
        {
            services.AddDbContextPool<T>(options);
            services.GetService<T>().Database.Migrate();
        }


        public static void AddJsonWebToken(this IServiceCollection services, string key, TimeSpan expires)
        {
            services.AddJsonWebToken(new JsonWebTokenSettings(key, expires));
        }

        public static void AddJsonWebToken(this IServiceCollection services, string key, TimeSpan expires, string audience, string issuer)
        {
            services.AddJsonWebToken(new JsonWebTokenSettings(key, expires, audience, issuer));
        }

        public static void AddJsonWebToken(this IServiceCollection services, JsonWebTokenSettings jsonWebTokenSettings)
        {
            services.AddSingleton<IJsonWebTokenSettings>(_ => jsonWebTokenSettings);
            services.AddSingleton<IJsonWebToken, JsonWebToken>();
        }

        public static void AddLogger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ILogger>(_ => new Logger(configuration));
        }

        public static void AddMatchingInterface(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.Scan(scan => scan
                .FromAssemblies(assemblies)
                .AddClasses()
                .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                .AsMatchingInterface()
                .WithScopedLifetime());
        }

        public static T GetService<T>(this IServiceCollection services)
        {
            return services.BuildServiceProvider().GetService<T>();
        }
    }
}
