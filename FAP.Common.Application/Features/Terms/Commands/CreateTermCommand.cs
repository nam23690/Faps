using FAP.Common.Application.Interfaces;
using FAP.Common.Domain.Academic.Terms;
using FAP.Common.Domain.Academic.Terms.Services;
using FAP.Common.Domain.Academic.Terms.ValueObjects;
using MediatR;

internal sealed class CreateTermCommandHandler
    : IRequestHandler<CreateTermCommand, Guid>
{
    private readonly ITermRepository _repo;
    private readonly ITermUniquenessChecker _checker;

    public CreateTermCommandHandler(
        ITermRepository repo,
        ITermUniquenessChecker checker)
    {
        _repo = repo;
        _checker = checker;
    }

    public async Task<Guid> Handle(CreateTermCommand cmd, CancellationToken ct)
    {
        var term = await Term.CreateAsync(
            new TermName(cmd.Name),
            new DateRange(cmd.StartDate, cmd.EndDate),
            _checker
        );

        await _repo.AddAsync(term);

        return term.Id;
    }
}
