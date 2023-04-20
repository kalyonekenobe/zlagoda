using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Zlagoda.Attributes;
using Zlagoda.Business.Interfaces;
using Zlagoda.Enums;
using Zlagoda.Services;

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
		[JwtAuthorize]
		public async Task<IActionResult> Index()
		{
			var model = new
			{
				Title = "Categories list",
				Categories = await _categoryRepository.GetAllCategoriesOrderedByNameAsync(),
				User = ClaimsService.GetUserFromClaims(User),
			};
			return View($"{model.User.empl_role}Index", model);
		}
	}
}