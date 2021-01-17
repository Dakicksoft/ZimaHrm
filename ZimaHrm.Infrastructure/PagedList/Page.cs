namespace ZimaHrm.Core.Infrastructure.PagedList
{
    public class Page
    {
        public Page()
        {
        }

        public Page(int index, int size)
        {
            Index = index;
            Size = size;
        }

        public int Index { get; set; }

        public int Size { get; set; }
    }
}
