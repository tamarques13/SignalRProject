using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Client.Services.Interfaces;
using Client.Services;

namespace Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            builder.Services.AddSingleton<Application>();

            builder.Services.AddTransient<IDataHandlerService, DataHandlerService>();
            builder.Services.AddTransient<ICollectInputService, CollectInputService>();

            builder.Services.AddScoped<IMessageQueueService, MessageQueueService>();
            builder.Services.AddScoped<IShutDownHandlerService, ShutDownHandlerService>();
            builder.Services.AddScoped<ISignalRConnectionService, SignalRConnectionService>();

            using var host = builder.Build();
            
            var app = host.Services.GetRequiredService<Application>();

            await app.ConsoleApp();
        }
    }
}