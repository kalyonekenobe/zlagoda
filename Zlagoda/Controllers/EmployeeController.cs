using Microsoft.AspNetCore.Mvc;
using Zlagoda.Attributes;
using Zlagoda.Business.Entities;
using Zlagoda.Business.Interfaces;
using Zlagoda.Business.Repositories;
using Zlagoda.Enums;
using Zlagoda.Models;
using Zlagoda.Services;

namespace Zlagoda.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly PasswordService _passwordService;

        public EmployeeController(IEmployeeRepository employeeRepository, IHttpContextAccessor httpContextAccessor)
        {
            _employeeRepository = employeeRepository;
            _passwordService = new PasswordService();
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("employees")]
        [Route("employees/list")]
		[JwtAuthorize(Role = nameof(UserRoles.Manager))]
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
		[JwtAuthorize(Role = nameof(UserRoles.Manager))]
		public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (ClaimsService.GetUserFromClaims(User).id_employee == id)
                {
                    throw new Exception("You cannot delete yourself from the employees list. Ask other managers to do it!");
                }
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
		[JwtAuthorize(Role = nameof(UserRoles.Manager))]
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
		[JwtAuthorize(Role = nameof(UserRoles.Manager))]
		public async Task<IActionResult> Create(CreateEmployeeViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("Create", model);
                }
                if (model.Employee.empl_password is not null)
                {
                    model.Employee.empl_password = _passwordService.Encrypt(model.Employee.empl_password);
                }
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
		[JwtAuthorize(Role = nameof(UserRoles.Manager))]
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
		[JwtAuthorize(Role = nameof(UserRoles.Manager))]
		public async Task<IActionResult> Edit(EditEmployeeViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("Edit", model);
                }
                if (model.Employee.empl_password is not null)
                {
                    model.Employee.empl_password = _passwordService.Encrypt(model.Employee.empl_password);
                }
                await _employeeRepository.UpdateEmployeeAsync(model.Employee);
                return RedirectToAction("Index");
            }
            catch (Exception exception)
            {
                model.Errors.Add(exception.Message);
                return View("Edit", model);
            }
        }

        [HttpGet]
        [Route("employees/details/{id}")]
        [JwtAuthorize(Role = nameof(UserRoles.Cashier))]
        public async Task<IActionResult> Details(string id)
        {
            try
            {
                var employee = await _employeeRepository.GetEmployeeByIdAsync(id);
                var model = new EmployeeDetailsViewModel
                {
                    Title = "Employee details",
                    Employee = employee,
                };
                return View("Details", model);
            }
            catch (Exception exception)
            {
                TempData["Errors"] = new List<string>
                {
                    exception.Message,
                };
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        [Route("employees/brief-info/{surname}")]
        [JwtAuthorize(Role = nameof(UserRoles.Manager))]
        public async Task<IActionResult> BriefInfo(string surname)
        {
            try
            {
                var query = await _employeeRepository.GetEmployeesPhoneAndAddressBySurnameAsync(surname);
                var model = new
                {
                    Title = "Employee brief info",
                    Employees = query,
                    Surname = surname,
                };    
                return View("BriefInfo", model);
            }
            catch (Exception exception)
            {
                TempData["Errors"] = new List<string>
                {
                    exception.Message,
                };
                return RedirectToAction("Index");
            }
        }
    }
}
