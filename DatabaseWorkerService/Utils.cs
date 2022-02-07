using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseWorkerService
{
    internal class Utils
    {
        public static string ConnectionStrings(string attribute)
        {
            var value = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")[attribute];
            return value; 
        }
        
        public static string GetAppSettings(string attribute)
        {
            var value = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")[attribute];
            return value;

        }
    }
}
