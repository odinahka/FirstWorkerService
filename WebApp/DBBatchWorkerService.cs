using WebApp.Models;

namespace WebApp
{
    public class DBBatchWorkerService : IHostedService, IDisposable
    {
        private readonly ILogger<DBBatchWorkerService> _logger;
        private readonly IServiceScopeFactory _scope;
        private bool _isRunning;
        private Thread _thread;
        private FileSystemWatcher _watcher;

        public DBBatchWorkerService(ILogger<DBBatchWorkerService> logger, IServiceScopeFactory scope)
        {
            _logger = logger;
            _scope = scope;
        }
        public void Dispose()
        {
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            //file watcher
            _watcher = new FileSystemWatcher
            {
                Filter = "*.csv",
                Path = Utils.GetAppSettings("TargetPath"),
                IncludeSubdirectories = false,
                EnableRaisingEvents = true
            };
            _watcher.Created += _watcher_Created;

            //Thread
            _isRunning = true;
            ThreadStart start = new ThreadStart(Perform);

            _thread = new Thread(start);
            _thread.Start();

            _logger.LogInformation("DB worker service started");

            return Task.CompletedTask;
        }

        private void _watcher_Created(object sender, FileSystemEventArgs e)
        {
            using (var scope = _scope.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<MyBootieContext>();

                //Read file
                _logger.LogInformation("Reading CSV file and uploading to SQL Server.");
                _logger.LogInformation(e.FullPath);
                using (var reader = new StreamReader(e.FullPath))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var value = line.Split(';');

                        // add to database
                        Product o = new Product
                        {
                            ProductName = value[0],
                            Quantity = Convert.ToInt32(value[1]),
                            Price = Convert.ToInt32(value[2]),
                            DateCreated = DateTime.Now
                        };
                        context.Products.Add(o);
                    }
                    //commit data
                    context.SaveChanges();
                }
                _logger.LogInformation("Finished uploading data to SQL Server");

                //Remove file
                if (File.Exists(e.FullPath))
                    File.Delete(e.FullPath);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _isRunning = false;
            _thread.Join(500); //Waiting to join and terminate

            return Task.CompletedTask;
        }

        public void Perform()
        {
            try
            {
                while (_isRunning)
                {
                    if (!_isRunning)
                        break;
                    Thread.Sleep(800);
                }
            }
            catch (Exception) { }
        }
    }
}
