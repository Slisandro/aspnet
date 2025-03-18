using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace GestionContableYugen.Filters
{
    public class RoleAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly int[] _allowedRoles;

        public RoleAuthorizeAttribute(params int[] allowedRoles)
        {
            _allowedRoles = allowedRoles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var usuarioRol = context.HttpContext.Session.GetInt32("RolID");

            if (!usuarioRol.HasValue || !_allowedRoles.Contains(usuarioRol.Value))
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
            }
        }
    }
}