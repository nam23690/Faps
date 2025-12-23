using FAP.Common.Infrastructure.EntitiesMaster;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FAP.Common.Infrastructure.Persistence
{
    public class MasterDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public MasterDbContext(DbContextOptions<MasterDbContext> options)
       : base(options)
        {
        }

        #region DbSets
        public DbSet<Campus> Campuses { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<AppPermission> AppPermissions { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Đổi tên các bảng Identity từ AspNet* sang App*
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("AppUsers");
            });

            builder.Entity<ApplicationRole>(entity =>
            {
                entity.ToTable("AppRoles");
            });

            builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("AppUserRoles");
                // Thêm cột CampusCode vào bảng AppUserRoles
                entity.Property<string>("CampusCode")
                    .HasMaxLength(50);
            });

            builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("AppUserClaims");
                // Thêm cột CampusCode vào bảng AppUserClaims
                entity.Property<string>("CampusCode")
                    .HasMaxLength(50);
            });

            builder.Entity<Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("AppRoleClaims");
            });

            builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("AppUserLogins");
            });

            builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("AppUserTokens");
            });

            // Configure RefreshToken entity
            builder.Entity<RefreshToken>(entity =>
            {
                entity.ToTable("RefreshTokens");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Token).IsUnique();
                entity.HasIndex(e => e.UserId);
                
                entity.Property(e => e.Token)
                    .IsRequired()
                    .HasMaxLength(500);
                
                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);
                
                entity.Property(e => e.DeviceInfo)
                    .HasMaxLength(500);
                
                entity.Property(e => e.IpAddress)
                    .HasMaxLength(50);
                
                // Foreign key relationship
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Apply all configurations from Configurations folder
            builder.ApplyConfigurationsFromAssembly(typeof(MasterDbContext).Assembly);
        }
    }
}
