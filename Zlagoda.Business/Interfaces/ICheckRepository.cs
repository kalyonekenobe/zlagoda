using Zlagoda.Business.Entities;

namespace Zlagoda.Business.Interfaces
{
	public interface ICheckRepository
	{
		Task<Check> CreateCheckAsync(Check check);
		Task<Check> DeleteCheckAsync(Check check);
		Task<IEnumerable<Check>> GetAllChecksAsync();
		Task<IEnumerable<Check>> GetAllChecksCreatedByCertainCashierDuringPeriodAsync(Employee employee, DateTime startDate, DateTime endDate);
		Task<IEnumerable<Check>> GetAllChecksCreatedByCertainCashierOnDateAsync(Employee employee, DateTime date);
		Task<IEnumerable<Check>> GetAllChecksCreatedByAllCashiersDuringPeriodAsync(DateTime startDate, DateTime endDate);
		Task<Check> GetCheckByNumberAsync(string number);
	}
}
