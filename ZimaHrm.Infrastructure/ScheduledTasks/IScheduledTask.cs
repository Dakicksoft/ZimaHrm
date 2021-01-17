using System;
using System.Threading;
using System.Threading.Tasks;

namespace ZimaHrm.Core.Infrastructure.ScheduledTasks
{
    public interface IScheduledTask: IDisposable
  {
        string Schedule { get; }
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
