using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace OrderService.Service.Services;

public class EventBusSubscriberService : IHostedService
{

	private readonly ILogger<EventBusSubscriberService> _logger;
	private readonly EventSubscriberService _eventSubscriber;

	public EventBusSubscriberService(ILogger<EventBusSubscriberService> logger, EventSubscriberService eventSubscriber)
	{
		_logger = logger;
		_eventSubscriber = eventSubscriber;
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		_logger.LogInformation("Starting EventBusSubscriberService");
		_eventSubscriber.Subscribe();
		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		_logger.LogInformation("Stopping EventBusSubscriberService");
		return Task.CompletedTask;
	}
}
