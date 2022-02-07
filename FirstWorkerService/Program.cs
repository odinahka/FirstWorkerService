using FirstWorkerService;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddHostedService<TimedWorkerService>();
        services.AddHostedService<ThreadWorkerService>();
        services.AddHostedService<FileWatcher>();
        services.AddHostedService<SocketServerWorkerService>();
        services.AddScoped<ISimpleService, SimpleService>();
    })
    .Build();

await host.RunAsync();
