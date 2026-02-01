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
        public DbSet<HomeTechService> HomeTechServices => Set<HomeTechService>();

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

            builder.Entity<AuditLog>(e =>
            {
                e.Property(x => x.Hash)
                    .IsRequired()
                    .HasMaxLength(64);

                e.HasIndex(x => x.CreatedAt);
            });

            builder.Entity<HomeTechService>(e =>
            {
                e.HasKey(x => x.Id);

                e.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

                e.HasIndex(x => x.Name).IsUnique();
                e.HasIndex(x => x.IsActive);

                e.HasQueryFilter(x => !x.IsDeleted);
            });
        }

        public override async Task<int> SaveChangesAsync(CancellationToken ct)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.MarkCreated();
                        break;

                    case EntityState.Modified:
                        entry.Entity.MarkUpdated();
                        break;

                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.Entity.SoftDelete();
                        break;
                }
            }

            return await base.SaveChangesAsync(ct);
        }
    }
}