// Data/AmsDbContext.cs
using Microsoft.EntityFrameworkCore;

namespace Ams.Media.Web.Data
{
    public sealed class AmsDbContext : DbContext
    {
        public AmsDbContext(DbContextOptions<AmsDbContext> options) : base(options)
        {
        }

        // หมายเหตุ: โปรเจ็กต์นี้ใช้ Dapper เป็นหลัก
        // DbSet ด้านล่างประกาศเฉพาะที่จำเป็นต่อ EF เท่านั้น (ถ้ามี)
        // public DbSet<SomeEntity> SomeEntities { get; set; } = null!;
    }
}
