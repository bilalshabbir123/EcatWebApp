using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Models.Security
{
    public class SuperAdminHandler :
        AuthorizationHandler<ManageAdminRolesAndClaimsRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ManageAdminRolesAndClaimsRequirement requirement)
        {
            throw new NotImplementedException();
        }
    }
}
