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
            _authService = new AuthService(employeeRepository, httpContextAccessor);
            _httpContextAccessor = httpContextAccessor; 
        }

        [HttpGet]
        [Route("auth")]
        public IActionResult Index()
        {
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
            var jwtToken = Request.Cookies["token"] ?? string.Empty;
            if (JwtTokenService.ValidateJwtToken(jwtToken)) 
            {
                return RedirectToAction("Index", "Home");
            }
            try
            {
                var token = await _authService.SignIn(model.IdEmployee, model.Password);
                _httpContextAccessor.HttpContext?.Response.Cookies.Append("token", token);
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
    }
}
