using FAP.Common.Domain.Academic.Terms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FAP.Infrastructure.Persistence.Configurations
{
    public class TermConfiguration : IEntityTypeConfiguration<Term>
    {
        public void Configure(EntityTypeBuilder<Term> builder)
        {
            builder.ToTable("Terms");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
                   .HasColumnName("TermID");

            // -----------------------------
            // TermName (ValueObject)
            // -----------------------------
            builder.OwnsOne(t => t.Name, name =>
            {
                name.Property(n => n.Value)
                    .HasColumnName("Name")
                    .HasMaxLength(100)
                    .IsRequired();
            });

            // -----------------------------
            // DateRange (ValueObject)
            // -----------------------------
            builder.OwnsOne(t => t.Duration, duration =>
            {
                duration.Property(d => d.StartDate)
                        .HasColumnName("StartDate")
                        .IsRequired();

                duration.Property(d => d.EndDate)
                        .HasColumnName("EndDate")
                        .IsRequired();
            });

            // -----------------------------
            // Soft delete
            // -----------------------------
            builder.Property(t => t.IsDeleted)
                   .HasDefaultValue(false);
        }
    }
}
