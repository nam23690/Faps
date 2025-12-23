using FAP.Common.Application.Interfaces;
using FAP.Common.Domain.Academic.Terms;
using FAP.Common.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FAP.Common.Infrastructure.Repositories
{
    internal sealed class TermRepository : ITermRepository
    {
        private readonly UniversityDbContext _context;

        public Task AddAsync(Term term, CancellationToken token)
        {
            _context.Add(term); // ✅ CHUẨN
            return Task.CompletedTask;
        }

        public Task<Term?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return _context.Terms
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        }

        public Task UpdateAsync(Term term, CancellationToken cancellationToken)
        {
            _context.Update(term);
            return Task.CompletedTask;
        }
    }
}

}
