using FAP.Common.Application.Attributes;
using FAP.Common.Application.Interfaces;
using FAP.Common.Domain.Entities;
using FAP.Common.Domain.Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FAP.Common.Application.Features.Term.Commands {
    [FapPermission("Term.Create")]
    public class CreateTermCommand : IRequest<int>
    {
        public short CampusID { get; set; }
        public string SemesterName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; } = DateTime.Today;
        public DateTime EndDate { get; set; } = DateTime.Today;
        public bool IsClosed { get; set; }

        public class Handler : IRequestHandler<CreateTermCommand, int>
        {
            private readonly IUnitOfWork _uow;

            public Handler(IUnitOfWork uow)
            {
                _uow = uow;
            }

            public async Task<int> Handle(CreateTermCommand request, CancellationToken cancellationToken)
            {
                // Use Factory Method
                var entity = FAP.Common.Domain.Entities.Term.Create(
                    request.CampusID, 
                    request.SemesterName, 
                    request.StartDate, 
                    request.EndDate
                );

                // Note: Domain Events are already added inside Term.Create()
                
                // If IsClosed was intended to be set on creation (rare, but possible)
                if (request.IsClosed)
                {
                    entity.Close();
                }

                await _uow.Terms.AddAsync(entity, cancellationToken);
                await _uow.SaveChangesAsync(cancellationToken);                
                
                return entity.Id;
            }
        }
    }
}
