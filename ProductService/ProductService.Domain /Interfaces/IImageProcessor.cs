namespace ProductService.Domain.Interfaces;

public interface IImageProcessor
{
	Task<string> SaveImageAsync(string base64Image, string? imageFileName);
}
