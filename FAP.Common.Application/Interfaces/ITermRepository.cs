using FAP.Common.Domain.Academic.Terms;

namespace FAP.Common.Application.Interfaces
{
    public interface ITermRepository
    {
        Task AddAsync(Term term, CancellationToken token);
        Task<Term?> GetByIdAsync(Guid id,CancellationToken cancellationToken);
        Task UpdateAsync(Term term,CancellationToken cancellationToken);
    }

}
