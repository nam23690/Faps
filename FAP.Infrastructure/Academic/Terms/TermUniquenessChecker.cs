using FAP.Common.Domain.Academic.Terms;
using FAP.Common.Domain.Academic.Terms.Services;
using FAP.Common.Domain.Academic.Terms.ValueObjects;
using FAP.Common.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

internal sealed class TermUniquenessChecker : ITermUniquenessChecker
{
    private readonly UniversityDbContext _context;

    public TermUniquenessChecker(UniversityDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsOverlappingAsync(DateRange range)
    {
        return await _context.Set<Term>()
            .AnyAsync(t =>
                !t.IsDeleted &&
                t.Duration.Start < range.End &&
                t.Duration.End > range.Start
            );
    }
}
