using System.Collections.Generic;

namespace ZimaHrm.Core.Infrastructure.Models
{
    public class PagedResult<T> where T : class
    {
        public PagedResult()
        {
            Data = new List<T>();
        }
        public List<T> Data { get; set; }
        public int TotalItems { get; set; } = 0;
    }
}
