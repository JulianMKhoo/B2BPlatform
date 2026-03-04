using B2BPlatform.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace B2BPlatform.Infrastructure.Contexts;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<CompanyEntity> Companies { get; set; }
    public DbSet<CompanyOwnerEntity> CompanyOwners { get; set; }
    public DbSet<EmployeeEntity> Employees { get; set; }
    public DbSet<EmployeeDataEntity> EmployeeData { get; set; }
    public DbSet<BusinessUnitEntity> BusinessUnits { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmployeeDataEntity>()
            .HasIndex(e => new { e.EmployeeId, e.UnitId })
            .IsUnique();
    }
}
