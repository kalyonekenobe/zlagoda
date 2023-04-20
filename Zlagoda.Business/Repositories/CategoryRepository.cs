using Dapper;
using System.Data.SqlClient;
using Zlagoda.Business.Entities;
using Zlagoda.Business.Interfaces;

namespace Zlagoda.Business.Repositories
{
	public class CategoryRepository : ICategoryRepository
	{
		private readonly string _connectionString;

		public CategoryRepository(string connectionString)
		{
			_connectionString = connectionString;
        }

		public async Task<Category> CreateCategoryAsync(Category category)
		{
			string query = @"INSERT INTO Category (category_number, category_name) VALUES (@CategoryNumber, @CategoryName)";
			using (var connection = new SqlConnection(_connectionString))
			{
				int affectedRows = await connection.ExecuteAsync(query, new
				{
					CategoryNumber = category.category_number,
					CategoryName = category.category_name,
				});
				if (affectedRows == 0)
				{
					throw new Exception("Category creation error!");
				}
				return category;
			}
		}

		public async Task<Category> DeleteCategoryAsync(Category category)
		{
			string query = @"DELETE FROM Category WHERE category_number=@CategoryNumber";
			using (var connection = new SqlConnection(_connectionString))
			{
				int affectedRows = await connection.ExecuteAsync(query, new
				{
					CategoryNumber = category.category_number,
				});
				if (affectedRows == 0)
				{
					throw new Exception("Category deletion error!");
				}
				return category;
			}
		}

		public async Task<IEnumerable<Category>> GetAllCategoriesOrderedByNameAsync()
		{
			string query = @"SELECT * FROM Category ORDER BY category_name ASC";
			using (var connection = new SqlConnection(_connectionString))
			{
				return await connection.QueryAsync<Category>(query);
			}
		}

		public async Task<Category> GetCategoryByNumberAsync(int number)
		{
			string query = @"SELECT * FROM Category WHERE category_number=@CategoryNumber";
			using (var connection = new SqlConnection(_connectionString))
			{
				var category = await connection.QueryFirstOrDefaultAsync<Category>(query, new
				{
					CategoryNumber = number
				});
				if (category is null)
				{
					throw new Exception("Category fetching error!");
				}
				return category;
			}
		}

		public async Task<Category> UpdateCategoryAsync(Category category)
		{
			string query = @"UPDATE Category SET category_name=@CategoryName WHERE category_number=@CategoryNumber";
			using (var connection = new SqlConnection(_connectionString))
			{
				int affectedRows = await connection.ExecuteAsync(query, new
				{
					CategoryName = category.category_name,
					CategoryNumber = category.category_number,
				});
				if (affectedRows == 0)
				{
					throw new Exception("Category updating error!");
				}
				return category;
			}
		}
	}
}
