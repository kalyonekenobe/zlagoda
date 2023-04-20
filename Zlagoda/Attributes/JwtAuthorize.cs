using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Zlagoda.Services;

namespace Zlagoda.Attributes
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class JwtAuthorize : Attribute, IAuthorizationFilter
	{
		public void OnAuthorization(AuthorizationFilterContext context)
		{
			var xAccessToken = context.HttpContext.Request.Cookies["X-Access-Token"] ?? string.Empty;
			if (!JwtTokenService.ValidateJwtToken(xAccessToken))
			{
				context.Result = new RedirectToActionResult("Index", "Auth", null);
			}
		}
	}
}
