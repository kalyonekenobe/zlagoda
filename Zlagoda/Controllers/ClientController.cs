using Microsoft.AspNetCore.Mvc;
using Zlagoda.Attributes;
using Zlagoda.Business.Entities;
using Zlagoda.Business.Interfaces;

namespace Zlagoda.Controllers
{
    public class ClientController : Controller
    {
        private readonly ICustomerCardRepository _customerCardRepository;

        public ClientController(ICustomerCardRepository customerCardRepository)
        {
            _customerCardRepository = customerCardRepository;
        }

        [HttpGet]
        [Route("clients")]
        [Route("clients/list")]
        [JwtAuthorize]
        public async Task<IActionResult> Index()
        {
            var clients = await _customerCardRepository.GetAllCustomerCardsOrderedBySurnameAsync();
            var model = new
            {
                Title = "Clients",
                Clients = clients,
                Errors = TempData["Errors"] ?? new List<string>(),
            };
            return View("List", model);
        }

        [HttpGet]
        [Route("client/delete/{id}")]
        [JwtAuthorize]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _customerCardRepository.DeleteCustomerCardAsync(new CustomerCard() { card_number = id });
            }
            catch (Exception exception)
            {
                TempData["Errors"] = new List<string>()
                {
                    exception.Message,
                };
            }
            return RedirectToAction("Index");
        }
    }
}
