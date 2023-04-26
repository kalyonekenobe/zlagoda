using Dapper;
using System.Data.SqlClient;
using Zlagoda.Business.Entities;
using Zlagoda.Business.Interfaces;

namespace Zlagoda.Business.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly string _connectionString;

        public EmployeeRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<Employee> CreateEmployeeAsync(Employee employee)
        {
            string query = @"INSERT INTO Employee 
                             (id_employee, empl_password, empl_surname, empl_name, empl_patronymic, 
                              empl_role, salary, date_of_birth, date_of_start, phone_number, 
                              city, street, zip_code) 
                             VALUES 
                             (@IdEmployee, @EmplPassword, @EmplSurname, @EmplName, @EmplPatronymic,
                              @EmplRole, @Salary, @DateOfBirth, @DateOfStart, @PhoneNumber,
                              @City, @Street, @ZipCode)";
            using (var connection = new SqlConnection(_connectionString))
            {
                int affectedRows = await connection.ExecuteAsync(query, new
                {
                    IdEmployee = employee.id_employee,
                    EmplPassword = employee.empl_password,
                    EmplSurname = employee.empl_surname,
                    EmplName = employee.empl_name,
                    EmplPatronymic = employee.empl_patronymic,
                    EmplRole = employee.empl_role,
                    Salary = employee.salary,
                    DateOfBirth = employee.date_of_birth,
                    DateOfStart = employee.date_of_start,
                    PhoneNumber = employee.phone_number,
                    City = employee.city,
                    Street = employee.street,
                    ZipCode = employee.zip_code,
                });

                employee.empl_password = string.Empty;

                if (affectedRows == 0)
                {
                    throw new Exception("Employee creation error!");
                }
                return employee;
            }
        }

        public async Task<Employee> DeleteEmployeeAsync(Employee employee)
        {
            string query = @"DELETE 
                             FROM Employee 
                             WHERE id_employee=@IdEmployee";
            using (var connection = new SqlConnection(_connectionString))
            {
                int affectedRows = await connection.ExecuteAsync(query, new
                {
                    IdEmployee = employee.id_employee,
                });

                if (affectedRows == 0)
                {
                    throw new Exception("Employee deletion error!");
                }

                return employee;
            }
        }

        public async Task<IEnumerable<dynamic>> GetAllCashiersWhoCreatedMoreThanQuantityChecksWithoutPromoStoreProductsAsync(int quantity)
        {
            string query = @"SELECT E.empl_surname, E.empl_name, E.empl_patronymic
                             FROM Employee E
                             WHERE EXISTS (SELECT check_number, COUNT(check_number) AS check_amount
	                                       FROM [Check] H
	                                       GROUP BY check_number
	                                       HAVING (SELECT COUNT(check_number) as check_amount 
                                                   FROM [Check] ССС
                                                   WHERE id_employee=E.id_employee
                                                   AND NOT EXISTS(SELECT UPC
			                                                      FROM Sale
				                                                  WHERE check_number=ССС.check_number 
                                                                  AND UPC IN(SELECT UPC
	                                                                         FROM Store_Product
	                                                                         WHERE promotional_product=1
                                                                            )  
                                                                 )
                                                  )>@Amount
                                          )";
            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QueryAsync(query, new
                {
                    Amount = quantity
                });
            }
        }

        public async Task<IEnumerable<dynamic>> GetAllCashiersWhoServedAllClientsServedByCashiersWithSurnameAsync(string surname)
        {
            string query = @"SELECT empl_surname, empl_name, empl_patronymic 
                             FROM Employee Emp 
                             WHERE NOT EXISTS(SELECT card_number,E.id_employee 
                                              FROM [Check]
                                              INNER JOIN Employee E 
                                              ON E.id_employee=[Check].id_employee 
                                              WHERE E.empl_surname=@EmplSurname            
                                              AND card_number NOT IN(SELECT card_number 
                                                                     FROM [Check]  
                                                                     WHERE Emp.id_employee=id_employee 
                                                                    ) 
                                             ) 
                             AND EXISTS(SELECT * 
                                        FROM [Check]  
                                        INNER JOIN Employee 
                                        ON [Check].id_employee=Employee.id_employee 
                                        WHERE Employee.empl_surname=@EmplSurname 
                                       )";
            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QueryAsync(query, new
                {
                    EmplSurname = surname,
                });
            }
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesCashiersOrderedBySurnameAsync()
        {
            string query = @"SELECT *
                             FROM Employee
                             WHERE empl_role='Cashier'
                             ORDER BY empl_surname ASC";
            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QueryAsync<Employee>(query);
            }
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesOrderedBySurnameAsync()
        {
            string query = @"SELECT *
                             FROM Employee
                             ORDER BY empl_surname ASC";
            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QueryAsync<Employee>(query);
            }
        }

        public async Task<Employee> GetEmployeeByIdAsync(string id)
        {
            string query = @"SELECT * 
                             FROM Employee 
                             WHERE id_employee=@IdEmployee";
            using (var connection = new SqlConnection(_connectionString))
            {
                var employee = await connection.QueryFirstOrDefaultAsync<Employee>(query, new
                {
                    IdEmployee = id,
                });

                if (employee is null)
                {
                    throw new Exception("Error when fetching employee by id!");
                }

                return employee;
            }
        }

        public async Task<Employee> GetEmployeeByPhoneAsync(string phone)
        {
            string query = @"SELECT * 
                             FROM Employee 
                             WHERE phone_number=@PhoneNumber";
            using (var connection = new SqlConnection(_connectionString))
            {
                var employee = await connection.QueryFirstOrDefaultAsync<Employee>(query, new
                {
                    PhoneNumber = phone,
                });

                if (employee is null)
                {
                    throw new Exception("Error when fetching employee by phone!");
                }

                return employee;
            }
        }

        public async Task<IEnumerable<dynamic>> GetEmployeesPhoneAndAddressBySurnameAsync(string surname)
        {
            string query = @"SELECT phone_number, city, street, zip_code, 
                             empl_surname, empl_name, empl_patronymic
                             FROM Employee 
                             WHERE empl_surname=@EmplSurname";
            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QueryAsync(query, new
                {
                    EmplSurname = surname,
                });

                if (result.Count() == 0)
                {
                    throw new Exception("Error when fetching employee phone and address by surname!");
                }

                return result;
            }
        }

        public async Task<Employee> UpdateEmployeeAsync(Employee employee)
        {
            string changePassword = employee.empl_password is not null ? "empl_password=@EmplPassword," : string.Empty;
            string query = $@"UPDATE Employee 
                             SET {changePassword} empl_surname=@EmplSurname, 
                             empl_name=@EmplName, empl_patronymic=@EmplPatronymic, 
                             empl_role=@EmplRole, salary=@Salary, date_of_birth=@DateOfBirth, 
                             date_of_start=@DateOfStart, phone_number=@PhoneNumber, 
                             city=@City, street=@Street, zip_code=@ZipCode 
                             WHERE id_employee=@IdEmployee";
            using (var connection = new SqlConnection(_connectionString))
            {
                int affectedRows = await connection.ExecuteAsync(query, new
                {
                    EmplPassword = employee.empl_password,
                    EmplSurname = employee.empl_surname,
                    EmplName = employee.empl_name,
                    EmplPatronymic = employee.empl_patronymic,
                    EmplRole = employee.empl_role,
                    Salary = employee.salary,
                    DateOfBirth = employee.date_of_birth,
                    DateOfStart = employee.date_of_start,
                    PhoneNumber = employee.phone_number,
                    City = employee.city,
                    Street = employee.street,
                    ZipCode = employee.zip_code,
                    IdEmployee = employee.id_employee,
                });

                employee.empl_password = string.Empty;

                if (affectedRows == 0)
                {
                    throw new Exception("Employee updating error!");
                }
                return employee;
            }
        }
    }
}
