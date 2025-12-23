using MediatR;
using FAP.Common.Application.Interfaces;
using FAP.Common.Domain.Academic.Terms;
using FAP.Common.Domain.Academic.Terms.Services;
using FAP.Common.Domain.Academic.Terms.ValueObjects;

namespace FAP.Common.Application.Features.Terms.Commands;

public class CreateTermCommand : IRequest<Guid>
{
    public string Name { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public class Handler : IRequestHandler<CreateTermCommand, Guid>
    {
        private readonly ITermRepository _termRepository;
        private readonly ITermUniquenessChecker _checker;

        public Handler(
            ITermRepository termRepository,
            ITermUniquenessChecker checker)
        {
            _termRepository = termRepository;
            _checker = checker;
        }

        public async Task<Guid> Handle(
            CreateTermCommand request,
            CancellationToken cancellationToken)
        {
            var duration = new DateRange(
                request.StartDate,
                request.EndDate);

            var term = await Term.CreateAsync(
                new TermName(request.Name),
                duration,
                _checker);

            await _termRepository.AddAsync(term, cancellationToken);

            return term.Id;
        }
    }
}
