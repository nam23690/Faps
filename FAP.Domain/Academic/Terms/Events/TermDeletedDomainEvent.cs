using FAP.Domain.Common;

public class TermDeletedDomainEvent : DomainEvent
{
    public Guid TermId { get; }

    public TermDeletedDomainEvent(Guid termId)
    {
        TermId = termId;
    }
}
