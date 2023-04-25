using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Zlagoda.Attributes;
using Zlagoda.Business.Entities;
using Zlagoda.Business.Interfaces;
using Zlagoda.Enums;
using Zlagoda.Models;
using Zlagoda.Services;

namespace Zlagoda.Controllers
{
    public class CheckController : Controller
    {
        private readonly ICheckRepository _checkRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IStoreProductRepository _storeProductRepository;
        private readonly ISaleRepository _saleRepository;
        private readonly ICustomerCardRepository _customerCardRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CheckController(ICheckRepository checkRepository, IEmployeeRepository employeeRepository, IStoreProductRepository storeProductRepository, ISaleRepository saleRepository, ICustomerCardRepository customerCardRepository, IHttpContextAccessor httpContextAccessor)
        {
            _checkRepository = checkRepository;
            _employeeRepository = employeeRepository;
            _storeProductRepository = storeProductRepository;
            _saleRepository = saleRepository;
            _customerCardRepository = customerCardRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("checks")]
        [Route("checks/list")]
        [JwtAuthorize]
        public async Task<IActionResult> Index([FromQuery(Name = "cashier-id")] string? cashierId = null, [FromQuery(Name = "start-date")] string? startDate = null, [FromQuery(Name = "end-date")] string? endDate = null)
        {
            DateTime dateStart, dateEnd;
            bool startDateExists = DateTime.TryParse(startDate, out dateStart);
            bool endDateExists = DateTime.TryParse(endDate, out dateEnd);
            IEnumerable<Check> checks = new List<Check>();
            var user = ClaimsService.GetUserFromClaims(_httpContextAccessor.HttpContext!.User);
            int totalQuantityOfSoldProducts = 0;
            if (user.empl_role == nameof(UserRoles.Cashier))
            {
                if (startDateExists)
                {
                    if (endDateExists)
                    {
                        checks = await _checkRepository.GetAllChecksCreatedByCertainCashierDuringPeriodAsync(user, dateStart, dateEnd);
                        totalQuantityOfSoldProducts = await _saleRepository.GetTotalQuantityOfStoreProductsSoldByCertainCashierDuringPeriodAsync(user, dateStart, dateEnd);
                    }
                    else
                    {
                        checks = await _checkRepository.GetAllChecksCreatedByCertainCashierOnDateAsync(user, dateStart);
                        totalQuantityOfSoldProducts = await _saleRepository.GetTotalQuantityOfStoreProductsSoldByCertainCashierDuringPeriodAsync(user, dateStart, dateStart);
                    }
                } 
                else
                {
                    checks = await _checkRepository.GetAllChecksCreatedByCertainCashierDuringPeriodAsync(user, new DateTime(1970, 1, 1), DateTime.Now);
                }
            }
            else
            {
                if (cashierId is not null)
                {
                    var cashier = await _employeeRepository.GetEmployeeByIdAsync(cashierId);
                    if (startDateExists)
                    {
                        if (endDateExists)
                        {
                            checks = await _checkRepository.GetAllChecksCreatedByCertainCashierDuringPeriodAsync(cashier, dateStart, dateEnd);
                            totalQuantityOfSoldProducts = await _saleRepository.GetTotalQuantityOfStoreProductsSoldByCertainCashierDuringPeriodAsync(cashier, dateStart, dateEnd);
                        }
                        else
                        {
                            checks = await _checkRepository.GetAllChecksCreatedByCertainCashierOnDateAsync(cashier, dateStart);
                            totalQuantityOfSoldProducts = await _saleRepository.GetTotalQuantityOfStoreProductsSoldByCertainCashierDuringPeriodAsync(cashier, dateStart, dateStart);
                        }
                    }
                    else
                    {
                        checks = await _checkRepository.GetAllChecksCreatedByCertainCashierDuringPeriodAsync(cashier, new DateTime(1970, 1, 1), DateTime.Now);
                    }
                }
                else
                {
                    checks = await _checkRepository.GetAllChecksAsync();
                    if (!startDateExists)
                        dateStart = new DateTime(1970, 1, 1);
                    if (!endDateExists)
                        dateEnd = DateTime.Now;
                    totalQuantityOfSoldProducts = await _saleRepository.GetTotalQuantityOfStoreProductsSoldByAllCashiersDuringPeriodAsync(dateStart, dateEnd);
                }
            }
            var model = new CheckListViewModel
            {
                Title = "Checks",
                Checks = checks,
                Cashiers = await _employeeRepository.GetAllEmployeesCashiersOrderedBySurnameAsync(),
                TotalQuantityOfSoldProducts = totalQuantityOfSoldProducts,
                Errors = TempData["Errors"] ?? new List<string>(),
            };
            return View("List", model);
        }

