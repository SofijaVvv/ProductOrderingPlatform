namespace ProductService.Domain.Interfaces;

public interface IImageFileHandler
{
	Task<string> StoreImageFromBase64Async(string base64Image, string? imageFileName);
}
