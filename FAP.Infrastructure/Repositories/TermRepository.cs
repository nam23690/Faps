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
    internal sealed class TermRepository : ITermRepository
    {
        private readonly FapDbContext _context;

        public Task AddAsync(Term term)
        {
            _context.Add(term); // ✅ CHUẨN
            return Task.CompletedTask;
        }
    }

}
