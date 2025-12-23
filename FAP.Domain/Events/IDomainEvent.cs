namespace FAP.Common.Domain.Events
{

    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
    }


}
