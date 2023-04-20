using System.ComponentModel;
using System.Reflection;
using System.Security.Claims;
using Zlagoda.Business.Entities;
using Zlagoda.Models;

namespace Zlagoda.Services
{
	public static class ClaimsService
	{
		public static object? GetClaim(ClaimsPrincipal principal, string type)
		{
			return principal.Claims.FirstOrDefault(claim => claim.Type == type)?.Value;
		}

		public static User GetUserFromClaims(ClaimsPrincipal principal) 
		{
            var user = new User(new Employee());
            foreach (PropertyInfo prop in user.GetType().GetProperties())
            {
				var converter = TypeDescriptor.GetConverter(prop.PropertyType);
                prop.SetValue(user, converter.ConvertFromString(GetClaim(principal, prop.Name)?.ToString() ?? string.Empty), null);
			}
            return user;
        } 
	}
}
