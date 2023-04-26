using Dapper;
using System.Data.SqlClient;
using Zlagoda.Business.Entities;
using Zlagoda.Business.Interfaces;

namespace Zlagoda.Business.Repositories
{
    public class CheckRepository : ICheckRepository
    {
        private readonly string _connectionString;

        public CheckRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<Check> CreateCheckAsync(Check check)
        {
            string query = @"INSERT 
                             INTO [Check]
                             (check_number, id_employee, card_number, print_date, sum_total, vat)
                             VALUES 
                             (@CheckNumber, @IdEmployee, @CardNumber, @PrintDate, @SumTotal, @Vat)";
            using (var connection = new SqlConnection(_connectionString))
            {
                int affectedRows = await connection.ExecuteAsync(query, new
                {
                    CheckNumber = check.check_number,
                    IdEmployee = check.id_employee,
                    CardNumber = check.card_number,
                    PrintDate = check.print_date,
                    SumTotal = check.sum_total,
                    Vat = check.vat,
                });

                if (affectedRows == 0)
                {
                    throw new Exception("Check creation error!");
                }

                return check;
            }
        }

        public async Task<Check> DeleteCheckAsync(Check check)
        {
            string query = @"DELETE
                             FROM [Check]
                             WHERE check_number=@CheckNumber";
            using (var connection = new SqlConnection(_connectionString))
            {
                int affectedRows = await connection.ExecuteAsync(query, new
                {
                    CheckNumber = check.check_number,
                });

                if (affectedRows == 0)
                {
                    throw new Exception("Check deletion error!");
                }

                return check;
            }
        }

        public async Task<IEnumerable<Check>> GetAllChecksAsync()
        {
            string query = @"SELECT C.*,
                             S.UPC, S.product_number, S.selling_price as store_product_total_price,
                             SP.UPC_prom, SP.id_product, SP.selling_price as store_product_price,
                             SP.products_number, SP.promotional_product, P.*, E.*
                             FROM [Check] C
                             INNER JOIN Sale S
                             ON C.check_number=S.check_number
                             INNER JOIN Store_Product SP
                             ON S.UPC=SP.UPC
                             INNER JOIN Product P
                             ON P.id_product=SP.id_product
                             INNER JOIN Employee E
                             ON C.id_employee=E.id_employee
                             ORDER BY C.check_number";
            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QueryAsync(query);
                List<Check> checks = new List<Check>();
                string previousCheckId = "";
                foreach (var item in result)
                {
                    if (item.check_number != previousCheckId)
                    {
                        checks.Add(new Check());
                        previousCheckId = item.check_number;
                    }

                    checks[checks.Count - 1].check_number = item.check_number;
                    checks[checks.Count - 1].id_employee = item.id_employee;
                    checks[checks.Count - 1].card_number = item.card_number;
                    checks[checks.Count - 1].print_date = item.print_date;
                    checks[checks.Count - 1].sum_total = item.sum_total;
                    checks[checks.Count - 1].vat = item.vat;
                    checks[checks.Count - 1].sales.Add(convertObjectToSale(item));
                    checks[checks.Count - 1].employee = new Employee
                    {
                        empl_surname = item.empl_surname,
                        empl_name = item.empl_name,
                        empl_patronymic = item.empl_patronymic,
                    };
                }

                return checks;
            }
        }

