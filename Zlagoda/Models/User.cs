using Zlagoda.Business.Entities;

namespace Zlagoda.Models
{
    public class User : Employee
    {
        public User(Employee employee)
        {
            id_employee = employee.id_employee;
            empl_name = employee.empl_name;
            empl_surname = employee.empl_surname;
            empl_patronymic = employee.empl_patronymic;
            empl_role = employee.empl_role;
            salary = employee.salary;
            date_of_birth = employee.date_of_birth;
            date_of_start = employee.date_of_start;
            city = employee.city;
            street = employee.street;
            zip_code = employee.zip_code;
        }
    }
}
