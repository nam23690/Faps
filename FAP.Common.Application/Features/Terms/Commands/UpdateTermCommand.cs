using FAP.Common.Application.Attributes;
using FAP.Common.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FAP.Common.Application.Features.Term.Commands
{
    [FapPermission("Term.Update")]
    public class UpdateTermCommand : IRequest<Unit>
    {
        public short Id { get; set; }
        public short CampusID { get; set; }
        public string SemesterName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; } = DateTime.Today;
        public DateTime EndDate { get; set; } = DateTime.Today;
        public bool IsClosed { get; set; }

        public class Handler : IRequestHandler<UpdateTermCommand, Unit>
        {
            private readonly IUnitOfWork _uow;

            public Handler(IUnitOfWork uow)
            {
                _uow  = uow;
            }

            public async Task<Unit> Handle(UpdateTermCommand request, CancellationToken cancellationToken)
            {

                var entity = await _uow.Terms.GetByIdAsync(request.Id,cancellationToken);

                if (entity == null)
                    throw new KeyNotFoundException("Term not found");
                
                // Use Domain Method
                entity.UpdateInfo(
                    request.CampusID, 
                    request.SemesterName, 
                    request.StartDate, 
                    request.EndDate, 
                    request.IsClosed
                );

                await _uow.Terms.UpdateAsync(entity, cancellationToken);
                await _uow.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }
    }
}
