using ProductService.Domain.Interfaces;
using ProductService.Model.Dto;
using ProductService.Model.Exceptions;
using ProductService.Model.Extentions;
using ProductService.Model.Models;
using ProductService.Repository.Interfaces;

namespace ProductService.Domain;

public class ProductDomain : IProductDomain
{
	private readonly IProductRepository _productRepository;

	public ProductDomain(IProductRepository productRepository)
	{
		_productRepository = productRepository;
	}

	public async Task<List<Product>> GetAllAsync()
	{
		return await _productRepository.GetAllAsync();
	}

	public async Task<Product> GetByIdAsync(string id)
	{
		var product = await _productRepository.GetByIdAsync(id);
		if (product == null) throw new NotFoundException("Product not found");

		return product;
	}

	public async Task<Product> AddAsync(ProductRequest productRequest)
	{
		Product product = productRequest.ToProduct();
		if (!string.IsNullOrEmpty(product.Image))
		{
			var imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "images");

			if (!Directory.Exists(imagesDirectory))
			{
				Directory.CreateDirectory(imagesDirectory);
			}

			var base64String = product.Image.Replace("data:image/png;base64,", "")
				.Replace("data:image/jpg;base64,", "");

			try
			{
				var imageBytes = Convert.FromBase64String(base64String);

				var fileExtension = GetImageExtension(imageBytes);

				if (fileExtension == null)
				{
					throw new ArgumentException("Invalid image format. Only PNG and JPG are allowed.");
				}

				var originalFileName = !string.IsNullOrEmpty(productRequest.ImageFileName)
					? Path.GetFileName(productRequest.ImageFileName)
					: Guid.NewGuid().ToString();
				var fileName = $"{originalFileName}.{fileExtension}";
				var filePath = Path.Combine(imagesDirectory, fileName);

				await File.WriteAllBytesAsync(filePath, imageBytes);

				product.Image = fileName;
			}
			catch (FormatException ex)
			{
				throw new ArgumentException("The provided image is not a valid Base64 string.", ex);
			}
		}

		_productRepository.Add(product);

		var savedProduct = await _productRepository.GetByIdAsync(product.Id.ToString())
		                   ?? throw new Exception("Something went wrong after saving product");
		return savedProduct;
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

	public async Task<bool> DeleteAsync(string id)
	{
		var product = await _productRepository.GetByIdAsync(id);
		if (product == null) throw new NotFoundException("Product not found");

		await _productRepository.DeleteAsync(id);
		return true;
	}

	public async Task UpdateAsync(string id, UpdateProductRequest updateProductRequest)
	{
		var product = await _productRepository.GetByIdAsync(id);
		if (product == null) throw new NotFoundException("Product not found");

		product.Name = updateProductRequest.Name;
		product.Image = updateProductRequest.Image;
		product.Brand = updateProductRequest.Brand;
		product.Price = updateProductRequest.Price;
		product.Category = updateProductRequest.Category;

		_productRepository.Update(product);
	}
}
