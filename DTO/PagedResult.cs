namespace Ams.Media.Web.Dto
{
    public sealed class PagedResult<T>
    {
        public IReadOnlyList<T> Items { get; }
        public int Total { get; }
        public int Page { get; }
        public int PageSize { get; }

        public PagedResult(IReadOnlyList<T> items, int total, int page, int pageSize)
        {
            Items = items;
            Total = total;
            Page = page;
            PageSize = pageSize;
        }

        public void Deconstruct(out IReadOnlyList<T> items, out int total, out int page, out int pageSize)
        {
            items = Items;
            total = Total;
            page = Page;
            pageSize = PageSize;
        }
    }
}
