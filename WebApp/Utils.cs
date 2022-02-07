namespace WebApp
{
    public class Utils
    {
        public static string GetAppSettings(string attributes)
        {
            return new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")[attributes];
        }
        public static string ConnectionStrings(string attributes)
        {
            return new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")[attributes];
        }
       
    }
}
