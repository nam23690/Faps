using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using FAP.Benchmarks.FakeServices;
using FAP.Benchmarks.Persistences;
using FAP.Common.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace FAP.Benchmarks
{


    [MemoryDiagnoser]

    [SimpleJob(RuntimeMoniker.Net80, baseline: true,
    warmupCount: 1, iterationCount: 5)]
    // Job chạy .NET 9
    [SimpleJob(RuntimeMoniker.Net90,
    warmupCount: 1, iterationCount: 5)]
    public class PrerequisiteCheckBenchmark
    {
        private UniversityDbContext _context;

        
        private readonly Consumer _consumer = new Consumer();

        [IterationSetup]
        public void SetupPerIteration()
        {
            _context = BenchmarkDbContextFactory.CreateDbContext();
        }



        [Benchmark]
        public async Task Query10Students()
        {
            var data = await _context.Terms.Take(10).ToListAsync();
            _consumer.Consume(data);
        }

        [Benchmark]
        public int AAA_Test()
        {
            return 1;
        }

    }

}
