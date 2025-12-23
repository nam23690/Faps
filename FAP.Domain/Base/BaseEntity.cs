using FAP.Common.Domain.Events;

namespace FAP.Common.Domain.Base
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        private readonly List<IDomainEvent> _domainEvents = new();

        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public void AddDomainEvent(IDomainEvent eventItem) => _domainEvents.Add(eventItem);

        public void ClearDomainEvents() => _domainEvents.Clear();
    }

    public abstract class BaseEntityString
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        private readonly List<IDomainEvent> _domainEvents = new();

        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public void AddDomainEvent(IDomainEvent eventItem) => _domainEvents.Add(eventItem);

        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}
