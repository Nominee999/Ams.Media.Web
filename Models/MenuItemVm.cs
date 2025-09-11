namespace Ams.Media.Web.Models
{
    /// <summary>ViewModel สำหรับเมนูย่อย</summary>
    public class MenuItemVm
    {
        public string Controller { get; set; } = "";
        public string Action { get; set; } = "";
        public string Name { get; set; } = "";

        public MenuItemVm() { }
        public MenuItemVm(string controller, string action, string name)
        {
            Controller = controller;
            Action = action;
            Name = name;
        }
    }
}
