using FAP.Common.Application.Interfaces;
using FAP.Common.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAP.Common.Application.Behaviors
{
    public class PerformanceBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly Stopwatch _timer;
        private readonly IPerformanceLogRepository _repo;
        private readonly IRequestMetadataService _metadata;
        private readonly ICurrentUserService _currentUser;

        public PerformanceBehavior(
            IPerformanceLogRepository repo,
            IRequestMetadataService metadata,
            ICurrentUserService currentUser)
        {
            _timer = new Stopwatch();
            _repo = repo;
            _metadata = metadata;
            _currentUser = currentUser;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            _timer.Start();
            var response = await next();
            _timer.Stop();

            var featureName = typeof(TRequest).Name;
            var elapsed = _timer.ElapsedMilliseconds;
            //var ctx = _http.HttpContext;

            var log = new PerformanceLog
            {
                TraceId = Guid.NewGuid(),
                UserName = _currentUser?.UserName,
                CampusCode = _currentUser?.CampusCode,
                FeatureName = featureName,
                ElapsedMilliseconds = (int)elapsed,
                RequestTime = DateTime.UtcNow,
                Method = _metadata.Method,
                Path = _metadata.Path,
                StatusCode = _metadata.StatusCode,
                Host = Environment.MachineName
            };

            await _repo.AddAsync(log, cancellationToken);

            return response;
        }
    }


}
