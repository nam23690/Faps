using MediatR;
using FAP.Common.Application.Interfaces;
using FAP.Common.Domain.Common;

namespace FAP.Common.Application.Features.Terms.Commands;

public class DeleteTermCommand : IRequest
{
    public Guid Id { get; set; }

    public class Handler : IRequestHandler<DeleteTermCommand>
    {
        private readonly ITermRepository _repository;

        public Handler(ITermRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(
            DeleteTermCommand request,
            CancellationToken cancellationToken)
        {
            var term = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (term == null)
                throw new DomainException("Term not found");

            term.SoftDelete();

            await _repository.UpdateAsync(term, cancellationToken);

            return Unit.Value;
        }
    }
}
