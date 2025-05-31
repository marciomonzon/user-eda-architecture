using EmailSender.Data;
using EmailSender.Enum;
using Microsoft.EntityFrameworkCore;
using System;

namespace EmailSender
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _provider;

        public Worker(ILogger<Worker> logger, IServiceProvider provider)
        {
            _logger = logger;
            _provider = provider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _provider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var pendings = await db.EmailNotifications
                                       .Where(n => n.Status == NotificationStatus.Pending)
                                       .ToListAsync();

                foreach (var n in pendings)
                {
                    try
                    {
                        Console.WriteLine($"Enviando e-mail para {n.Email}...");
                        // Simula envio de e-mail
                        await Task.Delay(500);
                        n.Status = NotificationStatus.Sent;
                        n.SentAt = DateTime.UtcNow;
                    }
                    catch
                    {
                        n.Status = NotificationStatus.Failed;
                    }
                }
                await db.SaveChangesAsync();
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
