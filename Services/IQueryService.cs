namespace Ams.Media.Web.Services
{
    public interface IQueryService
    {
        IQueryable<T> Page<T>(IQueryable<T> q, int page, int pageSize);
    }
}
