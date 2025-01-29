using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OrderService.Model.Dto;
using OrderService.Service.ConfigModel;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace OrderService.Service.Services;

public class EventSubscriberService
{
	private readonly ILogger<EventSubscriberService> _logger;
	private readonly RabbitMqConfig _config;

	public EventSubscriberService(ILogger<EventSubscriberService> logger, IConfiguration configuration)
	{
		_logger = logger;
		_config = configuration.GetSection("RabbitMqConfig").Get<RabbitMqConfig>() ?? throw new InvalidOperationException("RabbitMqConfig is missing");
	}

	public void Subscribe()
	{
		var factory = new ConnectionFactory()
		{
			HostName = _config.Host,
			UserName = _config.UserName,
			Password = _config.Password
		};

		var connection = factory.CreateConnection();
		var channel = connection.CreateModel();

		channel.QueueDeclare(
			queue: _config.QueueName,
			durable: true,
			exclusive: false,
			autoDelete: false,
			arguments: null
		);

		var consumer = new EventingBasicConsumer(channel);
		consumer.Received += (model, ea) =>
		{
			var body = ea.Body.ToArray();
			var message = Encoding.UTF8.GetString(body);
			JsonSerializer.Deserialize<PaymentDto>(message);

			_logger.LogInformation($"Received message: {message}");
		};

		channel.BasicConsume(
			queue: _config.QueueName,
			autoAck: true,
			consumer: consumer
		);

		_logger.LogInformation("Subscribed to RabbitMQ queue");
	}
}
