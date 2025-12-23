using FAP.Common.Application.Interfaces;
using FAP.Share.Dtos;
using FAP.Share.Dtos.Terms;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FAP.Common.Application.Features.Terms.Queries;

public class GetTermsQuery : IRequest<PagedResult<TermItemDto>>
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Keyword { get; set; }

    public class Handler : IRequestHandler<GetTermsQuery, PagedResult<TermItemDto>>
    {
        private readonly IUniversityDbContext _context;

        public Handler(IUniversityDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<TermItemDto>> Handle(
            GetTermsQuery request,
            CancellationToken cancellationToken)
        {
            var query = _context.Terms
                .AsNoTracking()
                .Where(x => !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                query = query.Where(x =>
                    x.Name.Value.Contains(request.Keyword));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(x => x.Duration.StartDate)
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new TermItemDto
                {
                    Id = x.Id,
                    Name = x.Name.Value,
                    StartDate = x.Duration.StartDate,
                    EndDate = x.Duration.EndDate
                })
                .ToListAsync(cancellationToken);

            return new PagedResult<TermItemDto>
            {
                Items = items,
                TotalRecords = totalCount,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize
            };
        }
    }
}
