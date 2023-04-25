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
		Task<dynamic> GetStoreProductPriceQuantityNameAndCharacteristicsByUPCAsync(string upc);
		Task<IEnumerable<StoreProduct>> GetAllPromotionalStoreProductsOrderedByQuantityThenByNameAsync();
		Task<IEnumerable<StoreProduct>> GetAllPromotionalStoreProductsOrderedByNameThenByQuantityAsync();
		Task<IEnumerable<StoreProduct>> GetAllNonPromotionalStoreProductsOrderedByQuantityThenByNameAsync();
		Task<IEnumerable<StoreProduct>> GetAllNonPromotionalStoreProductsOrderedByNameThenByQuantityAsync();
		Task<dynamic> GetStoreProductPriceAndQuantityByUPCAsync(string upc);
		Task<StoreProduct> GetStoreProductByUPCAsync(string upc);
		Task<StoreProduct?> GetStoreProductNonPromotionalByUPCAsync(string upc);
	}
}
