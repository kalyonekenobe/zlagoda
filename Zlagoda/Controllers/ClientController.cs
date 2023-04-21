using Microsoft.AspNetCore.Mvc;
using Zlagoda.Attributes;
using Zlagoda.Business.Entities;
using Zlagoda.Business.Interfaces;
using Zlagoda.Business.Repositories;
using Zlagoda.Models;
using Zlagoda.Services;

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
        public async Task<IActionResult> Index([FromQuery(Name = "surname")] string? surname = null, [FromQuery(Name = "percent")] int? percent = null)
        {
            var model = new ClientListViewModel
            {
                Title = "Clients",
                Errors = TempData["Errors"] ?? new List<string>(),
            };
            if (string.IsNullOrEmpty(surname) && percent == null)
            {
                model.Clients = await _customerCardRepository.GetAllCustomerCardsOrderedBySurnameAsync();
            } 
            else
            {
                if (!string.IsNullOrEmpty(surname))
                {
                    model.Clients = await _customerCardRepository.GetCustomerCardsBySurnameAsync(surname);
                }
                if (percent is not null)
                {
                    model.Clients = await _customerCardRepository.GetAllCustomerCardsWithCertainDiscountPercentOrderedBySurnameAsync(percent.Value);
                }
            }
            return View("List", model);
        }

        [HttpGet]
        [Route("clients/delete/{id}")]
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

        [HttpGet]
        [Route("clients/create")]
        [JwtAuthorize]
        public IActionResult Create()
        {
            var model = new CreateClientViewModel
            {
                Title = "Create client",
                Client = new CustomerCard(),
            };
            return View("Create", model);
        }

        [HttpPost]
        [Route("clients/create")]
        [JwtAuthorize]
        public async Task<IActionResult> Create(CreateClientViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("Create", model);
                }
                await _customerCardRepository.CreateCustomerCardAsync(model.Client);
                return RedirectToAction("Index");
            }
            catch (Exception exception)
            {
                model.Errors.Add(exception.Message);
                return View("Create", model);
            }
        }

        [HttpGet]
        [Route("clients/edit/{id}")]
        [JwtAuthorize]
        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                var client = await _customerCardRepository.GetCustomerCardByNumberAsync(id);
                if (client is null)
                {
                    throw new Exception();
                }
                var model = new EditClientViewModel
                {
                    Title = "Edit client",
                    Client = client,
                };
                return View("Edit", model);
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

        [HttpPost]
        [Route("clients/edit/{id}")]
        [JwtAuthorize]
        public async Task<IActionResult> Edit(EditClientViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("Edit", model);
                }
                await _customerCardRepository.UpdateCustomerCardAsync(model.Client);
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
