namespace Ams.Media.Web.Services
{
    public class QueryService : IQueryService
    {
        public IQueryable<T> Page<T>(IQueryable<T> q, int page, int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 20;
            return q.Skip((page - 1) * pageSize).Take(pageSize);
        }
    }
}
