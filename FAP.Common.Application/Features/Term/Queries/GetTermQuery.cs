using FAP.Common.Application.Attributes;
using FAP.Share.Dtos;
using FAP.Common.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAP.Common.Application.Features.Term.Queries
{
    [FapPermission("Term.View")]
    public class GetTermQuery
    {
        public record GetTermsQuery : IRequest<List<TermDto>>;
        public class Handler : IRequestHandler<GetTermsQuery, List<TermDto>>
        {
            private readonly ITermRepository _termRepository;
            public Handler(ITermRepository termRepository)
            {
                _termRepository = termRepository;
            }
            public async Task<List<TermDto>> Handle(GetTermsQuery request, CancellationToken cancellationToken)
            => await _termRepository.GetAllAsync(cancellationToken);
            
        }

    }
}
