using Zlagoda.Business.Entities;

namespace Zlagoda.Business.Interfaces
{
	public interface IEmployeeRepository
	{
		Task<Employee> CreateEmployeeAsync(Employee employee);	
		Task<Employee> UpdateEmployeeAsync(Employee employee);
		Task<Employee> DeleteEmployeeAsync(Employee employee);
		Task<IEnumerable<Employee>> GetAllEmployeesOrderedBySurnameAsync();
		Task<IEnumerable<Employee>> GetAllEmployeesCashiersOrderedBySurnameAsync();
		Task<dynamic> GetEmployeePhoneAndAddressBySurnameAsync(string surname);
		Task<Employee> GetEmployeeByIdAsync(string id);
		Task<Employee> GetEmployeeByPhoneAsync(string phone);
	}
}
