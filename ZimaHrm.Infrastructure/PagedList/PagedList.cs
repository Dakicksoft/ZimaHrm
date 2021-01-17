using System.Collections.Generic;
using System.Linq;
using ZimaHrm.Core.Infrastructure.Extensions;

namespace ZimaHrm.Core.Infrastructure.PagedList
{
    public class PagedList<T>
    {
        public PagedList(IQueryable<T> queryable, PagedListParameters parameters)
        {
            if (queryable == null || parameters == null)
            {
                return;
            }

            Count = queryable.LongCount();

            if (parameters.Order != null)
            {
                queryable = queryable.Order(parameters.Order.Property, parameters.Order.IsAscending);
            }

            if (parameters.Page != null)
            {
                queryable = queryable.Page(parameters.Page.Index, parameters.Page.Size);
            }

            List = queryable.AsEnumerable();
        }

        public long Count { get; }

        public IEnumerable<T> List { get; }
    }
}
