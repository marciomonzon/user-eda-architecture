using Contracts;
using Notification.Data;
using Notification.Enum;
using Notification.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Notification
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
            var factory = new ConnectionFactory { HostName = "localhost" };
            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();
            await channel.ExchangeDeclareAsync("user.exchange", ExchangeType.Fanout);
            string queueName = "user.email.notification";
            await channel.QueueDeclareAsync(queue: queueName,
                                durable: true,
                                exclusive: false,
                                autoDelete: false);

            await channel.QueueBindAsync(queue: queueName,
                                         exchange: "user.exchange",
                                         routingKey: "");

            var result = await channel.BasicGetAsync(queueName, true);
            if (result != null)
            {
                Console.WriteLine("Mensagem recebida via BasicGet!");
                Console.WriteLine(Encoding.UTF8.GetString(result.Body.ToArray()));
            }
            else
            {
                Console.WriteLine("Nenhuma mensagem na fila.");
            }

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (s, e) =>
            {
                var json = Encoding.UTF8.GetString(e.Body.ToArray());
                var evt = JsonSerializer.Deserialize<UserRegisteredEvent>(json);

                using var scope = _provider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                if (evt != null)
                {
                    db.EmailNotifications.Add(new EmailNotification
                    {
                        UserId = evt.UserId,
                        Email = evt.Email,
                        Name = evt.Name,
                        CreatedAt = DateTime.UtcNow,
                        Status = NotificationStatus.Pending
                    });
                    await db.SaveChangesAsync();
                }
            };

            await channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer);
            Console.WriteLine("Consumidor iniciado com sucesso na fila: " + queueName);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }

        }
    }
}
