using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.VisualStudio.Services.GitHubConnector;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Net;

namespace eventRegistration
{
    public class ClaimRequirementFilter :Attribute, IAuthorizationFilter
    {

     
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var authHeader = context.HttpContext.Request.Headers["Authorization"];

            if (authHeader.ToString() != "bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IlFSIiwiaWF0IjoxNTE2MjM5MDIyfQ.sX6nddIelhs-uW1JZKFkZ0r1pwX70LEASXHiZ3zpQ68")
            {
                context.HttpContext.Response.StatusCode = 401;
                //return;
                context.Result = new UnauthorizedResult();

            }

        }
    }
    //public class ClaimRequirementAttribute : TypeFilterAttribute
    //{
    //    public ClaimRequirementAttribute(string claimType, string claimValue) : base(typeof(ClaimRequirementFilter))
    //    {
    //        Arguments = new object[] { new Claim(claimType, claimValue) };
    //    }
    //}
}
