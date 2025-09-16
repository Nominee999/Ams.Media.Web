using Ams.Media.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Ams.Media.Web.Data
{
    public class AmsDbContext : DbContext
    {
        public AmsDbContext(DbContextOptions<AmsDbContext> options) : base(options) { }

        // DbSets
         

        public DbSet<SecurityUser> SecurityUsers => Set<SecurityUser>();
        public DbSet<SecurityMenu> SecurityMenus => Set<SecurityMenu>();
        public DbSet<SecurityLog> SecurityLogs => Set<SecurityLog>();

        public DbSet<SecurityLogFile> SecurityLogFiles => Set<SecurityLogFile>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);



            // security_user
            modelBuilder.Entity<SecurityUser>(e =>
            {
                e.ToTable("security_user");
                e.HasKey(x => x.Username);                    // ถ้าไม่มี PK ให้ใช้ e.HasNoKey();
                e.Property(x => x.Username).HasColumnName("username");
                e.Property(x => x.Password).HasColumnName("password");
                e.Property(x => x.Department).HasColumnName("department");

                e.Property(x => x.Masterfiles).HasColumnName("masterfiles");
                e.Property(x => x.Transactions).HasColumnName("transactions");
                e.Property(x => x.Reports).HasColumnName("reports");
                e.Property(x => x.Enquirys).HasColumnName("enquirys");
                e.Property(x => x.Systems).HasColumnName("systems");
                e.Property(x => x.Addinss).HasColumnName("addinss");
                e.Property(x => x.Exports).HasColumnName("exports");
                e.Property(x => x.Approved).HasColumnName("approved");

                e.Property(x => x.Addnew).HasColumnName("addnew");
                e.Property(x => x.Modify).HasColumnName("modify");
                e.Property(x => x.Deleted).HasColumnName("deleted");
                e.Property(x => x.Change).HasColumnName("change");
                e.Property(x => x.Levels).HasColumnName("levels");
                e.Property(x => x.Utility).HasColumnName("utility");
                e.Property(x => x.Report).HasColumnName("report");
                e.Property(x => x.Finance).HasColumnName("finance");
                e.Property(x => x.Postcheck).HasColumnName("postcheck");
                e.Property(x => x.Sections).HasColumnName("sections");
            });

            // security_menu
            modelBuilder.Entity<SecurityMenu>(e =>
            {
                e.ToTable("security_menu");
                // ถ้าไม่มีคีย์ในตาราง ให้ใช้ HasNoKey()
                // e.HasNoKey();

                e.Property(x => x.MCurrency).HasColumnName("mcurrency");
                e.Property(x => x.MClient).HasColumnName("mclient");
                e.Property(x => x.MCampaign).HasColumnName("mcampaign");
                e.Property(x => x.MProduct).HasColumnName("mproduct");
                e.Property(x => x.MStation).HasColumnName("mstation");
                e.Property(x => x.MTargetGroup).HasColumnName("mtargetgroup");
                e.Property(x => x.MLanguage).HasColumnName("mlanguage");
                e.Property(x => x.MMaterial).HasColumnName("mmaterial");
                e.Property(x => x.MMediaType).HasColumnName("mmediatype");
                e.Property(x => x.MVendor).HasColumnName("mvendor");
                e.Property(x => x.MRateCode).HasColumnName("mratecode");
                e.Property(x => x.MProgramType).HasColumnName("mprogramtype");
                e.Property(x => x.MRateItem).HasColumnName("mrateitem");
                e.Property(x => x.MBooking).HasColumnName("mbooking");
                e.Property(x => x.MTax).HasColumnName("mtax");
                e.Property(x => x.MDayPart).HasColumnName("mdaypart");

                e.Property(x => x.TNew).HasColumnName("tnew");
                e.Property(x => x.TDelete).HasColumnName("tdelete");
                e.Property(x => x.TBudget).HasColumnName("tbudget");
                e.Property(x => x.TSelectOrder).HasColumnName("tselectorder");
                e.Property(x => x.TSelectInvoice).HasColumnName("tselectinvoice");
                e.Property(x => x.TRefresh).HasColumnName("trefresh");

                e.Property(x => x.RMaster).HasColumnName("rmaster");
                e.Property(x => x.RPurchase).HasColumnName("rpurchase");
                e.Property(x => x.RAmendment).HasColumnName("ramendment");
                e.Property(x => x.RInvoice).HasColumnName("rinvoice");
                e.Property(x => x.RScheduleFlow).HasColumnName("rscheduleflow");
                e.Property(x => x.RRevenue).HasColumnName("rrevenue");
                e.Property(x => x.RExpense).HasColumnName("rexpense");
                e.Property(x => x.RJob).HasColumnName("rjob");
                e.Property(x => x.RSchedule).HasColumnName("rschedule");
                e.Property(x => x.RPurchaseRpt).HasColumnName("rpurchasereport");
                e.Property(x => x.RBilling).HasColumnName("rbilling");
                e.Property(x => x.RAgencyMargin).HasColumnName("rmargin");
                e.Property(x => x.RMonitor).HasColumnName("rmonitor");

                e.Property(x => x.EPurchase).HasColumnName("epurchase");
                e.Property(x => x.EInvoice).HasColumnName("einvoice");
                e.Property(x => x.ETVRating).HasColumnName("etvrating");
                e.Property(x => x.EReach).HasColumnName("ereach");

                e.Property(x => x.SSetup).HasColumnName("ssetup");
                e.Property(x => x.SImportTVA).HasColumnName("simporttvarating");
                e.Property(x => x.SImportArena).HasColumnName("simporttvarating_arena");
                e.Property(x => x.SBackup).HasColumnName("sbackup");

            });

            // Security_log
            modelBuilder.Entity<SecurityLog>(e =>
            {
                e.ToTable("Security_log");
                e.HasNoKey(); // ตาราง Log มักไม่มี PK
                e.Property(x => x.Username).HasColumnName("username");
                e.Property(x => x.UserDateTime).HasColumnName("userdatetime");
                e.Property(x => x.ComputerName).HasColumnName("computername");
                e.Property(x => x.Processing).HasColumnName("processing");
            });

            // SECURITY_LOGFILE
            modelBuilder.Entity<SecurityLogFile>(e =>
            {
                e.ToTable("SECURITY_LOGFILE");
                e.HasNoKey();
                e.Property(x => x.ProgramName).HasColumnName("programname");
                e.Property(x => x.Username).HasColumnName("username");
                e.Property(x => x.ComputerName).HasColumnName("computername");
                e.Property(x => x.Menu).HasColumnName("menu");
                e.Property(x => x.AuditType).HasColumnName("audittype");
                e.Property(x => x.AuditDate).HasColumnName("auditdate");

                e.Property(x => x.M_ScheduleNo).HasColumnName("m_scheduleno");
                e.Property(x => x.M_OrderNo).HasColumnName("m_orderno");
                e.Property(x => x.M_InvoiceNo).HasColumnName("m_invoiceno");
                e.Property(x => x.P_JobNo).HasColumnName("p_jobno");
                e.Property(x => x.P_InvoiceNo).HasColumnName("p_invoiceno");
                e.Property(x => x.Fi_JournalNo).HasColumnName("fi_journalno");
                e.Property(x => x.Fi_PrevUser).HasColumnName("fi_prevuser");
                e.Property(x => x.Fi_PrevDate).HasColumnName("fi_prevdate");
                e.Property(x => x.Item_Id).HasColumnName("item_id");
            });
        }
    }
}
