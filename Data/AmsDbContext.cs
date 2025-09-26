using Microsoft.EntityFrameworkCore;
using Ams.Media.Web.Models;

namespace Ams.Media.Web.Data
{
    public class AmsDbContext : DbContext
    {
        

        public DbSet<SecurityMenu> SecurityMenus => Set<SecurityMenu>();
        public DbSet<SecurityLog> SecurityLogs => Set<SecurityLog>();
        public DbSet<SecurityUser> SecurityUsers => Set<SecurityUser>();
        // ctor สำหรับใช้ผ่าน DI (Program.cs -> AddDbContext)
        public AmsDbContext(DbContextOptions<AmsDbContext> options) : base(options) { }

        // ctor สำหรับ HybridDbScope (เราส่ง SqlConnection เข้ามา)
        public AmsDbContext(DbContextOptions options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Security_menu : PK = username
            modelBuilder.Entity<SecurityMenu>(e =>
            {
                e.HasKey(x => x.Username);
                // ที่เหลือใช้ [Column] บนโมเดล
            });

            // Security_Log : ใช้ [Key] ในโมเดลแล้ว
            modelBuilder.Entity<SecurityLog>(e =>
            {
                e.ToTable("Security_Log");
            });

            // security_user : บางดาต้าเบสไม่มี PK → กำหนด Keyless
            modelBuilder.Entity<SecurityUser>(e =>
            {
                e.ToTable("security_user");
                e.HasNoKey();
                // มี [Column] กำกับในโมเดลแล้ว
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
