using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstWorkerService
{
    internal class TimedWorkerService : IHostedService, IDisposable
    {
        private readonly ILogger<TimedWorkerService> _logger;

        private int _counter  = 0;
        private Timer _timer;
        public TimedWorkerService(ILogger<TimedWorkerService> logger)
        {
            _logger = logger;
        }
        public void Dispose()
        {
            _timer?.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Worker Service Running...");
            _timer = new Timer(Perform, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Worker Service is Stopping...");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private void Perform(object state)
        {
            var counter = Interlocked.Increment(ref _counter);
            _logger.LogInformation("Counter: {0}", counter);
        }
    }
}
