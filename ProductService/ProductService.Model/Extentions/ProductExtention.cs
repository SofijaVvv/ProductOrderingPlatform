using ProductService.Model.Dto;
using ProductService.Model.Models;

namespace ProductService.Model.Extentions;

public static class ProductExtention
{
	public static Product ToProduct(this ProductRequest productRequest)
	{
		return new Product
		{
			Name = productRequest.Name,
			Image = productRequest.Image,
			Brand = productRequest.Brand,
			Price = productRequest.Price,
			Category = productRequest.Category
		};
	}

	public static ProductResponse ToResponse(this Product response)
	{
		return new ProductResponse
		{
			Id = response.Id,
			Name = response.Name,
			Image = response.Image,
			Brand = response.Brand,
			Price = response.Price,
			Category = response.Category
		};
	}

	public static List<ProductResponse> ToResponse(this List<Product> response)
	{
		return response.Select(product => product.ToResponse()).ToList();
	}

}
