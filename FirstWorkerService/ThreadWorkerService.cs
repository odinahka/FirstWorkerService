using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstWorkerService
{
    internal class ThreadWorkerService : Worker, IHostedService, IDisposable 
    {
        private readonly ILogger<ThreadWorkerService> _logger;
        private bool _isRunning;
        private Thread _thread;
        public ThreadWorkerService(ILogger<ThreadWorkerService> logger) 
        {
            _logger = logger;
        }
        public override void Dispose()
        {
            
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _isRunning = true;
            var start = new ThreadStart(Perform);

            _thread = new Thread(start);
            _thread.Start();
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _isRunning = false;
            _thread.Join(500); //waiting for thread to terminate
            return Task.CompletedTask;
        }
        public void Perform()
        {
            int counter = 0;
            try
            {
                while(_isRunning)
                {
                    _logger.LogInformation("Counter: {counter}", counter);
                    counter++;
                    if(counter > 100)
                        counter = 0;
                    if (!_isRunning)
                        break;

                    Thread.Sleep(2000);

                }
            }
            catch (Exception exception) 
            {
            }
        }
    }
}
