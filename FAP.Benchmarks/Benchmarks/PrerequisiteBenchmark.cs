using BenchmarkDotNet.Attributes;
using FAP.Benchmarks.Persistences;
using FAP.Common.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FAP.Benchmarks.Benchmarks
{
   

    [MemoryDiagnoser]
    public class PrerequisiteBenchmark
    {
        private UniversityDbContext _context;

        [GlobalSetup]
        public void Setup()
        {
            _context = BenchmarkDbContextFactory.CreateDbContext();
        }

        [Benchmark]
        public async Task Query10Students()
        {
            var data = await _context.Terms.Take(10).ToListAsync();
        }
    }

}
