namespace ZimaHrm.Core.Infrastructure.Result
{
    public interface IResult
    {
        string Message { get; }

        bool Success { get; }
    }
}
