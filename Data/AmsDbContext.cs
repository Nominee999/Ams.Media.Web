using Ams.Media.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Ams.Media.Web.Data
{
    public class AmsDbContext : DbContext
    {
        public AmsDbContext(DbContextOptions<AmsDbContext> options) : base(options) { }

        public DbSet<Client> Clients => Set<Client>();

        public DbSet<SecurityUser> SecurityUsers => Set<SecurityUser>();
        public DbSet<SecurityMenu> SecurityMenus => Set<SecurityMenu>();
        public DbSet<SecurityLog> SecurityLogs => Set<SecurityLog>();
        public DbSet<SecurityLogFile> SecurityLogFiles => Set<SecurityLogFile>();

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            base.OnModelCreating(modelBuilder);

            // -------- Client (คงเดิมของคุณ) --------
            modelBuilder.Entity<Client>(e =>
            {
                e.ToTable("Client");
                e.HasKey(x => x.ClientId);
                e.Property(x => x.ClientId).HasColumnName("ClientId");
                e.Property(x => x.ClieName).HasColumnName("ClieName");
                e.Property(x => x.CreditTerm).HasColumnName("CreditTerm");
                e.Property(x => x.AgencyCom).HasColumnName("AgencyCom");
                e.Property(x => x.ClientPrefix).HasColumnName("ClientPrefix");
                e.Property(x => x.ClientBranch).HasColumnName("ClientBranch");
                e.Property(x => x.ClientTaxNo).HasColumnName("ClientTaxNo");
                e.Property(x => x.BranchType).HasColumnName("BranchType");
                e.Property(x => x.ClientStatus).HasColumnName("ClientStatus");
            });

            // -------- security_user --------
            modelBuilder.Entity<SecurityUser>(e =>
            {
                e.ToTable("security_user");
                e.HasKey(x => x.Username);
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
                e.ToTable("Security_menu");
                e.HasKey(x => x.Username);
                e.Property(x => x.Username).HasColumnName("username");

                // Masterfiles
                e.Property(x => x.Mcurrency).HasColumnName("mcurrency");
                e.Property(x => x.Mtarget).HasColumnName("mtarget");
                e.Property(x => x.Mlanguage).HasColumnName("mlanguage");
                e.Property(x => x.Mstation).HasColumnName("mstation");
                e.Property(x => x.Mloading).HasColumnName("mloading");
                e.Property(x => x.Mmedia).HasColumnName("mmedia");
                e.Property(x => x.Mclient).HasColumnName("mclient");
                e.Property(x => x.Mcampaign).HasColumnName("mcampaign");
                e.Property(x => x.Mproduct).HasColumnName("mproduct");
                e.Property(x => x.Mvendor).HasColumnName("mvendor");
                e.Property(x => x.Mratecode).HasColumnName("mratecode");
                e.Property(x => x.Mprogramtype).HasColumnName("mprogramtype");
                e.Property(x => x.Mrateitem).HasColumnName("mrateitem");
                e.Property(x => x.Mmaterial).HasColumnName("mmaterial");
                e.Property(x => x.Mtax).HasColumnName("mtax");
                e.Property(x => x.Mbooking).HasColumnName("mbooking");
                e.Property(x => x.Mpackage).HasColumnName("mpackage");
                e.Property(x => x.Mdaypart).HasColumnName("mdaypart");
                e.Property(x => x.Mcirculation).HasColumnName("mcirculation");
                e.Property(x => x.Mspottype).HasColumnName("mspottype");
                e.Property(x => x.Mprogrammaster).HasColumnName("mprogrammaster");

                // Transactions
                e.Property(x => x.Tnew).HasColumnName("tnew");
                e.Property(x => x.Tdelete).HasColumnName("tdelete");
                e.Property(x => x.Tbudget).HasColumnName("tbudget");
                e.Property(x => x.Tselectinvoice).HasColumnName("tselectinvoice");
                e.Property(x => x.Tselectorder).HasColumnName("tselectorder");
                e.Property(x => x.Trefresh).HasColumnName("trefresh");
                e.Property(x => x.Tdeleteitem).HasColumnName("tdeleteitem");
                e.Property(x => x.Tjob).HasColumnName("tjob");
                e.Property(x => x.Tschedule).HasColumnName("tschedule");

                // Reports
                e.Property(x => x.Rmasterreport).HasColumnName("rmasterreport");
                e.Property(x => x.Rpurchase).HasColumnName("rpurchase");
                e.Property(x => x.Ramendment).HasColumnName("ramendment");
                e.Property(x => x.Rreports).HasColumnName("rreports");
                e.Property(x => x.Rinvoice).HasColumnName("rinvoice");
                e.Property(x => x.Rrevenue).HasColumnName("rrevenue");
                e.Property(x => x.Rexpense).HasColumnName("rexpense");
                e.Property(x => x.Rbilling).HasColumnName("rbilling");
                e.Property(x => x.Rmargin).HasColumnName("rmargin");
                e.Property(x => x.Rchecklist).HasColumnName("rchecklist");
                e.Property(x => x.Rcheckreport).HasColumnName("rcheckreport");
                e.Property(x => x.Rmonitor).HasColumnName("rmonitor");
                e.Property(x => x.Rorder).HasColumnName("rorder");
                e.Property(x => x.Rjob).HasColumnName("rjob");
                e.Property(x => x.Rschedule).HasColumnName("rschedule");

                // Enquiry
                e.Property(x => x.Epurchase).HasColumnName("epurchase");
                e.Property(x => x.Einvoice).HasColumnName("einvoice");
                e.Property(x => x.Erating).HasColumnName("erating");
                e.Property(x => x.Ereach).HasColumnName("ereach");

                // Systems & misc.
                e.Property(x => x.Ssetup).HasColumnName("ssetup");
                e.Property(x => x.Sbackup).HasColumnName("sbackup");
                e.Property(x => x.Srating).HasColumnName("srating");
                e.Property(x => x.Sreach).HasColumnName("sreach");
                e.Property(x => x.Saddins).HasColumnName("saddins");
                e.Property(x => x.Saddins1).HasColumnName("saddins1");
                e.Property(x => x.Saddins2).HasColumnName("saddins2");
                e.Property(x => x.Saddins3).HasColumnName("saddins3");
                e.Property(x => x.Saddins4).HasColumnName("saddins4");
                e.Property(x => x.Saddins5).HasColumnName("saddins5");
                e.Property(x => x.Saddins6).HasColumnName("saddins6");
                e.Property(x => x.Saddins7).HasColumnName("saddins7");
                e.Property(x => x.Saddins8).HasColumnName("saddins8");
                e.Property(x => x.Saddins9).HasColumnName("saddins9");
                e.Property(x => x.Saddins10).HasColumnName("saddins10");

                e.Property(x => x.Exdtl).HasColumnName("exdtl");
                e.Property(x => x.Extv3).HasColumnName("extv3");
                e.Property(x => x.Extv7).HasColumnName("extv7");
                e.Property(x => x.Pimport).HasColumnName("pimport");
                e.Property(x => x.Pmapping).HasColumnName("pmapping");
                e.Property(x => x.Pmlist).HasColumnName("pmlist");
                e.Property(x => x.Pmreport).HasColumnName("pmreport");
                e.Property(x => x.Pearianna).HasColumnName("pearianna");
                e.Property(x => x.Pematched).HasColumnName("pematched");
                e.Property(x => x.Pereport).HasColumnName("pereport");
                e.Property(x => x.Pereach).HasColumnName("pereach");
            });


            // -------- Security_Log --------
            modelBuilder.Entity<SecurityLog>(e =>
            {
                e.ToTable("Security_Log");
                e.HasKey(x => x.Username);
                e.Property(x => x.Username).HasColumnName("username");
                e.Property(x => x.UserDateTime).HasColumnName("userdatetime");
                e.Property(x => x.ComputerName).HasColumnName("computername");
                e.Property(x => x.Processing).HasColumnName("processing");
            });

            // -------- SECURITY_LOGFILE (คงรูปแบบเดิม) --------
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
