using FAP.Common.Application.Interfaces;
using FAP.Common.Domain.Base;
using FAP.Common.Domain.Entities;
using FAP.Common.Domain.Events;
using FAP.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAP.Common.Infrastructure.Persistence
{
    public class LoggingDbContext:DbContext, ILoggingDbContext
    {
        public LoggingDbContext(DbContextOptions<LoggingDbContext> options)
            : base(options)
        {
        }

        #region DbSets
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<PerformanceLog> PerformanceLogs { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ❗ BẮT BUỘC: EF KHÔNG ĐƯỢC MAP DOMAIN EVENT
            modelBuilder.Ignore<IDomainEvent>();
            modelBuilder.Ignore<DomainEvent>(); // nếu có class concrete
            base.OnModelCreating(modelBuilder);

            // Apply all configurations in the current assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UniversityDbContext).Assembly);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Audit, soft delete, domain events, etc.
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
