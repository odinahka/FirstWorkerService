namespace FirstWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        public IServiceProvider services { get; }

        public Worker() { }
        public Worker(ILogger<Worker> logger, IServiceProvider services)
        {
            _logger = logger;
            this.services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await ConsumerService(stoppingToken);
        }

        private async Task ConsumerService(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Consume Scoped Service running now");
            using (var scope = services.CreateScope())
            {
                var myService = scope.ServiceProvider.GetRequiredService<ISimpleService>();
                await myService.Perform(stoppingToken);
            }
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stop worker Service");
            return base.StopAsync(cancellationToken);
        }

        public string GetConnectionStrings(string attribute)
        {
            var value = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json")
                        .Build().GetSection("ConnectionStrings")[attribute];
            return value;
        }
    }
}