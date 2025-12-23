
using FAP.Common.Infrastructure.EntitiesMaster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FAP.Common.Infrastructure.Persistence.Configurations
{
    public class AppPermissionConfiguration : IEntityTypeConfiguration<AppPermission>
    {
        public void Configure(EntityTypeBuilder<AppPermission> builder)
        {
            builder.ToTable("AppPermissions");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Module)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Code)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Description)
                .HasMaxLength(500);

            builder.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Indexes
            builder.HasIndex(e => new { e.Module, e.Code })
                .IsUnique()
                .HasDatabaseName("IX_AppPermissions_Module_Code");

            builder.HasIndex(e => e.Module)
                .HasDatabaseName("IX_AppPermissions_Module");

            builder.HasIndex(e => e.Code)
                .HasDatabaseName("IX_AppPermissions_Code");
        }
    }
}

