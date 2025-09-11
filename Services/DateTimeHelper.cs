using System.Globalization;

namespace Ams.Media.Web.Services
{
    public class DateTimeHelper : IDateTimeHelper
    {
        private static string D_Short = "dd/MM/yyyy";
        private static string D_Long = "dd/MMM/yyyy";
        private static string T_Short = "HH:mm";
        private static string T_Long = "HH:mm:ss";

        public string FormatDateShort(DateTime? dt) => dt.HasValue ? dt.Value.ToString(D_Short, CultureInfo.InvariantCulture) : "";
        public string FormatDateLong(DateTime? dt) => dt.HasValue ? dt.Value.ToString(D_Long, CultureInfo.InvariantCulture) : "";
        public string FormatTimeShort(TimeSpan? ts) => ts.HasValue ? DateTime.UnixEpoch.Add(ts.Value).ToString(T_Short, CultureInfo.InvariantCulture) : "";
        public string FormatTimeLong(TimeSpan? ts) => ts.HasValue ? DateTime.UnixEpoch.Add(ts.Value).ToString(T_Long, CultureInfo.InvariantCulture) : "";
    }
}
