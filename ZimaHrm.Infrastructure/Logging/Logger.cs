using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace ZimaHrm.Core.Infrastructure.Logging
{
    public class Logger : ILogger
    {

        private Func<Int32?> AccountId { get; }
        private readonly IConfiguration Config;

        private static Object LogWriting = new Object();

        public Logger(IConfiguration configuration)
        {
            IHttpContextAccessor accessor = new HttpContextAccessor();
            AccountId = () => int.Parse(accessor.HttpContext?.User.Claims.First(s => s.Type == "Id").Value);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom
                .Configuration(configuration)
                .Enrich.WithMachineName()
                .Enrich.WithProcessId()
                .Enrich.WithThreadId()
                .CreateLogger();
        }
        public Logger(IConfiguration config, Int32? accountId)
        {
            AccountId = () => accountId;
            Config = config;
        }


        public void Debug(string message)
        {
            Log.Debug(message);
        }

        public void Debug(string message, object data)
        {
            Log.Logger.Data(data).Debug(message);
        }

        public void Error(Exception exception)
        {
            Log.Error(exception, exception.Message);
        }

        public void Error(Exception exception, object data)
        {
            Log.Logger.Data(data).Error(exception, exception.Message);
        }

        public void Error(string message)
        {
            Log.Error(message);
        }

        public void Error(string message, object data)
        {
            Log.Logger.Data(data).Error(message);
        }

        public void Fatal(Exception exception)
        {
            Log.Fatal(exception, exception.Message);
        }

        public void Fatal(Exception exception, object data)
        {
            Log.Logger.Data(data).Fatal(exception, exception.Message);
        }

        public void Information(string message)
        {
            Log.Information(message);
        }

        public void Information(string message, object data)
        {
            Log.Logger.Data(data).Information(message);
        }

        public void Warning(string message)
        {
            Log.Warning(message);
        }

        public void Warning(string message, object data)
        {
            Log.Logger.Data(data).Warning(message);
        }

        public void LogFile(string message)
        {
            String logDirectory = Path.Combine(Config["Application:Path"], Config["Logger:Directory"]);
            Int64 backupSize = Int64.Parse(Config["Logger:BackupSize"]);
            String logPath = Path.Combine(logDirectory, "Log.txt");

            StringBuilder log = new StringBuilder();
            log.AppendLine("Time   : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            log.AppendLine("Account: " + AccountId());
            log.AppendLine("Message: " + message);
            log.AppendLine();

            lock (LogWriting)
            {
                Directory.CreateDirectory(logDirectory);
                System.IO.File.AppendAllText(logPath, log.ToString());

                if (new FileInfo(logPath).Length >= backupSize)
                    System.IO.File.Move(logPath, Path.Combine(logDirectory, $"Log {DateTime.Now:yyyy-MM-dd HHmmss}.txt"));
            }
        }
        public void LogFile(Exception exception)
        {
            while (exception.InnerException != null)
                exception = exception.InnerException;

            LogFile($"{exception.GetType()}: {exception.Message}{Environment.NewLine}{exception.StackTrace}");
        }
    }
}