        [HttpGet]
        [Route("checks/delete/{id}")]
        [JwtAuthorize(Role = nameof(UserRoles.Manager))]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _checkRepository.DeleteCheckAsync(new Check { check_number = id });
            }
            catch (Exception exception)
            {
                TempData["Errors"] = new List<string>
                {
                    exception.Message,
                };
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("checks/create")]
        [JwtAuthorize(Role = nameof(UserRoles.Cashier))]
        public async Task<IActionResult> Create()
        {
            try
            {
                var storeProducts = await _storeProductRepository.GetAllStoreProductsOrderedByNameAsync();
                var clients = await _customerCardRepository.GetAllCustomerCardsOrderedBySurnameAsync();
                var model = new CreateCheckViewModel
                {
                    Title = "Create check",
                    StoreProducts = storeProducts,
                    Clients = clients,
                    Errors = TempData["Errors"] ?? new List<string>(),
                };
                return View("Create", model);
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

        [HttpPost]
        [Route("checks/create")]
        [JwtAuthorize(Role = nameof(UserRoles.Cashier))]
        public async Task<IActionResult> Create(string value)
        {
            try
            {
                string json;
                using (var streamReader = new StreamReader(Request.Body))
                {
                    json = await streamReader.ReadToEndAsync();
                }
                dynamic? result = JsonConvert.DeserializeObject(json ?? string.Empty);
                if (result is null)
                {
                    throw new Exception("Send request error!");
                }

                decimal totalPrice = 0;
                List<Sale> checkSales = new List<Sale>();
                foreach (var product in result.products)
                {
                    totalPrice += decimal.Parse(product.selling_price.ToString()) * decimal.Parse(product.product_number.ToString());
                    checkSales.Add(new Sale
                    {
                        check_number = result.check_number,
                        UPC = product.UPC,
                        selling_price = decimal.Parse(product.selling_price.ToString()),
                        product_number = int.Parse(product.product_number.ToString()),
                    });
                }
                var check = new Check
                {
                    check_number = result.check_number,
                    card_number = result.card_number.ToString() == "" ? null : result.card_number.ToString(),
                    id_employee = result.id_employee,
                    sum_total = totalPrice,
                    vat = totalPrice * 0.2m,
                    print_date = DateTime.Parse(result.print_date.ToString()),
                };
                await _checkRepository.CreateCheckAsync(check);
                foreach (var sale in checkSales)
                {
                    await _saleRepository.CreateSaleAsync(sale);
                }
                return StatusCode(200);
            }
            catch (Exception exception)
            {
                var storeProducts = await _storeProductRepository.GetAllStoreProductsOrderedByNameAsync();
                var clients = await _customerCardRepository.GetAllCustomerCardsOrderedBySurnameAsync();
                var model = new CreateCheckViewModel
                {
                    Title = "Create check",
                    StoreProducts = storeProducts,
                    Clients = clients,
                    Errors = new List<string>
                    {
                        exception.Message,
                    },
                };
                return StatusCode(500, model.Errors);
            }
        }

        [HttpGet]
        [Route("checks/details/{id}")]
        [JwtAuthorize]
        public async Task<IActionResult> Details(string id)
        {
            try
            {
                var check = await _checkRepository.GetCheckByNumberAsync(id);
                var model = new CheckDetailsViewModel
                {
                    Title = "Check details",
                    Check = check,
                    Errors = TempData["Errors"] ?? new List<string>(),
                };
                return View("Details", model);
            }
            catch (Exception exception)
            {
                TempData["Errors"] = new List<string>()
                {
                    exception.Message,
                };
                return RedirectToAction("Index");
            }
        }
    }
}
