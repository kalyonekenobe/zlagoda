using Dapper;
using System.Data.SqlClient;
using Zlagoda.Business.Entities;
using Zlagoda.Business.Interfaces;

namespace Zlagoda.Business.Repositories
{
	public class ProductRepository : IProductRepository
	{
		private readonly string _connectionString;

		public ProductRepository(string connectionString)
		{
			_connectionString = connectionString;
		}

		public async Task<Product> CreateProductAsync(Product product)
		{
			string query = @"INSERT INTO Product (id_product, category_number, product_name, characteristics) VALUES (@IdProduct, @CategoryNumber, @ProductName, @Characteristics)";
			using (var connection = new SqlConnection(_connectionString))
			{
				int affectedRows = await connection.ExecuteAsync(query, new
				{
					IdProduct = product.id_product,
					CategoryNumber = product.category_number,
					ProductName = product.product_name,
					Characteristics = product.characteristics,
				});
				if (affectedRows == 0)
				{
					throw new Exception("Product creation error!");
				}
				return product;
			}
		}

		public async Task<Product> DeleteProductAsync(Product product)
		{
			string query = @"DELETE FROM Product WHERE id_product=@IdProduct";
			using (var connection = new SqlConnection(_connectionString))
			{
				int affectedRows = await connection.ExecuteAsync(query, new
				{
					IdProduct = product.id_product,
				});
				if (affectedRows == 0)
				{
					throw new Exception("Product deletion error!");
				}
				return product;
			}
		}

		public async Task<IEnumerable<Product>> GetAllProductsOrderedByNameAsync()
		{
			string query = @"SELECT * FROM Product ORDER BY product_name ASC";
			using (var connection = new SqlConnection(_connectionString))
			{
				return await connection.QueryAsync<Product>(query);
			}
		}

		public async Task<Product> GetProductByIdAsync(int id)
		{
			string query = @"SELECT * FROM Product WHERE id_product=@IdProduct";
			using (var connection = new SqlConnection(_connectionString))
			{
				var product = await connection.QueryFirstOrDefaultAsync<Product>(query, new
				{
					IdProduct = id,
				});
				if (product is null)
				{
					throw new Exception("Product fetching error!");
				}
				return product;
			}
		}

		public async Task<IEnumerable<Product>> GetProductsByCategoryOrderedByNameAsync(Category category)
		{
			string query = @"SELECT * FROM Product WHERE category_number=@CategoryNumber";
			using (var connection = new SqlConnection(_connectionString))
			{
				return await connection.QueryAsync<Product>(query, new
				{
					CategoryNumber = category.category_number,
				});
			}
		}

		public async Task<IEnumerable<Product>> GetProductsByNameAsync(string productName)
		{
			string query = @"SELECT * FROM Product WHERE product_name=@ProductName";
			using (var connection = new SqlConnection(_connectionString))
			{
				return await connection.QueryAsync<Product>(query, new
				{
					ProductName = productName,
				});
			}
		}

		public async Task<Product> UpdateProductAsync(Product product)
		{
			string query = @"UPDATE Product SET category_number=@CategoryNumber, product_name=@ProductName, characteristics=@Characteristics WHERE id_product=@IdProduct";
			using (var connection = new SqlConnection(_connectionString))
			{
				int affectedRows = await connection.ExecuteAsync(query, new
				{
					CategoryNumber = product.category_number,
					ProductName = product.product_name,
					Characteristics = product.characteristics,
					IdProduct = product.id_product,
				});
				if (affectedRows == 0)
				{
					throw new Exception("Product updating error!");
				}
				return product;
			}
		}
	}
}
