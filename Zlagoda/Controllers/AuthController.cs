using Microsoft.AspNetCore.Mvc;
using Zlagoda.Business.Interfaces;
using Zlagoda.Models;
using Zlagoda.Services;

namespace Zlagoda.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthService _authService;
        private IHttpContextAccessor _httpContextAccessor;

        public AuthController(IEmployeeRepository employeeRepository, IHttpContextAccessor httpContextAccessor)
        {
            _authService = new AuthService(employeeRepository);
            _httpContextAccessor = httpContextAccessor; 
        }

        [HttpGet]
        [Route("auth")]
        public IActionResult Index()
        {
			var xAccessToken = Request.Cookies["X-Access-Token"] ?? string.Empty;
			if (JwtTokenService.ValidateJwtToken(xAccessToken))
            { 			
				return RedirectToAction("Index", "Home");
			}
			var model = new AuthViewModel
            {
                Title = "Authorization",
                Errors = TempData["Errors"] ?? new List<string>(),
            };
            return View("SignIn", model);
        }

        [HttpPost]
        [Route("auth")]
        public async Task<IActionResult> Authenticate(AuthViewModel model)
        {
            try
            {
                var token = await _authService.SignIn(model.Phone, model.Password);
                var cookies = _httpContextAccessor.HttpContext?.Response.Cookies;
                if (cookies is null)
                {
                    throw new Exception("Authenication failed!");
                }
                cookies.Append("X-Access-Token", token, new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict
                });
                return RedirectToAction("Index", "Home");
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

		[HttpGet]
		[Route("logout")]
		public IActionResult Logout()
		{
			Response.Cookies.Delete("X-Access-Token");
			return RedirectToAction("Index");
		}
	}
}
