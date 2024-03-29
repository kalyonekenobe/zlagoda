﻿using System.ComponentModel;
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
				var claim = GetClaim(principal, prop.Name);
                if (claim is not null)
				{
					string value = claim.ToString()!;
					prop.SetValue(user, converter.ConvertFromString(value), null);
				}
			}
            return user;
        } 
	}
}
