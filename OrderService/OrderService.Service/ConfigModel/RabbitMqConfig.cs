namespace OrderService.Service.ConfigModel;

public class RabbitMqConfig
{
	public string Host { get; set; }
	public string QueueName { get; set; }
	public string UserName { get; set; }
	public string Password { get; set; }
}
