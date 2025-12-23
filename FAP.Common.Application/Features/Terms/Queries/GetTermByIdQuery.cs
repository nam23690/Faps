using MediatR;
using Microsoft.EntityFrameworkCore;
using FAP.Common.Application.Interfaces;
using FAP.Share.Dtos.Terms;

namespace FAP.Common.Application.Features.Terms.Queries;

public class GetTermByIdQuery : IRequest<TermItemDto?>
{
    public Guid Id { get; set; }

    public class Handler : IRequestHandler<GetTermByIdQuery, TermItemDto?>
    {
        private readonly IUniversityDbContext _context;

        public Handler(IUniversityDbContext context)
        {
            _context = context;
        }

        public async Task<TermItemDto?> Handle(
            GetTermByIdQuery request,
            CancellationToken cancellationToken)
        {
            return await _context.Terms
                .AsNoTracking()
                .Where(x => x.Id == request.Id && !x.IsDeleted)
                .Select(x => new TermItemDto
                {
                    Id = x.Id,
                    Name = x.Name.Value,
                    StartDate = x.Duration.StartDate,
                    EndDate = x.Duration.EndDate
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
