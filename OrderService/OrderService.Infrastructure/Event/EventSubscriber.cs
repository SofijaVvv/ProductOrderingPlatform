using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OrderService.Infrastructure.ConfigModel;
using OrderService.Model.Dto;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace OrderService.Infrastructure.Event;

public class EventSubscriber
{
	private readonly ILogger<EventSubscriber> _logger;
	private readonly RabbitMqConfig _config;

	public EventSubscriber(ILogger<EventSubscriber> logger, IConfiguration configuration)
	{
		_logger = logger;
		_config = configuration.GetSection("RabbitMqConfig").Get<RabbitMqConfig>() ??
		          throw new InvalidOperationException("RabbitMqConfig is missing");
	}

	public void Subscribe()
	{
		var factory = new ConnectionFactory()
		{
			HostName = _config.Host,
			UserName = _config.UserName,
			Password = _config.Password,
		};

		var connection = factory.CreateConnection();
		var channel = connection.CreateModel();

		channel.ExchangeDeclare(exchange: "payment_exchange", type: ExchangeType.Topic);

		channel.QueueDeclare(
			queue: _config.QueueName,
			durable: true,
			exclusive: false,
			autoDelete: false,
			arguments: null
		);

		channel.QueueBind(
			queue: _config.QueueName,
			exchange: "payment_exchange",
			routingKey: "payment.status"
		);

		var consumer = new EventingBasicConsumer(channel);
		consumer.Received += (_, ea) =>
		{
			try
			{
				var body = ea.Body.ToArray();
				var message = Encoding.UTF8.GetString((byte[])body);

				_logger.LogInformation($"Received raw message: {message}");

				// var options = new JsonSerializerOptions
				// {
				// 	PropertyNameCaseInsensitive = true,
				// };

				var paymentEvent = JsonSerializer.Deserialize<PaymentEventMessage>(message);
				if (paymentEvent != null)
				{
					_logger.LogInformation($"Deserialized message: {message}");
					channel.BasicAck(ea.DeliveryTag, false);
				}
				else
				{
					_logger.LogWarning("Received a null message after deserialization!");
					channel.BasicNack(ea.DeliveryTag, false, true);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError($"Error processing message: {ex.Message}");
				channel.BasicNack(ea.DeliveryTag, false, true);
			}
		};

		channel.BasicConsume(
			queue: _config.QueueName,
			autoAck: false,
			consumer: consumer
		);


		_logger.LogInformation("Subscribed to RabbitMQ queue");
	}

}
