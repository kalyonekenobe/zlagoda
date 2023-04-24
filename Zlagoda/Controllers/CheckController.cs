using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection;
using Zlagoda.Attributes;
using Zlagoda.Business.Entities;
using Zlagoda.Business.Interfaces;
using Zlagoda.Enums;
using Zlagoda.Models;

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
        public async Task<IActionResult> Index()
        {
            var model = new CheckListViewModel
            {
                Title = "Checks",
                Checks = await _checkRepository.GetAllChecksAsync(),
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
    }
}
