using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrderService.Infrastructure.Event;

namespace OrderService.Infrastructure;

public class EventBusSubscriberService : IHostedService
{

	private readonly ILogger<EventBusSubscriberService> _logger;
	private readonly EventSubscriber _eventSubscriber;

	public EventBusSubscriberService(ILogger<EventBusSubscriberService> logger, EventSubscriber eventSubscriber)
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
