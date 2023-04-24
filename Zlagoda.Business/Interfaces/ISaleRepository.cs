using Zlagoda.Business.Entities;

namespace Zlagoda.Business.Interfaces
{
	public interface ISaleRepository
	{
		Task<Sale> CreateSaleAsync(Sale sale);
		Task<int> GetTotalQuantityOfStoreProductsSoldByCertainCashierDuringPeriodAsync(Employee employee, DateTime startDate, DateTime endDate);
		Task<int> GetTotalQuantityOfStoreProductsSoldByAllCashiersDuringPeriodAsync(DateTime startDate, DateTime endDate);
		Task<int> GetTotalQuantityOfStoreProductSoldDuringPeriodAsync(StoreProduct storeProduct, DateTime startDate, DateTime endDate);
	}
}
