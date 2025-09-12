using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Ams.Media.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace Ams.Media.Web.Services
{
    public class MenuGate : IMenuGate
    {
        private readonly AmsDbContext _db;
        public MenuGate(AmsDbContext db) { _db = db; }

        public async Task<Dictionary<string, IReadOnlyList<MenuItemVm>>> GetAllAsync(ClaimsPrincipal user)
        {
            var uname = user?.Identity?.Name ?? string.Empty;
            var menu = await _db.SecurityMenus.AsNoTracking()
                          .FirstOrDefaultAsync(x => x.Username == uname);

            var dict = new Dictionary<string, IReadOnlyList<MenuItemVm>>
            {
                ["M"] = new List<MenuItemVm>(),
                ["T"] = new List<MenuItemVm>(),
                ["R"] = new List<MenuItemVm>(),
                ["E"] = new List<MenuItemVm>(),
                ["S"] = new List<MenuItemVm>()
            };

            if (menu == null) return dict;

            // helper
            static bool On(string? v) => v == "1";

            // --- Masterfiles (ตัวอย่าง mapping controller/action สามารถเปลี่ยนได้)
            var M = new List<MenuItemVm>();
            if (On(menu.Mstation)) M.Add(new("Station", "Masterfiles", "Station"));
            if (On(menu.Mclient)) M.Add(new("Client", "Masterfiles", "Client"));
            if (On(menu.Mcampaign)) M.Add(new("Campaign", "Masterfiles", "Campaign"));
            if (On(menu.Mproduct)) M.Add(new("Product", "Masterfiles", "Product"));
            if (On(menu.Mtarget)) M.Add(new("Target Group", "Masterfiles", "TargetGroup"));
            if (On(menu.Mlanguage)) M.Add(new("Language", "Masterfiles", "Language"));
            if (On(menu.Mmaterial)) M.Add(new("Material", "Masterfiles", "Material"));
            if (On(menu.Mcurrency)) M.Add(new("Currency", "Masterfiles", "Currency"));
            if (On(menu.Mmedia)) M.Add(new("Media Type", "Masterfiles", "MediaType"));
            if (On(menu.Mvendor)) M.Add(new("Vendor", "Masterfiles", "Vendor"));
            if (On(menu.Mratecode)) M.Add(new("Rate Code", "Masterfiles", "RateCode"));
            if (On(menu.Mprogramtype)) M.Add(new("Program Type", "Masterfiles", "ProgramType"));
            if (On(menu.Mrateitem)) M.Add(new("Rate Item", "Masterfiles", "RateItem"));
            if (On(menu.Mbooking)) M.Add(new("Booking", "Masterfiles", "Booking"));
            if (On(menu.Mtax)) M.Add(new("Tax", "Masterfiles", "Tax"));
            if (On(menu.Mdaypart)) M.Add(new("DayPart", "Masterfiles", "DayPart"));
            dict["M"] = M;

            // --- Transactions
            var T = new List<MenuItemVm>();
            if (On(menu.Tnew)) T.Add(new("New Plan", "Trans", "NewPlan"));
            if (On(menu.Tdelete)) T.Add(new("Delete Trans", "Trans", "DeleteTrans"));
            if (On(menu.Tbudget)) T.Add(new("Budget", "Trans", "Budget"));
            if (On(menu.Tselectorder)) T.Add(new("Select Order", "Trans", "SelectOrder"));
            if (On(menu.Tselectinvoice)) T.Add(new("Select Invoice", "Trans", "SelectInvoice"));
            if (On(menu.Trefresh)) T.Add(new("Refresh Data", "Trans", "Refresh"));
            if (On(menu.Tjob)) T.Add(new("Job", "Trans", "Job"));
            if (On(menu.Tschedule)) T.Add(new("Schedule", "Trans", "Schedule"));
            dict["T"] = T;

            // --- Reports
            var R = new List<MenuItemVm>();
            if (On(menu.Rmasterreport)) R.Add(new("Master Files", "Reports", "MasterFiles"));
            if (On(menu.Rpurchase)) R.Add(new("Purchase Order", "Reports", "PurchaseOrder"));
            if (On(menu.Ramendment)) R.Add(new("Purchase Amendment", "Reports", "PurchaseAmendment"));
            if (On(menu.Rinvoice)) R.Add(new("Invoice", "Reports", "Invoice"));
            if (On(menu.Rrevenue)) R.Add(new("Revenue", "Reports", "Revenue"));
            if (On(menu.Rexpense)) R.Add(new("Expense", "Reports", "Expense"));
            if (On(menu.Rbilling)) R.Add(new("Billing", "Reports", "Billing"));
            if (On(menu.Rmargin)) R.Add(new("Agency Margin", "Reports", "AgencyMargin"));
            if (On(menu.Rmonitor)) R.Add(new("Monitor", "Reports", "Monitor"));
            if (On(menu.Rorder)) R.Add(new("Order", "Reports", "Order"));
            if (On(menu.Rjob)) R.Add(new("Job", "Reports", "Job"));
            if (On(menu.Rschedule)) R.Add(new("Schedule", "Reports", "Schedule"));
            dict["R"] = R;

            // --- Enquiry
            var E = new List<MenuItemVm>();
            if (On(menu.Epurchase)) E.Add(new("Purchase Order", "Enquiry", "PurchaseOrder"));
            if (On(menu.Einvoice)) E.Add(new("Invoice", "Enquiry", "Invoice"));
            if (On(menu.Erating)) E.Add(new("TV Rating", "Enquiry", "TVRating"));
            if (On(menu.Ereach)) E.Add(new("Reach", "Enquiry", "Reach"));
            dict["E"] = E;

            // --- Systems
            var S = new List<MenuItemVm>();
            if (On(menu.Ssetup)) S.Add(new("Setup", "Systems", "Setup"));
            if (On(menu.Sbackup)) S.Add(new("Backup", "Systems", "Backup"));
            if (On(menu.Srating)) S.Add(new("Import TVA Rating", "Systems", "ImportTVA"));
            if (On(menu.Sreach)) S.Add(new("Import Reach", "Systems", "ImportReach"));
            dict["S"] = S;

            return dict;
        }
    }
}
