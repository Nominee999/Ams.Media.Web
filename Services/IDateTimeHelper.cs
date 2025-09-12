namespace Ams.Media.Web.Services
{
    public interface IDateTimeHelper
    {
        string FormatDateShort(DateTime? dt);
        string FormatDateLong(DateTime? dt);
        string FormatTimeShort(TimeSpan? ts);
        string FormatTimeLong(TimeSpan? ts);
    }
}
