using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Ams.Media.Web.Services.Validation
{
    /// <summary>
    /// 2–4 ตัวอักษร A–Z หรือ 0–9
    /// </summary>
    public sealed class ClientPrefixAttribute : ValidationAttribute
    {
        public ClientPrefixAttribute() : base("ClientPrefix must be 2–4 letters/digits.") { }

        public override bool IsValid(object? value)
        {
            var s = (value as string)?.Trim() ?? "";
            if (s.Length == 0) return true; // ไม่บังคับกรอก
            return Regex.IsMatch(s, @"^[A-Za-z0-9]{2,4}$");
        }
    }
}
