using Zlagoda.Business.Entities;

namespace Zlagoda.Business.Interfaces
{
	public interface IProductRepository
	{
		Task<Product> CreateProductAsync(Product product);
		Task<Product> UpdateProductAsync(Product product);
		Task<Product> DeleteProductAsync(Product product);
		Task<IEnumerable<Product>> GetAllProductsOrderedByNameAsync();
		Task<IEnumerable<Product>> GetProductsByCategoryOrderedByNameAsync(Category category);
		Task<IEnumerable<Product>> GetProductsByNameAsync(string productName);
		Task<Product> GetProductByIdAsync(int id);
	}
}
