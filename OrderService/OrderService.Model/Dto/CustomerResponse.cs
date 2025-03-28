namespace OrderService.Model.Dto;

public class CustomerResponse
{
	public int Id { get; set; }

	public string Name { get; set; }

	public string Phone { get; set; }

	public string Email { get; set; }

	public string Address { get; set; }

	public DateTime CreatedAt { get; set; }
}
