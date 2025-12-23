using DocumentFormat.OpenXml.InkML;
using FAP.Share.Dtos;
using FAP.Common.Application.Features.Term.Commands;
using FAP.Common.Application.Interfaces;
using FAP.Common.Domain.Entities;
using FAP.Common.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAP.Common.Infrastructure.Repositories
{
    public class TermRepository : ITermRepository
    {
        private readonly IUniversityDbContext _context;
        public TermRepository(UniversityDbContext context)
        {

            _context = context;
        }



        public async Task<Term?> GetByIdAsync(int id, CancellationToken token)
            => await _context.Terms.FindAsync(id);


        public async Task<List<TermDto>> GetAllAsync(CancellationToken token)
        {
            return await _context.Terms
               .Select(x => new TermDto
               {
                   SemesterName = x.SemesterName,
                   StartDate = x.StartDate,
                   EndDate = x.EndDate,
                   IsClosed = x.IsClosed ? x.IsClosed : false
               }).ToListAsync();

        }
        public async Task AddAsync(Term term, CancellationToken token)
       => await _context.Terms.AddAsync(term);



        public async Task UpdateAsync(Term term, CancellationToken token)
        => _context.Terms.Update(term);



        public async Task DeleteAsync(short id, CancellationToken token)
        {
            var term = await GetByIdAsync(id, token);
            if (term != null)
            {
                _context.Terms.Remove(term);

            }
        }

        public async Task DeleteAsync(Term term, CancellationToken token)
        => _context.Terms.Remove(term);

        public async Task<bool> IsUniqueNameAsync(string name, int excludeId, CancellationToken ct)
        {
            return !await _context.Terms.AnyAsync(x => x.SemesterName == name && x.Id != excludeId, ct);
        }

    }
}
