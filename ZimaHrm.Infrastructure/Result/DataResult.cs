using System.Threading.Tasks;

namespace ZimaHrm.Core.Infrastructure.Result
{
    public class DataResult<T> : Result, IDataResult<T>
    {
        public DataResult(T data, bool success, string message) : base(success, message)
        {
            Data = data;
        }

        public DataResult(T data, bool success) : base(success)
        {
            Data = data;
        }

        public T Data { get; }

        public new Task<IDataResult<T>> ToTask()
        {
            return Task.FromResult(this as IDataResult<T>);
        }
    }
}
