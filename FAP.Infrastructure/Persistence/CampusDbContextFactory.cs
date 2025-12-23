using FAP.Common.Application.Interfaces;
using FAP.Common.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAP.Common.Infrastructure.Persistence
{
    public class CampusDbContextFactory : ICampusDbContextFactory
    {
        private readonly IConfiguration _config;
        private readonly ICampusProvider _campusProvider;
        private IUniversityDbContext _cached;
        private string _cachedCampusCode;
        private IMediator _mediator;

        public CampusDbContextFactory(IConfiguration config, ICampusProvider campusProvider, IMediator mediator)
        {
            _config = config;
            _campusProvider = campusProvider;
            _mediator = mediator;
        }

        public IUniversityDbContext Create(string campusCode)
        {
            var connectionString = _config.GetConnectionString(campusCode);
            var optionsBuilder = new DbContextOptionsBuilder<UniversityDbContext>();
            
            if (_cached != null && _cachedCampusCode == campusCode)
            {
                return _cached;
            }
            optionsBuilder.UseSqlServer(connectionString);
            _cachedCampusCode = campusCode;
            _cached = new UniversityDbContext(optionsBuilder.Options,_campusProvider, _mediator);
            return _cached;
            
        }
        public IUniversityDbContext Create()
        {
            var campusCode = _campusProvider.GetCampusCode();
            if (_cached != null && _cachedCampusCode == campusCode)
            {
                return _cached;
            }
            var connectionString = _config.GetConnectionString(campusCode);
            var optionsBuilder = new DbContextOptionsBuilder<UniversityDbContext>();
            optionsBuilder.UseSqlServer(connectionString);
            _cachedCampusCode = campusCode;
            _cached = new UniversityDbContext(optionsBuilder.Options, _campusProvider, _mediator);
            return _cached;
        }
    }
}
