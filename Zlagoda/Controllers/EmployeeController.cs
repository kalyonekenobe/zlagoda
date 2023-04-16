using Microsoft.AspNetCore.Mvc;
using Zlagoda.Business.Entities;
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

        [HttpGet]
        [Route("employees/list")]
        public async Task<IActionResult> Index([FromQuery(Name = "show")] string show = "")
        {
            IEnumerable<Employee> employees;
            if (show == "cashiers")
            {
                employees = await _employeeRepository.GetAllEmployeesCashiersOrderedBySurnameAsync();
            } 
            else
            {
                employees = await _employeeRepository.GetAllEmployeesOrderedBySurnameAsync();
            }

            var model = new 
            {
                Title = "Employees",
                Employees = employees,
                Errors = TempData["Errors"] ?? new List<string>(),
            };

            return View("List", model);
        }

        [HttpGet]
        [Route("employees/delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _employeeRepository.DeleteEmployeeAsync(new Employee { id_employee = id });
            }
            catch (Exception exception)
            {
                TempData["Errors"] = new List<string>
                {
                    exception.Message
                };
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("employees/create")]
        public IActionResult Create()
        {
            var model = new
            {
                Title = "Create employee"
            };
            return View("Create", model);
        }
    }
}
