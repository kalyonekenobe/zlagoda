using Zlagoda.Business.Entities;

namespace Zlagoda.Business.Interfaces
{
	public interface ICustomerCardRepository
	{
		Task<CustomerCard> CreateCustomerCardAsync(CustomerCard customerCard);
		Task<CustomerCard> UpdateCustomerCardAsync(CustomerCard customerCard);
		Task<CustomerCard> DeleteCustomerCardAsync(CustomerCard customerCard);
		Task<IEnumerable<CustomerCard>> GetAllCustomerCardsOrderedBySurnameAsync();
		Task<IEnumerable<CustomerCard>> GetAllCustomerCardsWithCertainDiscountPercentOrderedBySurnameAsync(int percent);
		Task<IEnumerable<CustomerCard>> GetCustomerCardsBySurnameAsync(string surname);
		Task<CustomerCard> GetCustomerCardByNumberAsync(string number);
	}
}
