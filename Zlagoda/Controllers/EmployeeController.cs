using Microsoft.AspNetCore.Mvc;
using Zlagoda.Business.Interfaces;

namespace Zlagoda.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<IActionResult> Index()
        {
            var model = new
            {
                Title = "Employees",
                Employees = await _employeeRepository.GetAllEmployeesOrderedBySurnameAsync(),
            };

            return View("ManagerEmployeeList", model);
        }
    }
}
