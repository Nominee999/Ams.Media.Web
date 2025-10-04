namespace Ams.Media.Web.Services
{
    public interface IUiStateService
    {
        /// <summary>คืนค่าโหมดปัจจุบัน: "C" (Current) หรือ "A" (All)</summary>
        Task<string> GetShowDataAsync();

        /// <summary>สลับค่า A<->C (toggle) แล้วคืนค่าใหม่</summary>
        Task<string> ToggleShowDataAsync();

        /// <summary>บังคับเซ็ตค่า (ใช้กับปุ่มแบบ Segmented/Dropdown): "A" หรือ "C"</summary>
        Task<string> SetShowDataAsync(string mode);
    }
}
