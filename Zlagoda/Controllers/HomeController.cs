using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Zlagoda.Business.Interfaces;

namespace Zlagoda.Controllers
{
	public class HomeController : Controller
	{
		private readonly ICategoryRepository _categoryRepository;

		public HomeController(ICategoryRepository categoryRepository)
		{
			_categoryRepository = categoryRepository;
		}

		[HttpGet]
		[Route("")]
		[Route("home")]
		public async Task<IActionResult> Index()
		{
			var claimsIdentity = User.Identity as ClaimsIdentity;
			var userId = claimsIdentity?.FindFirst(ClaimTypes.Name)?.Value;
			Console.WriteLine(User.Claims.Any().ToString());
			if (HttpContext.User.Identity is not null && !HttpContext.User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Auth");
			}
			var model = new
			{
				Title = "Categories list",
				Categories = await _categoryRepository.GetAllCategoriesOrderedByNameAsync()
			};	
			return View("ManagerIndex", model);
		}
	}
}