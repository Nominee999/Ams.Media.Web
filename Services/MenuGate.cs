using System.Security.Claims;
using Ams.Media.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace Ams.Media.Web.Services
{
    public class MenuGate : IMenuGate
    {
        private readonly AmsDbContext _db;
        public MenuGate(AmsDbContext db) { _db = db; }

        public bool CanMasterfiles(ClaimsPrincipal user) => ReadBool(user, "menu:M");
        public bool CanTransactions(ClaimsPrincipal user) => ReadBool(user, "menu:T");
        public bool CanReports(ClaimsPrincipal user) => ReadBool(user, "menu:R");
        public bool CanEnquirys(ClaimsPrincipal user) => ReadBool(user, "menu:E");
        public bool CanAddins(ClaimsPrincipal user) => ReadBool(user, "menu:A");
        public bool CanSystems(ClaimsPrincipal user) => ReadBool(user, "menu:S");

        private static bool ReadBool(ClaimsPrincipal user, string key)
            => bool.TryParse(user?.FindFirst(key)?.Value, out var b) && b;

        public Task<IReadOnlyList<MenuItemVm>> GetSubMenusAsync(char groupKey, ClaimsPrincipal user) => GetSubMenusAsync(groupKey.ToString(), user);

        public async Task<IReadOnlyList<MenuItemVm>> GetSubMenusAsync(string groupKey, ClaimsPrincipal user)
        {
            // ถ้าเมนูหลักถูกปิดใน claims → ไม่ต้องคืนเมนูย่อย
            if ((groupKey?.ToUpper()) switch
            {
                "M" => !CanMasterfiles(user),
                "T" => !CanTransactions(user),
                "R" => !CanReports(user),
                "E" => !CanEnquirys(user),
                "S" => !CanSystems(user),
                _ => true
            })
            {
                return Array.Empty<MenuItemVm>();
            }

            // อ่านสิทธิ์เมนูย่อยจาก security_menu
            // TODO: เลือก row ตามผู้ใช้/บทบาท หากตารางมีคีย์ผูกกับ user; ตอนนี้ใช้แถวแรกเพื่อให้ระบบรันได้
            var sm = await _db.SecurityMenus.AsNoTracking().FirstOrDefaultAsync();
            if (sm == null) return Array.Empty<MenuItemVm>();

            var list = new List<MenuItemVm>();

            switch (groupKey?.ToUpperInvariant())
            {
                case "M": // Masterfiles
                    list.Add(new MenuItemVm { Code = "mstation", Name = "Station", Controller = "mStation", Enabled = sm.MStation == "1" });
                    list.Add(new MenuItemVm { Code = "mclient", Name = "Client", Controller = "mClient", Enabled = sm.MClient == "1" });
                    list.Add(new MenuItemVm { Code = "mproduct", Name = "Product", Controller = "mProduct", Enabled = sm.MProduct == "1" });
                    list.Add(new MenuItemVm { Code = "mtargetgroup", Name = "Target Group", Controller = "mTargetGroup", Enabled = sm.MTargetGroup == "1" });
                    list.Add(new MenuItemVm { Code = "mcampaign", Name = "Campaign", Controller = "mCampaign", Enabled = sm.MCampaign == "1" });
                    list.Add(new MenuItemVm { Code = "mlanguage", Name = "Language", Controller = "mLanguage", Enabled = sm.MLanguage == "1" });
                    list.Add(new MenuItemVm { Code = "mmaterial", Name = "Material", Controller = "mMaterial", Enabled = sm.MMaterial == "1" });
                    list.Add(new MenuItemVm { Code = "mcurrency", Name = "Currency", Controller = "mCurrency", Enabled = sm.MCurrency == "1" });
                    list.Add(new MenuItemVm { Code = "mmediatype", Name = "Media Type", Controller = "mMediaType", Enabled = sm.MMediaType == "1" });
                    list.Add(new MenuItemVm { Code = "mvendor", Name = "Vendor", Controller = "mVendor", Enabled = sm.MVendor == "1" });
                    list.Add(new MenuItemVm { Code = "mratecode", Name = "Rate Code", Controller = "mRateCode", Enabled = sm.MRateCode == "1" });
                    list.Add(new MenuItemVm { Code = "mprogramtype", Name = "Program Type", Controller = "mProgramType", Enabled = sm.MProgramType == "1" });
                    list.Add(new MenuItemVm { Code = "mrateitem", Name = "Rate Item", Controller = "mRateItem", Enabled = sm.MRateItem == "1" });
                    list.Add(new MenuItemVm { Code = "mbooking", Name = "Booking", Controller = "mBooking", Enabled = sm.MBooking == "1" });
                    list.Add(new MenuItemVm { Code = "mtax", Name = "Tax", Controller = "mTax", Enabled = sm.MTax == "1" });
                    list.Add(new MenuItemVm { Code = "mdaypart", Name = "DayPart", Controller = "mDayPart", Enabled = sm.MDayPart == "1" });
                    break;

                case "T": // Transactions
                    list.Add(new MenuItemVm { Code = "tnew", Name = "New Plan", Controller = "tNewPlan", Enabled = sm.TNew == "1" });
                    list.Add(new MenuItemVm { Code = "tdelete", Name = "Delete Plan", Controller = "tDeletePlan", Enabled = sm.TDelete == "1" });
                    list.Add(new MenuItemVm { Code = "tbudget", Name = "Define Schedule", Controller = "tDefineSchedule", Enabled = sm.TBudget == "1" });
                    list.Add(new MenuItemVm { Code = "tselectorder", Name = "Select For Order", Controller = "tSelectOrder", Enabled = sm.TSelectOrder == "1" });
                    list.Add(new MenuItemVm { Code = "tselectinvoice", Name = "Select For Invoice", Controller = "tSelectInvoice", Enabled = sm.TSelectInvoice == "1" });
                    list.Add(new MenuItemVm { Code = "trefresh", Name = "Refresh Data", Controller = "tRefresh", Enabled = sm.TRefresh == "1" });
                    break;

                case "R": // Reports
                    list.Add(new MenuItemVm { Code = "rmasterreport", Name = "Master Files", Controller = "rMaster", Enabled = sm.RMaster == "1" });
                    list.Add(new MenuItemVm { Code = "rpurchase", Name = "Purchase Order", Controller = "rPurchase", Enabled = sm.RPurchase == "1" });
                    list.Add(new MenuItemVm { Code = "ramendment", Name = "Purchase Amendment", Controller = "rAmendment", Enabled = sm.RAmendment == "1" });
                    list.Add(new MenuItemVm { Code = "rinvoice", Name = "Invoice", Controller = "rInvoice", Enabled = sm.RInvoice == "1" });
                    list.Add(new MenuItemVm { Code = "rscheduleflow", Name = "Schedule / Flowchart", Controller = "rScheduleFlow", Enabled = sm.RScheduleFlow == "1" });
                    list.Add(new MenuItemVm { Code = "rrevenue", Name = "Revenue Report", Controller = "rRevenue", Enabled = sm.RRevenue == "1" });
                    list.Add(new MenuItemVm { Code = "rexpense", Name = "Expense Report", Controller = "rExpense", Enabled = sm.RExpense == "1" });
                    list.Add(new MenuItemVm { Code = "rjob", Name = "Job Report", Controller = "rJob", Enabled = sm.RJob == "1" });
                    list.Add(new MenuItemVm { Code = "rschedule", Name = "Schedule Report", Controller = "rSchedule", Enabled = sm.RSchedule == "1" });
                    list.Add(new MenuItemVm { Code = "rpurchasereport", Name = "Purchase Report", Controller = "rPurchaseRpt", Enabled = sm.RPurchaseRpt == "1" });
                    list.Add(new MenuItemVm { Code = "rbillingreport", Name = "Billing Report", Controller = "rBilling", Enabled = sm.RBilling == "1" });
                    list.Add(new MenuItemVm { Code = "ragencymargin", Name = "Agency Margin Report", Controller = "rAgencyMargin", Enabled = sm.RAgencyMargin == "1" });
                    list.Add(new MenuItemVm { Code = "rmonitor", Name = "Monitor Report", Controller = "rMonitor", Enabled = sm.RMonitor == "1" });
                    break;

                case "E": // Enquiry
                    list.Add(new MenuItemVm { Code = "epurchase", Name = "Purchase Order", Controller = "ePurchase", Enabled = sm.EPurchase == "1" });
                    list.Add(new MenuItemVm { Code = "einvoice", Name = "Invoice", Controller = "eInvoice", Enabled = sm.EInvoice == "1" });
                    list.Add(new MenuItemVm { Code = "etvrating", Name = "TV Rating", Controller = "eTVRating", Enabled = sm.ETVRating == "1" });
                    list.Add(new MenuItemVm { Code = "ereach", Name = "Reach", Controller = "eReach", Enabled = sm.EReach == "1" });
                    break;

                case "S": // Systems
                    list.Add(new MenuItemVm { Code = "ssetup", Name = "Setup", Controller = "sSetup", Enabled = sm.SSetup == "1" });
                    list.Add(new MenuItemVm { Code = "simporttvarating", Name = "Import TVA Rating", Controller = "sImportTVA", Enabled = sm.SImportTVA == "1" });
                    list.Add(new MenuItemVm { Code = "simporttvarating_arena", Name = "Import TVA Rating - Arena", Controller = "sImportArena", Enabled = sm.SImportArena == "1" });
                    break;
            }

            // คืนเฉพาะเมนูที่ Enabled = true
            return list.Where(x => x.Enabled).ToList();
        }
    }
}
