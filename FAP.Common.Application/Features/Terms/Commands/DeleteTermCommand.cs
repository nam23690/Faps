using FAP.Common.Application.Attributes;
using FAP.Common.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FAP.Common.Application.Features.Term.Commands
{
    [FapPermission("Term.Delete")]
    public class DeleteTermCommand : IRequest<Unit>
    {
        public int Id { get; set; }

        public class Handler : IRequestHandler<DeleteTermCommand, Unit>
        {
            private readonly IUnitOfWork _uow;

            public Handler(IUnitOfWork uow)
            {
                _uow = uow;
            }

            public async Task<Unit> Handle(DeleteTermCommand request, CancellationToken cancellationToken)
            {
                await _uow.Terms.DeleteAsync((short)request.Id, cancellationToken);
                await _uow.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }
    }
}
