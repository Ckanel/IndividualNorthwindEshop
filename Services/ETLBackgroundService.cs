using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.DependencyInjection;
using ETL.Load;

namespace IndividualNorthwindEshop.Services
{
    public class ETLBackgroundService : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;

        public ETLBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Initialize and start the timer to run the ETL process immediately and then every hour
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(1));
            return Task.CompletedTask;
        }

        private async Task RunETLProcess()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var etlProcess = scope.ServiceProvider.GetRequiredService<ETLProcess>();

                try
                {
                    await etlProcess.RunETLAsync();
                }
                catch (Exception ex)
                {
                    // Log the exception
                    Console.WriteLine($"ETL process encountered an error: {ex.Message}");
                }
            }
        }

        private void DoWork(object state)
        {
            RunETLProcess().GetAwaiter().GetResult();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Stop the timer when the service stops
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            // Dispose of the timer if it's not null
            _timer?.Dispose();
        }
    }
}