using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ams.Media.Web.Models;

namespace Ams.Media.Web.Services
{
    public class MenuGate : IMenuGate
    {
        // เมนูจำลองชั่วคราวเพื่อให้คอมไพล์ผ่าน (ภายหลังจะผูกสิทธิ์จาก DB)
        private static readonly Dictionary<string, List<MenuItemVm>> _menus = new()
        {
            ["Master"] = new()
            {
                new MenuItemVm { Key="MClient",      Text="Client",        Url="/Master/Client" },
                new MenuItemVm { Key="MCampaign",    Text="Campaign",      Url="/Master/Campaign" },
                new MenuItemVm { Key="MProduct",     Text="Product",       Url="/Master/Product" },
                new MenuItemVm { Key="MVendor",      Text="Vendor",        Url="/Master/Vendor" },
                new MenuItemVm { Key="MMedia",       Text="Media",         Url="/Master/Media" },
                new MenuItemVm { Key="MStation",     Text="Station",       Url="/Master/Station" },
                new MenuItemVm { Key="MTarget",      Text="Target Group",  Url="/Master/Target" },
                new MenuItemVm { Key="MLanguage",    Text="Language",      Url="/Master/Language" },
                new MenuItemVm { Key="MMaterial",    Text="Material",      Url="/Master/Material" },
                new MenuItemVm { Key="MCurrency",    Text="Currency",      Url="/Master/Currency" },
                new MenuItemVm { Key="MRateCode",    Text="Rate Code",     Url="/Master/RateCode" },
                new MenuItemVm { Key="MProgramType", Text="Program Type",  Url="/Master/ProgramType" },
                new MenuItemVm { Key="MRateItem",    Text="Rate Item",     Url="/Master/RateItem" },
                new MenuItemVm { Key="MBooking",     Text="Booking",       Url="/Master/Booking" },
                new MenuItemVm { Key="MTax",         Text="Tax",           Url="/Master/Tax" },
                new MenuItemVm { Key="MDayPart",     Text="DayPart",       Url="/Master/DayPart" },
            },
            ["Transaction"] = new()
            {
                new MenuItemVm { Key="TNew",            Text="New Plan",       Url="/Trans/New" },
                new MenuItemVm { Key="TDelete",         Text="Delete Plan",    Url="/Trans/Delete" },
                new MenuItemVm { Key="TBudget",         Text="Budget",         Url="/Trans/Budget" },
                new MenuItemVm { Key="TSelectOrder",    Text="Select Order",   Url="/Trans/SelectOrder" },
                new MenuItemVm { Key="TSelectInvoice",  Text="Select Invoice", Url="/Trans/SelectInvoice" },
                new MenuItemVm { Key="TRefresh",        Text="Refresh",        Url="/Trans/Refresh" },
            },
            ["Reports"] = new()
            {
                new MenuItemVm { Key="RMasterReport", Text="Master Report",  Url="/Reports/Master" },
                new MenuItemVm { Key="RPurchase",     Text="Purchase",       Url="/Reports/Purchase" },
                new MenuItemVm { Key="RAmendment",    Text="Amendment",      Url="/Reports/Amendment" },
                new MenuItemVm { Key="RInvoice",      Text="Invoice",        Url="/Reports/Invoice" },
                new MenuItemVm { Key="RRevenue",      Text="Revenue",        Url="/Reports/Revenue" },
                new MenuItemVm { Key="RExpense",      Text="Expense",        Url="/Reports/Expense" },
                new MenuItemVm { Key="RBilling",      Text="Billing",        Url="/Reports/Billing" },
                new MenuItemVm { Key="RMargin",       Text="Margin",         Url="/Reports/Margin" },
                new MenuItemVm { Key="RMonitor",      Text="Monitor",        Url="/Reports/Monitor" },
                new MenuItemVm { Key="ROrder",        Text="Order",          Url="/Reports/Order" },
                new MenuItemVm { Key="RJob",          Text="Job",            Url="/Reports/Job" },
                new MenuItemVm { Key="RSchedule",     Text="Schedule",       Url="/Reports/Schedule" },
                new MenuItemVm { Key="RPurchaseRpt",  Text="Purchase Rpt",   Url="/Reports/PurchaseRpt" },
            },
            ["Systems"] = new()
            {
                new MenuItemVm { Key="SSetup",   Text="Setup",   Url="/Systems/Setup" },
                new MenuItemVm { Key="SBackup",  Text="Backup",  Url="/Systems/Backup" },
                new MenuItemVm { Key="SRating",  Text="Rating",  Url="/Systems/Rating" },
            },
        };

        public Task<IReadOnlyList<MenuItemVm>> GetSubMenusAsync(string topKey)
        {
            if (string.IsNullOrWhiteSpace(topKey))
                return Task.FromResult<IReadOnlyList<MenuItemVm>>(Array.Empty<MenuItemVm>());

            if (_menus.TryGetValue(topKey, out var list))
                return Task.FromResult<IReadOnlyList<MenuItemVm>>(list);

            return Task.FromResult<IReadOnlyList<MenuItemVm>>(Array.Empty<MenuItemVm>());
        }
    }
}
