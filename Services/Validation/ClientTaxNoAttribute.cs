using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Ams.Media.Web.Services.Validation
{
    /// <summary>
    /// เลขผู้เสียภาษีไทย: ถ้าใส่ต้องเป็นตัวเลขติดกัน 13 หลัก; ถ้าไม่ใส่ = ว่างได้
    /// </summary>
    public sealed class ClientTaxNoAttribute : ValidationAttribute
    {
        public ClientTaxNoAttribute() : base("ClientTaxNo must be 13 digits or empty.") { }

        public override bool IsValid(object? value)
        {
            var s = (value as string)?.Trim();
            if (string.IsNullOrEmpty(s)) return true; // ว่างได้
            return Regex.IsMatch(s, @"^\d{13}$");
        }
    }
}
