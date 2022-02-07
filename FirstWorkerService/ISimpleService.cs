using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstWorkerService
{
    internal interface ISimpleService
    {
        Task Perform(CancellationToken stoppingToken);
    }
}
