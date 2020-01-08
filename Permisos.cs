using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Permissions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WDXWebApiDespachoJuridico
{
    public class PermissionAuthorizationRequirement : IAuthorizationRequirement
    {
        //Add any custom requirement properties if you have them
    }

    public class PermisoRequirement : AuthorizationHandler<PermissionAuthorizationRequirement>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionAuthorizationRequirement requirement)
        {
            
            if (!context.User.Identity.IsAuthenticated)
            {
                context.Fail();
            }
            else context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
