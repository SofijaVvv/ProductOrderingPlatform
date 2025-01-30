namespace ProductService.Domain.Interfaces;

public interface IImageProcessor
{
	Task<string> ProcessImageAsync(string base64Image, string? imageFileName);
}
