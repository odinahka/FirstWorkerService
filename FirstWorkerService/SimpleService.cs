using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstWorkerService
{
    internal class SimpleService : ISimpleService
    {
        private readonly ILogger<SimpleService> logger;
        

        public SimpleService(ILogger<SimpleService> logger)
        {
            this.logger = logger;
            
        }
        public async Task Perform(CancellationToken stoppingToken)
        {
            var worker = new Worker();
            while (!stoppingToken.IsCancellationRequested)
            {
                var connectionString = worker.GetConnectionStrings("OracleDBConnection");
                logger.LogInformation("SimpleService running at: {time} while getting {connectionString}", DateTimeOffset.Now, connectionString);
                await Task.Delay(5000, stoppingToken);
                
            }
            
        }
    }
}
