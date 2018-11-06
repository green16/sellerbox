using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace SellerBox
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);
            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
#if DEBUG
                    config.AddJsonFile("appsettings.Debug.json", false, true);
#elif RELEASE
                    config.AddJsonFile("appsettings.Release.json", false, true);
#else
                    config.AddJsonFile("appsettings.Development.json", false, true);
#endif
                    config.AddEnvironmentVariables();
                })
                .UseStartup<Startup>()
                .Build();
    }
}
