using FAP.Common.Application.Interfaces;
using FAP.Common.Infrastructure.Persistence;
using FAP.Common.Infrastructure.Repositories;
using FAP.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore.Storage;
using System.Transactions;



namespace FAP.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UniversityDbContext _context;
        private readonly IMediator _mediator;


        public UnitOfWork(UniversityDbContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct)
        {
            // 1. Lấy domain events trước commit
            var domainEvents = _context.ChangeTracker
                .Entries<AggregateRoot>()
                .SelectMany(e => e.Entity.DomainEvents)
                .ToList();

            // 2. Commit DB
            var result = await _context.SaveChangesAsync(ct);

            // 3. Dispatch domain events SAU commit
            foreach (var domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent, ct);
            }

            // 4. Clear
            foreach (var entry in _context.ChangeTracker.Entries<AggregateRoot>())
            {
                entry.Entity.ClearDomainEvents();
            }

            return result;
        }
    }


}
