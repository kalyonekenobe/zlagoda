using Zlagoda.Business.Entities;

namespace Zlagoda.Business.Interfaces
{
	public interface ICategoryRepository
	{
		Task<Category> CreateCategoryAsync(Category category);
		Task<Category> UpdateCategoryAsync(Category category);
		Task<Category> DeleteCategoryAsync(Category category);
		Task<IEnumerable<Category>> GetAllCategoriesOrderedByNameAsync();
		Task<IEnumerable<Category>> GetAllCategoriesWithStoreProductsNumberAsync();
		Task<Category> GetCategoryByNumberAsync(int number);
	}
}
