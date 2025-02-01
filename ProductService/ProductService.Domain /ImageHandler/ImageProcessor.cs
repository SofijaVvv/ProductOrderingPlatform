using ProductService.Domain.Interfaces;

namespace ProductService.Domain.ImageHandler;

public class ImageProcessor : IImageProcessor
{
	public async Task<string> SaveImageAsync(string base64Image, string? imageFileName)
	{
		var imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "images");

		if (!Directory.Exists(imagesDirectory))
		{
			Directory.CreateDirectory(imagesDirectory);
		}

		var base64String = base64Image.Replace("data:image/png;base64,", "")
			.Replace("data:image/jpg;base64,", "");

		try
		{
			var imageBytes = Convert.FromBase64String(base64String);

			var fileExtension = GetImageExtension(imageBytes);

			if (fileExtension == null)
			{
				throw new ArgumentException("Invalid image format. Only PNG and JPG are allowed.");
			}

			var originalFileName = !string.IsNullOrEmpty(imageFileName)
				? Path.GetFileName(imageFileName)
				: Guid.NewGuid().ToString();
			var fileName = $"{originalFileName}.{fileExtension}";
			var filePath = Path.Combine(imagesDirectory, fileName);

			await File.WriteAllBytesAsync(filePath, imageBytes);

			return fileName;
		}
		catch (FormatException ex)
		{
			throw new ArgumentException("The provided image is not a valid Base64 string.", ex);
		}
}

	private string? GetImageExtension(byte[] imageBytes)
	{
		if (imageBytes.Length > 8 && imageBytes[0] == 0x89 && imageBytes[1] == 0x50 &&
		    imageBytes[2] == 0x4E && imageBytes[3] == 0x47 && imageBytes[4] == 0x0D &&
		    imageBytes[5] == 0x0A && imageBytes[6] == 0x1A && imageBytes[7] == 0x0A)
		{
			return "png";
		}

		if (imageBytes.Length > 3 && imageBytes[0] == 0xFF && imageBytes[1] == 0xD8 &&
		    imageBytes[2] == 0xFF)
		{
			return "jpg";
		}

		return null;
	}
}
