namespace OrderService.Model;

public class ErrorModel
{
	public int StatusCode { get; set; }
	public string Message { get; set; }
	public string? Details { get; set; }
}
