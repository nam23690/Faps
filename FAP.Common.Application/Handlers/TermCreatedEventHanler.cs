using FAP.Common.Application.Events;
using FAP.Common.Domain.Events;
using MediatR;

namespace FAP.Common.Application.Handlers
{


    
    public class TermCreatedEventHandler
        : INotificationHandler<DomainEventNotification<TermCreatedEvent>>
    {
        public Task Handle(DomainEventNotification<TermCreatedEvent> notification, CancellationToken ct)
        {
            Console.WriteLine("Bắt đầu chạy event Term created: " + notification.DomainEvent.term.Name);
            return Task.CompletedTask;
        }
    }


}
