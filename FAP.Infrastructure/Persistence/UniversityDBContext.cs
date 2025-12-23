using FAP.Common.Application.Events;
using FAP.Common.Application.Interfaces;
using FAP.Common.Domain.Academic.Terms;
using FAP.Common.Domain.Base;
using FAP.Common.Domain.Entities;
using FAP.Common.Domain.Events;
using FAP.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using static System.Net.WebRequestMethods;

namespace FAP.Common.Infrastructure.Persistence
{
    public class UniversityDbContext : DbContext, IUniversityDbContext
    {
        
        private readonly ICampusProvider _campusProvider;
        private readonly IMediator _mediator;
        public UniversityDbContext(DbContextOptions<UniversityDbContext> options, ICampusProvider campusProvider, IMediator mediator)
            : base(options)
        {
             _campusProvider = campusProvider;
            _mediator = mediator;
        }

        #region DbSets
        public DbSet<Domain.Academic.Terms.Term> Terms { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ❗ BẮT BUỘC: EF KHÔNG ĐƯỢC MAP DOMAIN EVENT
            modelBuilder.Ignore<IDomainEvent>();
            modelBuilder.Ignore<DomainEvent>(); // nếu có class concrete
            base.OnModelCreating(modelBuilder);

            // Apply all configurations in the current assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UniversityDbContext).Assembly);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Audit, soft delete, domain events, etc.
          
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;
                }
            }

            var domainEvents = ChangeTracker.Entries<BaseEntity>()
             .Select(x => x.Entity.DomainEvents)
            .SelectMany(x => x)
             .ToList();

            // Clear để tránh chạy lại nếu SaveChanges được gọi lần 2
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
                entry.Entity.ClearDomainEvents();
            //Chạy giao dịch chính
            int result= await base.SaveChangesAsync(cancellationToken);


            //publish domain events để chạy
            foreach (var domainEvent in domainEvents)
                await PublishDomainEvent(domainEvent, cancellationToken);

            return result;

        }

        private async Task PublishDomainEvent(IDomainEvent domainEvent, CancellationToken ct)
        {
            var wrapperType = typeof(DomainEventNotification<>)
                .MakeGenericType(domainEvent.GetType());

            var wrapperInstance = Activator.CreateInstance(wrapperType, domainEvent);

            if (wrapperInstance is INotification notification)
            {
                await _mediator.Publish(notification, ct);
            }
            else
            {
                throw new ArgumentException("Wrapper instance does not implement INotification.");
            }
        }




    }
}
