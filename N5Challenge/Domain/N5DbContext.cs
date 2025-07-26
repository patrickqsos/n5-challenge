using Microsoft.EntityFrameworkCore;

namespace N5Challenge.Domain;

public class N5DbContext : DbContext
{
    public N5DbContext(DbContextOptions<N5DbContext> options) : base(options) { }

    public DbSet<Permissions> Permissions => Set<Permissions>();
    public DbSet<PermissionTypes> PermissionTypes => Set<PermissionTypes>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ----------------------- PermissionType -----------------------
        builder.Entity<PermissionTypes>(cfg =>
        {
            cfg.ToTable("PermissionTypes");
            cfg.HasKey(pt => pt.Id);
            cfg.Property(pt => pt.Description)
                .IsRequired()
                .HasMaxLength(100);
            cfg.HasIndex(pt => pt.Description).IsUnique();
        });

        // ------------------------- Permission -------------------------
        builder.Entity<Permissions>(cfg =>
        {
            cfg.ToTable("Permissions");
            cfg.HasKey(p => p.Id);

            cfg.Property(p => p.EmployeeName)
                .IsRequired()
                .HasMaxLength(150);

            cfg.Property(p => p.PermissionDate)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            cfg.HasOne(p => p.PermissionTypes)
                .WithMany()
                .HasForeignKey(p => p.PermissionTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            cfg.HasIndex(p => new { p.PermissionTypeId, p.PermissionDate });
        });
    }
}