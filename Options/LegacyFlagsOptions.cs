namespace Ams.Media.Web.Options;
public sealed class LegacyFlagsOptions
{
    public bool SYSMultiLanguage { get; set; } = false;
    /// <summary>A=all, C=hide NOT USE</summary>
    public string ShowData { get; set; } = "C";
}
