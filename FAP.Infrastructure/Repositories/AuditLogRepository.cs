using FAP.Common.Application.Interfaces;
using FAP.Common.Domain.Entities;
using FAP.Common.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAP.Common.Infrastructure.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly ILoggingDbContext _context;

        public AuditLogRepository(ILoggingDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(AuditLog log, CancellationToken cancellationToken = default)
        {
            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

}
