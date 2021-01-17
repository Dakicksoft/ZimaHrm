namespace ZimaHrm.Core.Infrastructure.Result
{
    public interface IDataResult<out T> : IResult
    {
        T Data { get; }
    }
}
