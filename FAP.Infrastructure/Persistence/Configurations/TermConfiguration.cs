using FAP.Common.Domain.Academic.Terms;
using FAP.Common.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FAP.Infrastructure.Persistence.Configurations
{
    public class TermConfiguration : IEntityTypeConfiguration<Term>
    {
        public void Configure(EntityTypeBuilder<Term> builder)
        {
            // If the database table is "Terms"
            builder.ToTable("Terms");
            builder.Property(t => t.Id)
                   .HasColumnName("TermID"); 

            builder.Property(t => t.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(t => t.Duration.StartDate)
                   .IsRequired();

            builder.Property(t => t.Duration.EndDate)
                   .IsRequired();

            builder.Property(t => t.IsDeleted)
                   .HasDefaultValue(false);
        }
    }
}
