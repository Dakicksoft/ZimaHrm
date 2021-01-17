using System.Collections.Concurrent;

namespace ZimaHrm.Core.Infrastructure.Extensions
{
  public static class ConcurrentQueueExtensions
  {
    public static void Clear<T>(this ConcurrentQueue<T> queue)
    {
      T item;
      while (queue.TryDequeue(out item)) { }
    }
  }
}
