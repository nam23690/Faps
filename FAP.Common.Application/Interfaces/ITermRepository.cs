using FAP.Share.Dtos;
using FAP.Common.Domain.Entities;

namespace FAP.Common.Application.Interfaces
{
    public interface ITermRepository
    {
        Task<Term?> GetByIdAsync(int id,CancellationToken token);
        Task<List<TermDto>> GetAllAsync(CancellationToken token);
        Task AddAsync(Term term, CancellationToken token);
        Task UpdateAsync(Term term, CancellationToken token);
        Task DeleteAsync(short id, CancellationToken token);
    }

}
