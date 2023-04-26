using Dapper;
using System.Data.SqlClient;
using Zlagoda.Business.Entities;
using Zlagoda.Business.Interfaces;

namespace Zlagoda.Business.Repositories
{
	public class CustomerCardRepository : ICustomerCardRepository
	{
		private readonly string _connectionString;

		public CustomerCardRepository(string connectionString)
		{
			_connectionString = connectionString;
		}

		public async Task<CustomerCard> CreateCustomerCardAsync(CustomerCard customerCard)
		{
			string query = @"INSERT 
							 INTO Customer_Card 
							 (card_number, cust_surname, cust_name, cust_patronymic, phone_number, city, street, zip_code, [percent]) 
							 VALUES 
							 (@CardNumber, @CustSurname, @CustName, @CustPatronymic, @PhoneNumber, @City, @Street, @ZipCode, @Percent)";
			using (var connection = new SqlConnection(_connectionString))
			{
				int affectedRows = await connection.ExecuteAsync(query, new
				{
					CardNumber = customerCard.card_number,
					CustSurname = customerCard.cust_surname,
					CustName = customerCard.cust_name,
					CustPatronymic = customerCard.cust_patronymic,
					PhoneNumber = customerCard.phone_number,
					City = customerCard.city,
					Street = customerCard.street,
					ZipCode = customerCard.zip_code,
					Percent = customerCard.percent,
				});
				if (affectedRows == 0)
				{
					throw new Exception("CustomerCard creation error!");
				}
				return customerCard;
			}
		}

		public async Task<CustomerCard> DeleteCustomerCardAsync(CustomerCard customerCard)
		{
			string query = @"DELETE 
							 FROM Customer_Card 
							 WHERE card_number=@CardNumber";
			using (var connection = new SqlConnection(_connectionString))
			{
				int affectedRows = await connection.ExecuteAsync(query, new
				{
					CardNumber = customerCard.card_number,
				});
				if (affectedRows == 0)
				{
					throw new Exception("CustomerCard deletion error!");
				}
				return customerCard;
			}
		}

		public async Task<IEnumerable<CustomerCard>> GetAllCustomerCardsOrderedBySurnameAsync()
		{
			string query = @"SELECT * 
							 FROM Customer_Card 
							 ORDER BY cust_surname ASC";
			using (var connection = new SqlConnection(_connectionString))
			{
				return await connection.QueryAsync<CustomerCard>(query);
			}
		}

		public async Task<IEnumerable<CustomerCard>> GetAllCustomerCardsWithCertainDiscountPercentOrderedBySurnameAsync(int percent)
		{
			string query = @"SELECT * 
							 FROM Customer_Card 
							 WHERE [percent]=@Percent 
							 ORDER BY cust_surname ASC";
			using (var connection = new SqlConnection(_connectionString))
			{
				return await connection.QueryAsync<CustomerCard>(query, new
				{
					Percent = percent,
				});
			}
		}

		public async Task<CustomerCard> GetCustomerCardByNumberAsync(string number)
		{
			string query = @"SELECT * 
							 FROM Customer_Card 
							 WHERE card_number=@CardNumber";
			using (var connection = new SqlConnection(_connectionString))
			{
				var customerCard = await connection.QueryFirstOrDefaultAsync<CustomerCard>(query, new
				{
					CardNumber = number,
				});
				if (customerCard is null)
				{
					throw new Exception("CustomerCard fetching error!");
				}
				return customerCard;
			}
		}

		public async Task<IEnumerable<CustomerCard>> GetCustomerCardsBySurnameAsync(string surname)
		{
			string query = @"SELECT * 
							 FROM Customer_Card 
							 WHERE cust_surname=@CustSurname";
			using (var connection = new SqlConnection(_connectionString))
			{
				return await connection.QueryAsync<CustomerCard>(query, new
				{
					CustSurname = surname,
				});
			}
		}

		public async Task<CustomerCard> UpdateCustomerCardAsync(CustomerCard customerCard)
		{
			string query = @"UPDATE Customer_Card 
							 SET cust_surname=@CustSurname, cust_name=@CustName, cust_patronymic=@CustPatronymic, 
							 phone_number=@PhoneNumber, city=@City, street=@Street, zip_code=@ZipCode, 
							 [percent]=@Percent 
							 WHERE card_number=@CardNumber";
			using (var connection = new SqlConnection(_connectionString))
			{
				int affectedRows = await connection.ExecuteAsync(query, new
				{
					CustSurname = customerCard.cust_surname,
					CustName = customerCard.cust_name,
					CustPatronymic = customerCard.cust_patronymic,
					PhoneNumber = customerCard.phone_number,
					City = customerCard.city,
					Street = customerCard.street,
					ZipCode = customerCard.zip_code,
					Percent = customerCard.percent,
					CardNumber = customerCard.card_number,
				});
				if (affectedRows == 0)
				{
					throw new Exception("CustomerCard updating error!");
				}
				return customerCard;
			}
		}
	}
}
