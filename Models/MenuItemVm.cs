using System.Collections.Generic;

namespace Ams.Media.Web.Models
{
    // รวมเป็นเวอร์ชันเดียวให้ใช้ได้ทั้ง View และ MenuGate
    public class MenuItemVm
    {
        // ฟิลด์ที่ MenuGate/_Layout ใช้
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public string Controller { get; set; } = "";
        public string Action { get; set; } = "Index";
        public bool Enabled { get; set; } = false;

        // ฟิลด์เดิม (เผื่อโค้ดเก่าบางส่วนยังอ้างอิง)
        public string Text { get => Name; set => Name = value ?? ""; }
        public string Icon { get; set; } = "";
        public string Url { get; set; } = "";
        public List<MenuItemVm> Children { get; } = new();

        public MenuItemVm() { }

        // คงคอนสตรัคเตอร์แบบเดิม (text, icon, url)
        public MenuItemVm(string text, string icon, string url)
        {
            Name = text ?? "";
            Icon = icon ?? "";
            Url = url ?? "";
        }
    }
}
