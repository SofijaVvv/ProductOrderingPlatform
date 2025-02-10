using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PaymentService.Infrastructure.ConfigModel;
using PaymentService.Infrastructure.Interface;
using RabbitMQ.Client;

namespace PaymentService.Infrastructure.Event;

public class EventPublisher : IEventPublisher
{
	private readonly ILogger<EventPublisher> _logger;
	private readonly RabbitMqConfig _config;

	public EventPublisher(ILogger<EventPublisher> logger, IConfiguration configuration)
	{
		_logger = logger;
		_config = configuration.GetSection("RabbitMqConfig").Get<RabbitMqConfig>()
		          ?? throw new InvalidOperationException("Configuration section 'RabbitMqConfig' is missing or invalid");
	}

	public  void PublishAsync(string topic, object message)
	{
		var factory = new ConnectionFactory()
		{
			HostName = _config.Host,
			UserName = _config.UserName,
			Password = _config.Password
		};

		try
		{
			using var connection = factory.CreateConnection();
			using var channel = connection.CreateModel();

			channel.ExchangeDeclare(exchange: "payment_exchange", type: ExchangeType.Topic);


			var serializedMessage = JsonSerializer.Serialize(message);
			_logger.LogInformation($"Serialized message: {serializedMessage}");
			var body = Encoding.UTF8.GetBytes(serializedMessage);

			channel.BasicPublish(
				exchange: "payment_exchange",
				routingKey: topic,
				basicProperties: null,
				body: body
			);

			_logger.LogInformation($"Published message to topic: {topic}, message: {serializedMessage}");
		}
		catch (Exception ex)
		{
			_logger.LogError($"An error occurred while publishing to RabbitMQ: {ex.Message}");
			throw;
		}
	}
}
