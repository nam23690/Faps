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

    public async Task<bool> IsOverlappingAsync(
    DateRange duration,
    Guid? excludeTermId = null)
    {
        return await _context.Terms.AnyAsync(x =>
            !x.IsDeleted &&
            (excludeTermId == null || x.Id != excludeTermId) &&
            x.Duration.StartDate < duration.EndDate &&
            duration.StartDate < x.Duration.EndDate
        );
    }

}
