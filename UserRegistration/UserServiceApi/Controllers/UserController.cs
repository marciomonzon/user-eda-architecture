using Contracts;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using UserServiceApi.DTO;

namespace UserServiceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpPost]
        public IActionResult Register([FromBody] RegisterUserRequest request)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare("user.exchange", ExchangeType.Fanout);

            var evt = new UserRegisteredEvent
            {
                UserId = Guid.NewGuid(),
                Email = request.Email,
                Name = request.Name,
                RegisteredAt = DateTime.UtcNow
            };

            var json = JsonSerializer.Serialize(evt);
            var body = Encoding.UTF8.GetBytes(json);

            var props = channel.CreateBasicProperties();
            props.Persistent = true; // garante que a mensagem seja persistente

            channel.BasicPublish(
                exchange: "user.exchange",
                routingKey: string.Empty,
                basicProperties: props,
                body: body
            );

            return Ok(new { message = "Usuário registrado com sucesso", evt.UserId });
        }
    }
}
