using Microsoft.AspNetCore.Mvc;
using Zlagoda.Business.Entities;
using Zlagoda.Business.Interfaces;
using Zlagoda.Models;
using Zlagoda.Services;

namespace Zlagoda.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly PasswordService _passwordService;

        public EmployeeController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
            _passwordService = new PasswordService();
        }

        [HttpGet]
        [Route("employees")]
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
            var model = new CreateEmployeeViewModel
            {
                Title = "Create employee",
                Employee = new Employee(),
            };
            return View("Create", model);
        }

        [HttpPost]
        [Route("employees/create")]
        public async Task<IActionResult> Create(CreateEmployeeViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("Create", model);
                }
                model.Employee.empl_password = _passwordService.Encrypt(model.Employee.empl_password);
                await _employeeRepository.CreateEmployeeAsync(model.Employee);
                return RedirectToAction("Index");
            }
            catch (Exception exception)
            {
                model.Errors.Add(exception.Message);
                return View("Create", model);
            }
        }

        [HttpGet]
        [Route("employees/edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                var employee = await _employeeRepository.GetEmployeeByIdAsync(id);
                var model = new EditEmployeeViewModel
                {
                    Title = "Edit employee",
                    Employee = employee,
                };
                return View("Edit", model);
            }
            catch (Exception exception)
            {
                TempData["Errors"] = new List<string>
                {
                    exception.Message
                };
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Route("employees/edit/{id}")]
        public async Task<IActionResult> Edit(EditEmployeeViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("Edit", model);
                }
                model.Employee.empl_password = _passwordService.Encrypt(model.Employee.empl_password);
                await _employeeRepository.UpdateEmployeeAsync(model.Employee);
                return RedirectToAction("Index");
            }
            catch (Exception exception)
            {
                model.Errors.Add(exception.Message);
                return View("Edit", model);
            }
        }
    }
}
