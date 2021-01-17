using System.Collections.Generic;

namespace ZimaHrm.Core.Infrastructure.PagedList
{
    public class PagedListParameters
    {
        public PagedListParameters()
        {
        }

        public PagedListParameters(Order order)
        {
            Order = order;
        }

        public PagedListParameters(Page page)
        {
            Page = page;
        }

        public PagedListParameters(Order order, Page page)
        {
            Order = order;
            Page = page;
        }

        public PagedListParameters(string property, bool isAscending)
        {
            Order = new Order(property, isAscending);
        }

        public PagedListParameters(int index, int size)
        {
            Page = new Page(index, size);
        }

        public PagedListParameters(string property, bool isAscending, int index, int size)
        {
            Order = new Order(property, isAscending);
            Page = new Page(index, size);
        }

        public Order Order { get; set; }

        public Page Page { get; set; }
    }
}
