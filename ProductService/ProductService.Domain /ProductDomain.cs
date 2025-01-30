using ProductService.Domain.ImageHandler;
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
	private readonly IImageProcessor _imageProcessor;

	public ProductDomain(IProductRepository productRepository,IImageProcessor imageProcessor)
	{
		_productRepository = productRepository;
		_imageProcessor = imageProcessor;
	}

	public async Task<List<ProductResponse>> GetAllAsync()
	{
		var products = await _productRepository.GetAllAsync();
		return products.ToResponse();
	}

	public async Task<ProductResponse> GetByIdAsync(string id)
	{
		var product = await _productRepository.GetByIdAsync(id);
		if (product == null) throw new NotFoundException("Product not found");

		return product.ToResponse();
	}

	public async Task<ProductResponse> AddAsync(ProductRequest productRequest)
	{
		Product product = productRequest.ToProduct();
		if (!string.IsNullOrEmpty(product.Image))
		{
			product.Image = await _imageProcessor.ProcessImageAsync(product.Image, productRequest.ImageFileName);
		}

		await _productRepository.AddAsync(product);

		return product.ToResponse();
	}

	public async Task DeleteAsync(string id)
	{
		var product = await _productRepository.GetByIdAsync(id);
		if (product == null) throw new NotFoundException("Product not found");

		await _productRepository.DeleteAsync(id);
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

		await _productRepository.UpdateAsync(product);
	}
}