        public async Task<IEnumerable<Check>> GetAllChecksCreatedByAllCashiersDuringPeriodAsync(DateTime startDate, DateTime endDate)
        {
            string query = @"SELECT C.*,
                             S.UPC, S.product_number, S.selling_price as store_product_total_price,
                             SP.UPC_prom, SP.id_product, SP.selling_price as store_product_price,
                             SP.products_number, SP.promotional_product, P.*, E.*
                             FROM [Check] C
                             INNER JOIN Sale S
                             ON C.check_number=S.check_number
                             INNER JOIN Store_Product SP
                             ON S.UPC=SP.UPC
                             INNER JOIN Product P
                             ON P.id_product=SP.id_product
                             INNER JOIN Employee E
                             ON C.id_employee=E.id_employee
                             WHERE C.print_date BETWEEN @StartDate AND @EndDate
                             ORDER BY C.check_number";
            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QueryAsync(query, new
                {
                    StartDate = startDate,
                    EndDate = endDate,
                });
                List<Check> checks = new List<Check>();
                string previousCheckId = "";
                foreach (var item in result)
                {
                    if (item.check_number != previousCheckId)
                    {
                        checks.Add(new Check());
                        previousCheckId = item.check_number;
                    }

                    checks[checks.Count - 1].check_number = item.check_number;
                    checks[checks.Count - 1].id_employee = item.id_employee;
                    checks[checks.Count - 1].card_number = item.card_number;
                    checks[checks.Count - 1].print_date = item.print_date;
                    checks[checks.Count - 1].sum_total = item.sum_total;
                    checks[checks.Count - 1].vat = item.vat;
                    checks[checks.Count - 1].sales.Add(convertObjectToSale(item));
                    checks[checks.Count - 1].employee = new Employee
                    {
                        empl_surname = item.empl_surname,
                        empl_name = item.empl_name,
                        empl_patronymic = item.empl_patronymic,
                    };
                }

                return checks;
            }
        }

        public async Task<IEnumerable<Check>> GetAllChecksCreatedByCertainCashierDuringPeriodAsync(Employee employee, DateTime startDate, DateTime endDate)
        {
            string query = @"SELECT C.*,
                             S.UPC, S.product_number, S.selling_price as store_product_total_price,
                             SP.UPC_prom, SP.id_product, SP.selling_price as store_product_price,
                             SP.products_number, SP.promotional_product, P.*, E.*
                             FROM [Check] C
                             INNER JOIN Sale S
                             ON C.check_number=S.check_number
                             INNER JOIN Store_Product SP
                             ON S.UPC=SP.UPC
                             INNER JOIN Product P
                             ON P.id_product=SP.id_product
                             INNER JOIN Employee E
                             ON C.id_employee=E.id_employee
                             WHERE C.id_employee=@IdEmployee AND C.print_date BETWEEN @StartDate AND @EndDate
                             ORDER BY C.check_number";
            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QueryAsync(query, new
                {
                    IdEmployee = employee.id_employee,
                    StartDate = startDate,
                    EndDate = endDate,
                });
                List<Check> checks = new List<Check>();
                string previousCheckId = "";
                foreach (var item in result)
                {
                    if (item.check_number != previousCheckId)
                    {
                        checks.Add(new Check());
                        previousCheckId = item.check_number;
                    }

                    checks[checks.Count - 1].check_number = item.check_number;
                    checks[checks.Count - 1].id_employee = item.id_employee;
                    checks[checks.Count - 1].card_number = item.card_number;
                    checks[checks.Count - 1].print_date = item.print_date;
                    checks[checks.Count - 1].sum_total = item.sum_total;
                    checks[checks.Count - 1].vat = item.vat;
                    checks[checks.Count - 1].sales.Add(convertObjectToSale(item));
                    checks[checks.Count - 1].employee = new Employee
                    {
                        empl_surname = item.empl_surname,
                        empl_name = item.empl_name,
                        empl_patronymic = item.empl_patronymic,
                    };
                }

                return checks;
            }
        }

        public async Task<IEnumerable<Check>> GetAllChecksCreatedByCertainCashierOnDateAsync(Employee employee, DateTime date)
        {
            string query = @"SELECT C.*,
                             S.UPC, S.product_number, S.selling_price as store_product_total_price,
                             SP.UPC_prom, SP.id_product, SP.selling_price as store_product_price,
                             SP.products_number, SP.promotional_product, P.*, E.*
                             FROM [Check] C
                             INNER JOIN Sale S
                             ON C.check_number=S.check_number
                             INNER JOIN Store_Product SP
                             ON S.UPC=SP.UPC
                             INNER JOIN Product P
                             ON P.id_product=SP.id_product
                             INNER JOIN Employee E
                             ON C.id_employee=E.id_employee
                             WHERE C.id_employee=@IdEmployee AND C.print_date=@Date
                             ORDER BY C.check_number";
            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QueryAsync(query, new
                {
                    IdEmployee = employee.id_employee,
                    Date = date,
                });
                List<Check> checks = new List<Check>();
                string previousCheckId = "";
                foreach (var item in result)
                {
                    if (item.check_number != previousCheckId)
                    {
                        checks.Add(new Check());
                        previousCheckId = item.check_number;
                    }

                    checks[checks.Count - 1].check_number = item.check_number;
                    checks[checks.Count - 1].id_employee = item.id_employee;
                    checks[checks.Count - 1].card_number = item.card_number;
                    checks[checks.Count - 1].print_date = item.print_date;
                    checks[checks.Count - 1].sum_total = item.sum_total;
                    checks[checks.Count - 1].vat = item.vat;
                    checks[checks.Count - 1].sales.Add(convertObjectToSale(item));
                    checks[checks.Count - 1].employee = new Employee
                    {
                        empl_surname = item.empl_surname,
                        empl_name = item.empl_name,
                        empl_patronymic = item.empl_patronymic,
                    };
                }

                return checks;
            }
        }

        public async Task<IEnumerable<Check>> GetAllChecksWhichHasStoreProductsCategoriesFruitsAndVegetablesCreatedByCashierSurnameAsync(string surname)
        {
            string query = @"SELECT *
                             FROM [Check] C
                             WHERE NOT EXISTS (SELECT *
		                                       FROM Category Cat
		                                       WHERE category_number IN (SELECT category_number
                                                                         FROM Category
                                                                         WHERE category_name IN ('Овочі', 'Фрукти')
                                                                        )
                                               AND NOT EXISTS (SELECT * 
				                                               FROM Sale S
				                                               INNER JOIN Store_Product SP
				                                               ON S.UPC=SP.UPC
				                                               INNER JOIN Product P
				                                               ON SP.id_product=P.id_product
				                                               INNER JOIN Category SP_C
				                                               ON P.category_number=SP_C.category_number
				                                               WHERE S.check_number=C.check_number
				                                               AND SP_C.category_number=Cat.category_number
                                                              )
		                                      )
                             AND id_employee IN (SELECT id_employee
	    	                                     FROM Employee
	                                             WHERE empl_role='Cashier'
	  	                                         AND empl_surname=@EmplSurname
                                                )";
            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QueryAsync<Check>(query, new
                {
                    EmplSurname = surname,
                });
            }
        }

        public async Task<Check> GetCheckByNumberAsync(string number)
        {
            string query = @"SELECT C.*,
                             S.UPC, S.product_number, S.selling_price as store_product_total_price,
                             SP.UPC_prom, SP.id_product, SP.selling_price as store_product_price,
                             SP.products_number, SP.promotional_product, P.*, E.*, CC.*, Cat.*
                             FROM [Check] C
                             INNER JOIN Sale S
                             ON C.check_number=S.check_number
                             INNER JOIN Store_Product SP
                             ON S.UPC=SP.UPC
                             INNER JOIN Product P
                             ON P.id_product=SP.id_product
                             INNER JOIN Employee E
                             ON C.id_employee=E.id_employee
                             INNER JOIN Category Cat
                             ON Cat.category_number=P.category_number
                             LEFT JOIN Customer_Card CC
                             ON C.card_number=CC.card_number
                             WHERE C.check_number=@CheckNumber";
            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QueryAsync(query, new
                {
                    CheckNumber = number,
                });

                if (result is null)
                {
                    throw new Exception("Error when fetching check by number!");
                }

                Check check = new Check();
                foreach (var item in result)
                {
                    check.check_number = item.check_number;
                    check.id_employee = item.id_employee;
                    check.card_number = item.card_number;
                    check.print_date = item.print_date;
                    check.sum_total = item.sum_total;
                    check.vat = item.vat;
                    check.sales.Add(convertObjectToSale(item));
                    check.employee = new Employee
                    {
                        empl_surname = item.empl_surname,
                        empl_name = item.empl_name,
                        empl_patronymic = item.empl_patronymic,
                    };
                    check.customer_card = new CustomerCard
                    {
                        cust_name = item.cust_name,
                        cust_surname = item.cust_surname,
                        cust_patronymic = item.cust_patronymic,
                        percent = item.percent ?? 0
                    };
                }

                return check;
            }
        }

        private Sale convertObjectToSale(dynamic item)
        {
            return new Sale
            {
                UPC = item.UPC,
                check_number = item.check_number,
                product_number = item.product_number,
                selling_price = item.store_product_total_price,
                storeProduct = new StoreProduct
                {
                    UPC = item.UPC,
                    UPC_prom = item.UPC_prom,
                    id_product = item.id_product,
                    selling_price = item.store_product_price,
                    products_number = item.products_number,
                    promotional_product = item.promotional_product,
                    product = new Product
                    {
                        id_product = item.id_product,
                        category_number = item.category_number,
                        product_name = item.product_name,
                        characteristics = item.characteristics,
                        category = new Category
                        {
                            category_number = item.category_number,
                            category_name = item.category_name,
                        },
                    },
                },
            };
        }
    }
}
