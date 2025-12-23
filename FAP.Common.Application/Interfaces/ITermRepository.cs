using FAP.Share.Dtos;
using FAP.Common.Domain.Entities;

namespace FAP.Common.Application.Interfaces
{
    public interface ITermRepository
    {
        Task AddAsync(Term term, CancellationToken token);
    }

}
