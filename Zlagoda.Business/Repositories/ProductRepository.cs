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
			string query = @"INSERT 
                             INTO Product 
                             (id_product, category_number, product_name, characteristics) 
                             VALUES 
                             (@IdProduct, @CategoryNumber, @ProductName, @Characteristics)";
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
			string query = @"DELETE 
                             FROM Product 
                             WHERE id_product=@IdProduct";
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
			string query = @"SELECT id_product, product_name, characteristics, 
							 C.category_number, category_name
                             FROM Product P
							 INNER JOIN Category C
							 ON P.category_number = C.category_number
                             ORDER BY product_name ASC";
			using (var connection = new SqlConnection(_connectionString))
			{
				var result = await connection.QueryAsync(query);
				List<Product> products = new List<Product>();
				foreach (var resultItem in result)
				{
					products.Add(new Product
                    {
                        id_product = resultItem.id_product,
                        category_number = resultItem.category_number,
                        product_name = resultItem.product_name,
                        characteristics = resultItem.characteristics,
                        category = new Category
                        {
                            category_name = resultItem.category_name,
                            category_number = resultItem.category_number,
                        },
                    });
				}
				return products;
			}
		}

		public async Task<Product> GetProductByIdAsync(int id)
		{
			string query = @"SELECT id_product, product_name, characteristics, 
							 C.category_number, category_name
                             FROM Product P
							 INNER JOIN Category C
							 ON P.category_number = C.category_number
                             WHERE id_product=@IdProduct";
			using (var connection = new SqlConnection(_connectionString))
			{
				var result = await connection.QueryFirstOrDefaultAsync(query, new
				{
					IdProduct = id,
				});
				if (result is null)
				{
					throw new Exception("Product fetching error!");
				}
                var product = new Product
                {
                    id_product = result.id_product,
                    category_number = result.category_number,
                    product_name = result.product_name,
                    characteristics = result.characteristics,
                    category = new Category
                    {
                        category_name = result.category_name,
                        category_number = result.category_number,
                    },
                };
                return product;
			}
		}

		public async Task<IEnumerable<Product>> GetProductsByCategoryOrderedByNameAsync(Category category)
		{
			string query = @"SELECT id_product, product_name, characteristics, 
							 C.category_number, category_name
                             FROM Product P
							 INNER JOIN Category C
							 ON P.category_number = C.category_number 
                             WHERE P.category_number=@CategoryNumber";
			using (var connection = new SqlConnection(_connectionString))
			{
                var result = await connection.QueryAsync(query, new
				{
					CategoryNumber = category.category_number,
				});
                List<Product> products = new List<Product>();
                foreach (var resultItem in result)
                {
                    products.Add(new Product
                    {
                        id_product = resultItem.id_product,
                        category_number = resultItem.category_number,
                        product_name = resultItem.product_name,
                        characteristics = resultItem.characteristics,
                        category = new Category
                        {
                            category_name = resultItem.category_name,
                            category_number = resultItem.category_number,
                        },
                    });
                }
                return products;
            }
		}

		public async Task<IEnumerable<Product>> GetProductsByNameAsync(string productName)
		{
			string query = @"SELECT id_product, product_name, characteristics, 
							 C.category_number, category_name
                             FROM Product P
							 INNER JOIN Category C
							 ON P.category_number = C.category_number
                             WHERE product_name=@ProductName";
			using (var connection = new SqlConnection(_connectionString))
			{
                var result = await connection.QueryAsync(query, new
                {
                    ProductName = productName,
                });
                List<Product> products = new List<Product>();
                foreach (var resultItem in result)
                {
                    products.Add(new Product
                    {
                        id_product = resultItem.id_product,
                        category_number = resultItem.category_number,
                        product_name = resultItem.product_name,
                        characteristics = resultItem.characteristics,
                        category = new Category
                        {
                            category_name = resultItem.category_name,
                            category_number = resultItem.category_number,
                        },
                    });
                }
                return products;
            }
		}

		public async Task<Product> UpdateProductAsync(Product product)
		{
			string query = @"UPDATE Product 
                             SET category_number=@CategoryNumber, product_name=@ProductName, characteristics=@Characteristics 
                             WHERE id_product=@IdProduct";
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
