using Microsoft.EntityFrameworkCore;
using FAP.Common.Domain.Academic.Terms;

namespace FAP.Common.Application.Interfaces
{
    public interface IUniversityDbContext
    {
        DbSet<Term> Terms { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
