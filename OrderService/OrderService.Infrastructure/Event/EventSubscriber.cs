using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrderService.Domain.Interface;
using OrderService.Infrastructure.ConfigModel;
using OrderService.Model.Dto;
using OrderService.Model.Enum;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace OrderService.Infrastructure.Event;

public class EventSubscriber
{
	private readonly ILogger<EventSubscriber> _logger;
	private readonly RabbitMqConfig _config;
	private readonly IServiceProvider _serviceProvider;

	public EventSubscriber(ILogger<EventSubscriber> logger, IConfiguration configuration,
		IServiceProvider serviceProvider)
	{
		_logger = logger;
		_config = configuration.GetSection("RabbitMqConfig").Get<RabbitMqConfig>() ??
		          throw new InvalidOperationException("Configuration section 'RabbitMqConfig' is missing or invalid");
		_serviceProvider = serviceProvider;
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

		channel.ExchangeDeclare("payment_exchange", ExchangeType.Topic);

		channel.QueueDeclare(
			_config.QueueName,
			true,
			false,
			false,
			null
		);

		channel.QueueBind(
			_config.QueueName,
			"payment_exchange",
			"payment.status"
		);

		var consumer = new EventingBasicConsumer(channel);
		consumer.Received += async (_, ea) =>
		{
			try
			{
				var body = ea.Body.ToArray();
				var message = Encoding.UTF8.GetString(body);
				_logger.LogInformation($"Received raw message: {message}");

				var options = new JsonSerializerOptions
				{
					Converters = { new JsonStringEnumConverter() },
					NumberHandling = JsonNumberHandling.AllowReadingFromString,
					PropertyNameCaseInsensitive = true
				};

				var paymentEvent = JsonSerializer.Deserialize<PaymentDto>(message, options);
				if (paymentEvent != null)
				{
					_logger.LogInformation($"Deserialized PaymentStatus: {paymentEvent.PaymentStatus}");
					_logger.LogInformation($"PaymentStatus Enum Name: {paymentEvent.PaymentStatus.ToString()}");

					using (var scope = _serviceProvider.CreateScope())
					{
						var orderDomain = scope.ServiceProvider.GetRequiredService<IOrderDomain>();

						var orderResponse = await orderDomain.GetByIdAsync(paymentEvent.OrderId);


						var orderStatus = paymentEvent.PaymentStatus switch
						{
							PaymentStatus.Completed => OrderStatus.Completed,
							PaymentStatus.Failed => OrderStatus.Failed,
							_ => OrderStatus.Pending
						};

						await orderDomain.Update(orderResponse.Id, new UpdateOrderRequest
						{
							OrderStatus = orderStatus,
							CustomerId = orderResponse.CustomerId
						});
					}
				}
			}
			catch (Exception ex)
			{
				_logger.LogError($"Error processing message: {ex.Message}");
				channel.BasicNack(ea.DeliveryTag, false, true);
			}
		};

		channel.BasicConsume(
			_config.QueueName,
			false,
			consumer
		);


		_logger.LogInformation("Subscribed to RabbitMQ queue");
	}
}
