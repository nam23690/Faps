using FAP.Common.Domain.Academic.Terms.Events;
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

        var term = new Term
        {
            Id = Guid.NewGuid(),
            Name = name,
            Duration = duration,
            IsDeleted = false
        };

        term.AddDomainEvent(new TermCreatedDomainEvent(term.Id));
        return term;

    }

    public async Task UpdateAsync(
       TermName name,
       DateRange duration,
       ITermUniquenessChecker checker)
    {
        if (await checker.IsOverlappingAsync(duration, Id))
            throw new DomainException("Term overlaps");

        if (IsDeleted)
            throw new DomainException("Cannot update deleted term");

        Name = name;
        Duration = duration;
    }

    public void SoftDelete()
    {
        if (IsDeleted)
            throw new DomainException("Term already deleted");
        IsDeleted = true;
        AddDomainEvent(new TermDeletedDomainEvent(Id));
    }
}
