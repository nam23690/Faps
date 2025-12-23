using FAP.Common.Domain.Academic.Terms;

namespace FAP.Common.Domain.Events
{
    public class TermCreatedEvent : IDomainEvent
    {
        public Term term { get; }
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public TermCreatedEvent(Term _term)
        {
            term = _term;
        }
    }
}
