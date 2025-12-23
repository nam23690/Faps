using FAP.Common.Domain.Academic.Terms.ValueObjects;

namespace FAP.Common.Domain.Academic.Terms.Services;

public interface ITermUniquenessChecker
{
    Task<bool> IsOverlappingAsync(DateRange range);
}
