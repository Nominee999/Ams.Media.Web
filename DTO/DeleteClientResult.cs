using System.Collections.Generic;

namespace Ams.Media.Web.Dto
{
    public sealed class DeleteClientResult
    {
        public bool IsSuccess { get; private set; }
        public IReadOnlyList<UsedByRow> UsedBy { get; private set; } = new List<UsedByRow>();

        public static DeleteClientResult Ok() => new() { IsSuccess = true };
        public static DeleteClientResult Blocked(IReadOnlyList<UsedByRow> usedBy)
            => new() { IsSuccess = false, UsedBy = usedBy };
    }
}
