using FAP.Common.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FAP.Infrastructure.Persistence.Configurations
{
    public class TermConfiguration : IEntityTypeConfiguration<Term>
    {
        public void Configure(EntityTypeBuilder<Term> builder)
        {
            // If the database table is "Term"
            builder.ToTable("Term");

            // Mapping Id from BaseEntity to TermID column if we want to keep legacy DB schema
            // Or just map to Id. For now, let's assume we map to "TermID" column but as int (or short conversion).
            // However, BaseEntity.Id is int. If DB is short, this might fail at runtime.
            // Safe bet: Map to "TermID" column.
            builder.Property(t => t.Id)
                   .HasColumnName("TermID"); 

            builder.Property(t => t.SemesterName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(t => t.StartDate)
                   .IsRequired();

            builder.Property(t => t.EndDate)
                   .IsRequired();

            builder.Property(t => t.IsClosed)
                   .HasDefaultValue(false);
            
            // Configure relationship if needed
            // builder.HasOne(t => t.Campus)...
        }
    }
}
