using FAP.Common.Application.Interfaces;
using FAP.Common.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAP.Common.Infrastructure.Repositories
{
    public class PerformanceLogRepository : IPerformanceLogRepository
    {
        private readonly ILoggingDbContext _context;

        public PerformanceLogRepository(ILoggingDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PerformanceLog log, CancellationToken token)
        {
            _context.PerformanceLogs.Add(log);
            await _context.SaveChangesAsync(token);
        }
    }

}
