// D:\VS2022\Ams.Media.Web\Services\MenuGate.cs
using System.Security.Claims;
using Ams.Media.Web.Data;
using Ams.Media.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Ams.Media.Web.Services
{
    public class MenuGate : IMenuGate
    {
        private readonly AmsDbContext _db;
        public MenuGate(AmsDbContext db) => _db = db;

        public async Task<IReadOnlyList<MenuItemVm>> GetSubMenusAsync(string groupKey, ClaimsPrincipal user)
        {
            var menus = new List<MenuItemVm>();

            var username = (user?.Identity?.Name ?? "").Trim();
            // อ่าน row สิทธิ์จาก SECURITY_MENU โดย key = username
            // ถ้าไม่พบ ให้คืนลิสต์ว่าง (หรือจะ FirstOrDefault ทั้งตารางก็ได้หากต้องการ default)
            var sm = await _db.SecurityMenus.AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Username == username);
            if (sm == null) return menus;

            // helper แปลง '1' → true
            bool On(string? s) => string.Equals(s?.Trim(), "1", StringComparison.Ordinal);

            switch (groupKey.ToUpperInvariant())
            {
                // ===== Masterfiles =====
                case "M":
                    if (On(sm.MClient)) menus.Add(new MenuItemVm { Name = "Client", Controller = "mClient", Action = "Index" });
                    if (On(sm.MCampaign)) menus.Add(new MenuItemVm { Name = "Campaign", Controller = "mCampaign", Action = "Index" });
                    if (On(sm.MProduct)) menus.Add(new MenuItemVm { Name = "Product", Controller = "mProduct", Action = "Index" });
                    if (On(sm.MVendor)) menus.Add(new MenuItemVm { Name = "Vendor", Controller = "mVendor", Action = "Index" });
                    if (On(sm.MMedia)) menus.Add(new MenuItemVm { Name = "Media", Controller = "mMedia", Action = "Index" });

                    if (On(sm.MStation)) menus.Add(new MenuItemVm { Name = "Station", Controller = "mStation", Action = "Index" });
                    if (On(sm.MTarget)) menus.Add(new MenuItemVm { Name = "Target Group", Controller = "mTargetGroup", Action = "Index" });
                    if (On(sm.MLanguage)) menus.Add(new MenuItemVm { Name = "Language", Controller = "mLanguage", Action = "Index" });
                    if (On(sm.MMaterial)) menus.Add(new MenuItemVm { Name = "Material", Controller = "mMaterial", Action = "Index" });
                    if (On(sm.MRateCode)) menus.Add(new MenuItemVm { Name = "Rate Code", Controller = "mRateCode", Action = "Index" });
                    if (On(sm.MProgramType)) menus.Add(new MenuItemVm { Name = "Program Type", Controller = "mProgramType", Action = "Index" });
                    if (On(sm.MRateItem)) menus.Add(new MenuItemVm { Name = "Rate Item", Controller = "mRateItem", Action = "Index" });
                    if (On(sm.MBooking)) menus.Add(new MenuItemVm { Name = "Booking", Controller = "mBooking", Action = "Index" });
                    if (On(sm.MTax)) menus.Add(new MenuItemVm { Name = "Tax", Controller = "mTax", Action = "Index" });
                    if (On(sm.MDayPart)) menus.Add(new MenuItemVm { Name = "DayPart", Controller = "mDayPart", Action = "Index" });
                    if (On(sm.MCurrency)) menus.Add(new MenuItemVm { Name = "Currency", Controller = "mCurrency", Action = "Index" });
                    // note: ในสคริปต์มี mloading/mpackage/mcirculation/mspottype/mprogrammaster ด้วย
                    break;

                // ===== Transactions =====
                case "T":
                    if (On(sm.TNew)) menus.Add(new MenuItemVm { Name = "New Plan", Controller = "tNewPlan", Action = "Index" });
                    if (On(sm.TDelete)) menus.Add(new MenuItemVm { Name = "Delete Plan", Controller = "tDeletePlan", Action = "Index" });
                    if (On(sm.TBudget)) menus.Add(new MenuItemVm { Name = "Define Schedule", Controller = "tDefineSchedule", Action = "Index" });
                    if (On(sm.TSelectOrder)) menus.Add(new MenuItemVm { Name = "Select Order", Controller = "tSelectOrder", Action = "Index" });
                    if (On(sm.TSelectInvoice)) menus.Add(new MenuItemVm { Name = "Select Invoice", Controller = "tSelectInvoice", Action = "Index" });
                    if (On(sm.TRefresh)) menus.Add(new MenuItemVm { Name = "Refresh Data", Controller = "tRefresh", Action = "Index" });
                    // extras: tdeleteitem, tjob, tschedule
                    break;

                // ===== Reports =====
                case "R":
                    if (On(sm.RMasterReport)) menus.Add(new MenuItemVm { Name = "Master Files", Controller = "rMasterFiles", Action = "Index" });
                    if (On(sm.RPurchase)) menus.Add(new MenuItemVm { Name = "Purchase Order", Controller = "rPurchaseOrder", Action = "Index" });
                    if (On(sm.RAmendment)) menus.Add(new MenuItemVm { Name = "Purchase Amendment", Controller = "rAmendment", Action = "Index" });
                    if (On(sm.RInvoice)) menus.Add(new MenuItemVm { Name = "Invoice", Controller = "rInvoice", Action = "Index" });
                    if (On(sm.RRevenue)) menus.Add(new MenuItemVm { Name = "Revenue Report", Controller = "rRevenue", Action = "Index" });
                    if (On(sm.RExpense)) menus.Add(new MenuItemVm { Name = "Expense Report", Controller = "rExpense", Action = "Index" });
                    if (On(sm.RBilling)) menus.Add(new MenuItemVm { Name = "Billing Report", Controller = "rBilling", Action = "Index" });
                    if (On(sm.RMargin)) menus.Add(new MenuItemVm { Name = "Agency Margin", Controller = "rMargin", Action = "Index" });
                    if (On(sm.RMonitor)) menus.Add(new MenuItemVm { Name = "Monitor Report", Controller = "rMonitor", Action = "Index" });
                    if (On(sm.ROrder)) menus.Add(new MenuItemVm { Name = "Order Report", Controller = "rOrder", Action = "Index" });
                    if (On(sm.RJob)) menus.Add(new MenuItemVm { Name = "Job Report", Controller = "rJob", Action = "Index" });
                    if (On(sm.RSchedule)) menus.Add(new MenuItemVm { Name = "Schedule Report", Controller = "rSchedule", Action = "Index" });
                    if (On(sm.RPurchaseRpt)) menus.Add(new MenuItemVm { Name = "Purchase Report", Controller = "rPurchaseReport", Action = "Index" });
                    break;

                // ===== Enquiry =====
                case "E":
                    if (On(sm.EPurchase)) menus.Add(new MenuItemVm { Name = "Purchase Order", Controller = "ePurchaseOrder", Action = "Index" });
                    if (On(sm.EInvoice)) menus.Add(new MenuItemVm { Name = "Invoice", Controller = "eInvoice", Action = "Index" });
                    if (On(sm.ERating)) menus.Add(new MenuItemVm { Name = "TV Rating", Controller = "eTvRating", Action = "Index" });
                    if (On(sm.EReach)) menus.Add(new MenuItemVm { Name = "Reach", Controller = "eReach", Action = "Index" });
                    break;

                // ===== Systems =====
                case "S":
                    if (On(sm.SSetup)) menus.Add(new MenuItemVm { Name = "Setup", Controller = "sSetup", Action = "Index" });
                    if (On(sm.SBackup)) menus.Add(new MenuItemVm { Name = "Backup", Controller = "sBackup", Action = "Index" });
                    if (On(sm.SRating)) menus.Add(new MenuItemVm { Name = "Import TVA Rating", Controller = "sImportTVA", Action = "Index" });
                    // มี saddins.., sreach ฯลฯ ตามสคริปต์ด้วย
                    break;
            }

            return menus;
        }
    }
}
