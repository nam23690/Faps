using FAP.Common.Application.Interfaces;
using FAP.Common.Domain.Entities;
using FAP.Common.Infrastructure.Persistence;

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
    }

}
