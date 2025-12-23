using FAP.Common.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAP.Common.Application.Interfaces
{
    public interface IPerformanceLogRepository
    {
        Task AddAsync(PerformanceLog log, CancellationToken token);
    }

}
