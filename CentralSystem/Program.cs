using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace CentralSystem
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateWebHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    }
}
