using FAP.Common.Domain.Academic.Terms.Services;
using FAP.Common.Domain.Academic.Terms.ValueObjects;
using FAP.Common.Domain.Common;
using FAP.Domain.Common;

namespace FAP.Common.Domain.Academic.Terms;

public class Term : AggregateRoot
{
    public TermName Name { get; private set; }
    public DateRange Duration { get; private set; }
    public bool IsDeleted { get; private set; }

    private Term() { }

    public static async Task<Term> CreateAsync(
        TermName name,
        DateRange duration,
        ITermUniquenessChecker checker)
    {
        if (await checker.IsOverlappingAsync(duration))
            throw new DomainException("Term date overlaps");

        return new Term
        {
            Id = Guid.NewGuid(),
            Name = name,
            Duration = duration,
            IsDeleted = false
        };
    }

    public async Task UpdateAsync(
       TermName name,
       DateRange duration,
       ITermUniquenessChecker checker)
    {
        if (await checker.IsOverlappingAsync(duration, Id))
            throw new DomainException("Term overlaps");

        Name = name;
        Duration = duration;
    }




    public void SoftDelete()
    {
        IsDeleted = true;
    }
}
