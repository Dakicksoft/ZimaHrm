namespace ZimaHrm.Core.Infrastructure.Logging
{
    public static class LoggerExtensions
    {
        public static Serilog.ILogger Data(this Serilog.ILogger log, object data)
        {
            return log.ForContext("@Data", data, true);
        }
    }
}
