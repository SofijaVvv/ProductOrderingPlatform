namespace PaymentService.Domain.Interface;

public interface IEventPublisher
{
	void PublishAsync(string topic, object message);
}
