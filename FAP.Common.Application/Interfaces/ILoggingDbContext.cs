using FAP.Common.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAP.Common.Application.Interfaces
{
    public interface ILoggingDbContext
    {
        DbSet<AuditLog> AuditLogs { get; }
        public DbSet<PerformanceLog> PerformanceLogs { get; set; }
        Task<int> SaveChangesAsync(CancellationToken token);
    }
}
