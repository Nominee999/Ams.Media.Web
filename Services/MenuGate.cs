using System.Security.Claims;
using Ams.Media.Web.Data;
using Ams.Media.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Ams.Media.Web.Services
{
    public class MenuGate : IMenuGate
    {
        private readonly AmsDbContext _db;
        public MenuGate(AmsDbContext db) { _db = db; }

        public async Task<IReadOnlyList<MenuItemVm>> GetSubMenusAsync(string groupKey, ClaimsPrincipal user)
        {
            var menus = new List<MenuItemVm>();

            // โหลดสิทธิของผู้ใช้คนนี้เท่านั้น (PK = username)
            var username = user?.Identity?.Name?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(username)) return menus;

            var sm = await _db.SecurityMenus
                              .AsNoTracking()
                              .SingleOrDefaultAsync(x => x.Username == username);
            if (sm == null) return menus;

            bool On(string? v) => string.Equals(v?.Trim(), "1", StringComparison.Ordinal);

            switch (groupKey.ToUpperInvariant())
            {
                case "M":
                    if (On(sm.MStation)) menus.Add(new("mStation", "Index", "Station"));
                    if (On(sm.MClient)) menus.Add(new("mClient", "Index", "Client"));
                    if (On(sm.MProduct)) menus.Add(new("mProduct", "Index", "Product"));
                    if (On(sm.MTarget)) menus.Add(new("mTargetGroup", "Index", "Target Group"));
                    if (On(sm.MCampaign)) menus.Add(new("mCampaign", "Index", "Campaign"));
                    if (On(sm.MLanguage)) menus.Add(new("mLanguage", "Index", "Language"));
                    if (On(sm.MMaterial)) menus.Add(new("mMaterial", "Index", "Material"));
                    if (On(sm.MCurrency)) menus.Add(new("mCurrency", "Index", "Currency"));
                    if (On(sm.MMedia)) menus.Add(new("mMediaType", "Index", "Media Type"));
                    if (On(sm.MVendor)) menus.Add(new("mVendor", "Index", "Vendor"));
                    if (On(sm.MRateCode)) menus.Add(new("mRateCode", "Index", "Rate Code"));
                    if (On(sm.MProgramType)) menus.Add(new("mProgramType", "Index", "Program Type"));
                    if (On(sm.MSpotType)) menus.Add(new("mProgramSpotType", "Index", "Program SpotType"));
                    if (On(sm.MRateItem)) menus.Add(new("mRateItem", "Index", "Rate Item"));
                    if (On(sm.MBooking)) menus.Add(new("mBooking", "Index", "Booking"));
                    if (On(sm.MTax)) menus.Add(new("mTax", "Index", "Tax"));
                    if (On(sm.MDayPart)) menus.Add(new("mDayPart", "Index", "DayPart"));
                    break;

                case "T":
                    if (On(sm.TSchedule)) menus.Add(new("tDefineSchedule", "Index", "Define Schedule"));
                    if (On(sm.TNew)) menus.Add(new("tNewPlan", "Index", "New Plan"));
                    if (On(sm.TDelete)) menus.Add(new("tDeletePlan", "Index", "Delete Plan"));
                    if (On(sm.TDeleteItem)) menus.Add(new("tDeleteTransaction", "Index", "Delete Transaction"));
                    if (On(sm.TSelectOrder)) menus.Add(new("tSelectForOrder", "Index", "Select For Order"));
                    if (On(sm.TSelectInvoice)) menus.Add(new("tSelectForInvoice", "Index", "Select For Invoice"));
                    if (On(sm.TRefresh)) menus.Add(new("tRefreshData", "Index", "Refresh Data"));
                    break;

                case "R":
                    if (On(sm.RMasterReport)) menus.Add(new("rMasterFiles", "Index", "Master Files"));
                    if (On(sm.RPurchase)) menus.Add(new("rPurchaseOrder", "Index", "Purchase Order"));
                    if (On(sm.RAmendment)) menus.Add(new("rPurchaseAmendment", "Index", "Purchase Amendment"));
                    if (On(sm.RInvoice)) menus.Add(new("rInvoice", "Index", "Invoice"));
                    if (On(sm.RSchedule)) menus.Add(new("rScheduleReport", "Index", "Schedule Report"));
                    if (On(sm.RRevenue)) menus.Add(new("rRevenue", "Index", "Revenue Report"));
                    if (On(sm.RExpense)) menus.Add(new("rExpense", "Index", "Expense Report"));
                    if (On(sm.RJob)) menus.Add(new("rJob", "Index", "Job Report"));
                    if (On(sm.ROrder)) menus.Add(new("rOrder", "Index", "Order Report"));
                    if (On(sm.RBilling)) menus.Add(new("rBilling", "Index", "Billing Report"));
                    if (On(sm.RMargin)) menus.Add(new("rAgencyMargin", "Index", "Agency Margin Report"));
                    if (On(sm.RMonitor)) menus.Add(new("rMonitor", "Index", "Monitor Report"));
                    break;

                case "E":
                    if (On(sm.EPurchase)) menus.Add(new("ePurchase", "Index", "Purchase Order"));
                    if (On(sm.EInvoice)) menus.Add(new("eInvoice", "Index", "Invoice"));
                    if (On(sm.ERating)) menus.Add(new("eTVRating", "Index", "TV Rating"));
                    if (On(sm.EReach)) menus.Add(new("eReach", "Index", "Reach"));
                    break;

                case "S":
                    if (On(sm.SSetup)) menus.Add(new("sSetup", "Index", "Setup"));
                    if (On(sm.SRating)) menus.Add(new("sImportTvaRating", "Index", "Import TVA Rating"));
                    if (On(sm.SReach)) menus.Add(new("sImportTvaRatingArena", "Index", "Import TVA Rating - Arena"));
                    if (On(sm.SBackup)) menus.Add(new("sBackup", "Index", "Backup"));
                    break;
            }

            return menus;
        }
    }
}
