using FAP.Benchmarks.FakeServices;
using FAP.Common.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAP.Benchmarks.Persistences
{
    public static class BenchmarkDbContextFactory
    {
        public static UniversityDbContext CreateDbContext()
        {
            var campusProvider = new FakeCampusProvider();
            var mediator = new FakeMediator();

            var options = new DbContextOptionsBuilder<UniversityDbContext>()
                .UseSqlServer(campusProvider.GetCampusConnectionString())
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
                .Options;

            return new UniversityDbContext(options, campusProvider, mediator);
        }
    }

}
