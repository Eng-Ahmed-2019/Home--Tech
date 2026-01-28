using HomeTech.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using HomeTech.Application.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace HomeTech.Infrastructure.Data
{
    public class HomeTechDbContext
        : IdentityDbContext<ApplicationUser>, IApplicationDbContext
    {
        public HomeTechDbContext(DbContextOptions<HomeTechDbContext> options)
            : base(options)
        { }

        public DbSet<UserSession> UserSessions => Set<UserSession>();
        public DbSet<BannedIp> BannedIps => Set<BannedIp>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<Location> Locations => Set<Location>();
        public DbSet<Category> Categories => Set<Category>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Location>(e =>
            {
                e.HasKey(x => x.Id);
                e.HasIndex(x => x.Name).IsUnique();
                e.HasIndex(x => new { x.ParentId, x.IsActive });
                e.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(100);
                e.HasOne(x => x.Parent)
                    .WithMany(x => x.SubLocations)
                    .HasForeignKey(x => x.ParentId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasQueryFilter(x => !x.IsDeleted);
            });

            builder.Entity<Category>(e =>
            {
                e.HasKey(x => x.Id);
                e.HasIndex(x => x.Name).IsUnique();
                e.HasIndex(x => x.IsActive);
                e.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);
                e.Property(x => x.RowVersion)
                .IsRowVersion();
                e.HasQueryFilter(x => !x.IsDeleted);
            });

            builder.Entity<AuditLog>()
                .Property(x => x.Hash)
                .IsRequired();

            builder.Entity<AuditLog>()
                .HasIndex(x => x.CreatedAt);

            builder.Entity<AuditLog>()
                .Property(x => x.Hash)
                .HasMaxLength(64);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken ct)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                    entry.Entity.MarkCreated();

                if (entry.State == EntityState.Modified)
                    entry.Entity.MarkUpdated();
            }

            return await base.SaveChangesAsync(ct);
        }
    }
}