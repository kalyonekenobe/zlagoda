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
		public HomeController() { }

		[HttpGet]
		[Route("")]
		[Route("home")]
		[JwtAuthorize]
		public IActionResult Index()
		{
			var model = new
			{
				Title = "Homepage",
				User = ClaimsService.GetUserFromClaims(User),
			};
			return View($"{model.User.empl_role}Index", model);
		}
	}
}