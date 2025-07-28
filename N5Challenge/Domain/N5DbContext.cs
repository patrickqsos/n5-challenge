using Microsoft.EntityFrameworkCore;

namespace N5Challenge.Domain;

public class N5DbContext : DbContext
{
    public N5DbContext(DbContextOptions<N5DbContext> options) : base(options) { }

    public DbSet<Permission> Permission => Set<Permission>();
    public DbSet<PermissionType> PermissionType => Set<PermissionType>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ----------------------- PermissionType -----------------------
        builder.Entity<PermissionType>(cfg =>
        {
            cfg.ToTable("PermissionTypes");
            cfg.HasKey(pt => pt.Id);
            cfg.Property(pt => pt.Description)
                .IsRequired()
                .HasMaxLength(100);
            cfg.HasIndex(pt => pt.Description).IsUnique();
        });

        // ------------------------- Permission -------------------------
        builder.Entity<Permission>(cfg =>
        {
            cfg.ToTable("Permissions");
            cfg.HasKey(p => p.Id);

            cfg.Property(p => p.EmployeeForename)
                .IsRequired()
                .HasMaxLength(150);

            cfg.Property(p => p.EmployeeSurname)
                .IsRequired()
                .HasMaxLength(150);
            
            cfg.Property(p => p.PermissionDate)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            cfg.HasOne(p => p.PermissionTypeNavigation)
                .WithMany()
                .HasForeignKey(p => p.PermissionType)
                .OnDelete(DeleteBehavior.Restrict);

            cfg.HasIndex(p => new { PermissionTypeId = p.PermissionType, p.PermissionDate });
        });
    }
}