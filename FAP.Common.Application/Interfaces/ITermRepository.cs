using FAP.Common.Domain.Academic.Terms;

namespace FAP.Common.Application.Interfaces
{
    public interface ITermRepository
    {
        Task AddAsync(Term term, CancellationToken token);
    }

}
