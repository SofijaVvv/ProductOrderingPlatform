namespace PaymentService.Infrastructure.Interface;

public interface IEventPublisher
{
	void PublishAsync(string topic, object message);
}
