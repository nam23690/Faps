using FAP.Common.Application.Events;
using FAP.Common.Domain.Events;
using MediatR;

namespace FAP.Common.Application.Handlers
{   
    public class TermCreatedEventEmailHandler
        : INotificationHandler<DomainEventNotification<TermCreatedEventEmail>>
    {
        public Task Handle(DomainEventNotification<TermCreatedEventEmail> notification, CancellationToken ct)
        {
            Console.WriteLine("\nStarting sending Email: " + notification.DomainEvent.term.Name);
            Console.WriteLine("\nFinised Email: ");
            return Task.CompletedTask;
        }
    }


}
