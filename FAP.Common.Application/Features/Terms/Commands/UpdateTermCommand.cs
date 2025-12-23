using FAP.Common.Application.Interfaces;
using FAP.Common.Domain.Academic.Terms.Services;
using FAP.Common.Domain.Academic.Terms.ValueObjects;
using FAP.Common.Domain.Common;
using MediatR;

namespace FAP.Common.Application.Features.Terms.Commands;

public class UpdateTermCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public class Handler
        : IRequestHandler<UpdateTermCommand, Unit>
    {
        private readonly ITermRepository _repository;
        private readonly ITermUniquenessChecker _checker;
        private readonly IUnitOfWork _unitOfWork;

        public Handler(
            ITermRepository repository,
            ITermUniquenessChecker checker,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _checker = checker;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(
    UpdateTermCommand request,
    CancellationToken cancellationToken)
        {
            var term = await _repository.GetByIdAsync(
                request.Id,
                cancellationToken);

            if (term == null)
                throw new DomainException("Term not found");

            var duration = new DateRange(
                request.StartDate,
                request.EndDate);

            await term.UpdateAsync(
                new TermName(request.Name),
                duration,
                _checker); // ✅ BẮT BUỘC
            // 🔥 COMMIT RÕ RÀNG – STRICT DDD
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }

    }
}
