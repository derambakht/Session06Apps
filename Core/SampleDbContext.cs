using Core.Entities.Base;
using Core.Entities.Security;
using Microsoft.EntityFrameworkCore;

namespace Core;
public class SampleDbContext : DbContext
{
    public DbSet<Company> Companies { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<UserPermission> UserPermission { get; set; }

    public SampleDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // modelBuilder.Entity<User>()
        // .Property(q => q.FirstName).IsRequired().HasMaxLength(64);

        modelBuilder.Entity<UserRefreshToken>() .ToTable(
        "UserRefreshTokenes",
        b => b.IsTemporal());
            // b =>
            // {
            //     b.HasPeriodStart("ValidFrom");
            //     b.HasPeriodEnd("ValidTo");
            //     b.UseHistoryTable("EmployeeHistoricalData");
            // }));

    }

}