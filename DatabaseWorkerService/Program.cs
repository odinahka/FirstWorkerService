using DatabaseWorkerService;
using DatabaseWorkerService.Models;
using Microsoft.EntityFrameworkCore;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        //Register DBContext EF core
        var conString = Utils.ConnectionStrings("DefaultConnection");
        services.AddHostedService<DatabaseWorker>();
        services.AddDbContext<MyBootieContext>(
            options =>
            options.UseSqlServer(conString)
            );
    })
    .Build();

await host.RunAsync();
