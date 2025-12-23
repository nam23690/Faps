using MediatR;
using FAP.Common.Application.Interfaces;
using FAP.Common.Domain.Common;

namespace FAP.Common.Application.Features.Terms.Commands;

public class DeleteTermCommand : IRequest<Unit>
{
    public Guid Id { get; set; }

    public class Handler
        : IRequestHandler<DeleteTermCommand, Unit>
    {
        private readonly ITermRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        public Handler(ITermRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(
            DeleteTermCommand request,
            CancellationToken cancellationToken)
        {
            var term = await _repository.GetByIdAsync(
                request.Id,
                cancellationToken);

            if (term == null)
                throw new DomainException("Term not found");

            term.SoftDelete();
            // 🔥 COMMIT RÕ RÀNG – STRICT DDD
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
