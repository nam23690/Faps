using FAP.Common.Infrastructure.EntitiesMaster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FAP.Common.Infrastructure.Persistence.Configurations
{
    public class CampusConfiguration : IEntityTypeConfiguration<Campus>
    {
        public void Configure(EntityTypeBuilder<Campus> builder)
        {
            builder.ToTable("Campuses");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.Description)
                .HasMaxLength(1000);

            builder.Property(e => e.Address)
                .HasMaxLength(500);

            builder.Property(e => e.Phone)
                .HasMaxLength(50);

            builder.Property(e => e.Email)
                .HasMaxLength(100);

            builder.Property(e => e.ConnectionString)
                .HasMaxLength(500);

            // Indexes
            builder.HasIndex(e => e.Code)
                .IsUnique()
                .HasDatabaseName("IX_AppCampuses_Code");

            builder.HasIndex(e => e.Name)
                .HasDatabaseName("IX_AppCampuses_Name");
        }
    }
}

