namespace Ams.Media.Web.Dto
{
    public sealed class UsedByRow
    {
        public string TableName { get; set; } = "";
        public int RefCount { get; set; }
    }
}
