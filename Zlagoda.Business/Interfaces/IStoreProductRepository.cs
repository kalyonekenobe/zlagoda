using Zlagoda.Business.Entities;

namespace Zlagoda.Business.Interfaces
{
	public interface IStoreProductRepository
	{
		Task<StoreProduct> CreateStoreProductAsync(StoreProduct storeProduct);
		Task<StoreProduct> UpdateStoreProductAsync(StoreProduct storeProduct);
		Task<StoreProduct> DeleteStoreProductAsync(StoreProduct storeProduct);
		Task<IEnumerable<StoreProduct>> GetAllStoreProductsOrderedByNameAsync();
		Task<IEnumerable<StoreProduct>> GetAllStoreProductsOrderedByQuantityAsync();
		Task<object> GetStoreProductPriceQuantityNameAndCharacteristicsByUPCAsync(string upc);
		Task<IEnumerable<StoreProduct>> GetAllPromotionalStoreProductsOrderedByQuantityThenByNameAsync();
		Task<IEnumerable<StoreProduct>> GetAllNonPromotionalStoreProductsOrderedByQuantityThenByNameAsync();
		Task<object> GetStoreProductPriceAndQuantityByUPCAsync(string upc);
		Task<StoreProduct> GetStoreProductByUPCAsync(string upc);
	}
}
