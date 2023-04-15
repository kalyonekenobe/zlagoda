using Microsoft.AspNetCore.Mvc;
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

		public async Task<IActionResult> Index()
		{
			var model = new
			{
				Title = "Categories list",
				Categories = await _categoryRepository.GetAllCategoriesOrderedByNameAsync()
			};	
			return View("ManagerIndex", model);
		}
	}
}