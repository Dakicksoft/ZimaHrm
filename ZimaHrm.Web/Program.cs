using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;

namespace ZimaHrm.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog((context, services, configuration) => Infrastructure.SerilogLogging.InitLogger(context, context.Configuration, configuration))
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>()
                                                                    .UseWebRoot(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"))
                                                                    .UseContentRoot(Directory.GetCurrentDirectory()); });

    }
}
