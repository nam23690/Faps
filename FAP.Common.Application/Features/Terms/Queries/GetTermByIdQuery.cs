using FAP.Common.Application.Attributes;
using FAP.Common.Application.Interfaces;
using FAP.Common.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;


namespace FAP.Common.Application.Features.User.Queries
{
    [FapPermission("Term.View")]
    public class GetTermByIdQuery : IRequest<FAP.Common.Domain.Entities.Term?>
    {
        public short Id { get; set; }

        public class Handler : IRequestHandler<GetTermByIdQuery, FAP.Common.Domain.Entities.Term?>
        {
            private readonly ITermRepository _termRepository;

            public Handler(ITermRepository userRepository)
            {
                _termRepository = userRepository;
            }

            public async Task<FAP.Common.Domain.Entities.Term?> Handle(GetTermByIdQuery request, CancellationToken cancellationToken)
            {
                return await _termRepository.GetByIdAsync(request.Id,cancellationToken);
                
            }
        }
    }
}
