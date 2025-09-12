using System;
using System.Collections.Generic;

namespace Ams.Media.Web.Models
{
    public class MenuItemVm
    {
        public string Key { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string? Url { get; set; }

        // เผื่อรองรับเมนูซ้อนในอนาคต
        public IReadOnlyList<MenuItemVm> Children { get; set; } = Array.Empty<MenuItemVm>();
        public bool Visible { get; set; } = true;
    }
}
