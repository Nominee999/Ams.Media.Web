using System.ComponentModel.DataAnnotations;

namespace Ams.Media.Web.Models
{
    public class LoginVm
    {
        [Required]
        public string? UserName { get; set; }

        [Required, DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
