using Dapper;
using System.Data.SqlClient;
using Zlagoda.Business.Entities;
using Zlagoda.Business.Interfaces;

namespace Zlagoda.Business.Repositories
{
	public class SaleRepository : ISaleRepository
	{
		private readonly string _connectionString;

		public SaleRepository(string connectionString)
		{
			_connectionString = connectionString;
		}

        public async Task<Sale> CreateSaleAsync(Sale sale)
        {
			string query = @"INSERT 
							 INTO Sale
						     (UPC, check_number, product_number, selling_price)
						     VALUES
							 (@UPC, @CheckNumber, @ProductNumber, @SellingPrice)";
			using (var connection = new SqlConnection(_connectionString))
			{
				int affectedRows = await connection.ExecuteAsync(query, new
				{
					UPC = sale.UPC,
					CheckNumber = sale.check_number,
					ProductNumber = sale.product_number,
					SellingPrice = sale.selling_price,
				});

				if (affectedRows == 0)
				{
					throw new Exception("Check sale creation error!");
				}

				return sale;
			}
        }

        public async Task<int> GetTotalQuantityOfStoreProductSoldDuringPeriodAsync(StoreProduct storeProduct, DateTime startDate, DateTime endDate)
		{
			string query = @"SELECT SUM(product_number) 
							 FROM Sale 
						     WHERE UPC=@UPC AND check_number IN (SELECT check_number 
													FROM [Check] 
													WHERE print_date BETWEEN @StartDate AND @EndDate)";
			using (var connection = new SqlConnection(_connectionString))
			{
				return await connection.ExecuteScalarAsync<int>(query, new
				{
					UPC = storeProduct.UPC,
					StartDate = startDate,
					EndDate = endDate,
				});
			}
		}

		public async Task<int> GetTotalQuantityOfStoreProductsSoldByAllCashiersDuringPeriodAsync(DateTime startDate, DateTime endDate)
		{
			string query = @"SELECT SUM(product_number) 
							 FROM Sale 
							 WHERE check_number IN (SELECT check_number 
													FROM [Check] 
													WHERE print_date BETWEEN @StartDate AND @EndDate)";
			using (var connection = new SqlConnection(_connectionString))
			{
				return await connection.ExecuteScalarAsync<int>(query, new
				{
					StartDate = startDate,
					EndDate = endDate,
				});
			}
		}

		public async Task<int> GetTotalQuantityOfStoreProductsSoldByCertainCashierDuringPeriodAsync(Employee employee, DateTime startDate, DateTime endDate)
		{
			string query = @"SELECT SUM(product_number) 
							 FROM Sale 
							 WHERE check_number IN (SELECT check_number 
													FROM [Check] 
													WHERE id_employee=@IdEmployee 
													AND print_date BETWEEN @StartDate AND @EndDate)";
			using (var connection = new SqlConnection(_connectionString))
			{
				return await connection.ExecuteScalarAsync<int>(query, new
				{
					IdEmployee = employee.id_employee,
					StartDate = startDate,
					EndDate = endDate,
				});
			}
		}
	}
}
