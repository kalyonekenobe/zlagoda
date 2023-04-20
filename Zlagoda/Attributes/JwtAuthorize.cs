using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Zlagoda.Enums;
using Zlagoda.Services;

namespace Zlagoda.Attributes
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class JwtAuthorize : Attribute, IAuthorizationFilter
	{
		public string? Role { get; set; }
		public void OnAuthorization(AuthorizationFilterContext context)
		{
			var xAccessToken = context.HttpContext.Request.Cookies["X-Access-Token"] ?? string.Empty;
			if (!JwtTokenService.ValidateJwtToken(xAccessToken))
			{
				context.Result = new RedirectToActionResult("Index", "Auth", null);
				return;
			}
			var handler = new JwtSecurityTokenHandler();
			var token = handler.ReadToken(xAccessToken);
			var securityToken = token as JwtSecurityToken;
			if (securityToken is not null)
			{
				var claims = securityToken.Claims;
				context.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
				var role = ClaimsService.GetClaim(context.HttpContext.User, "empl_role");
				if (Role is null || role is null)
				{
					return;
				}
				if (!Role.Equals(role))
				{
					context.Result = new RedirectToActionResult("Index", "Home", null);
				}
				return;
			} 
			else
			{
				context.HttpContext.Response.Cookies.Delete("X-Access-Token");
				context.Result = new RedirectToActionResult("Index", "Auth", null);
			}
		}
	}
}
