using Microsoft.AspNetCore.Mvc;
using Zlagoda.Attributes;
using Zlagoda.Services;

namespace Zlagoda.Controllers
{
	public class HomeController : Controller
	{
		public HomeController() { }

		[HttpGet]
		[Route("")]
		[Route("home")]
		[JwtAuthorize]
		public IActionResult Index()
		{
			var user = ClaimsService.GetUserFromClaims(User);
            var model = new
			{
				Title = "Homepage",
			};
			return View($"{user.empl_role}Index", model);
		}
	}
}