namespace ZimaHrm.Core.Infrastructure.PagedList
{
    public class Order
    {
        public Order()
        {
        }

        public Order(string property, bool isAscending)
        {
            Property = property;
            IsAscending = isAscending;
        }

        public bool IsAscending { get; set; }

        public string Property { get; set; }
    }
}
