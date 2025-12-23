using FAP.Domain.Common;

namespace FAP.Common.Domain.Academic.Terms.Events;

public class TermCreatedDomainEvent : DomainEvent
{
    public Guid TermId { get; }

    public TermCreatedDomainEvent(Guid termId)
    {
        TermId = termId;
    }
}
