using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Filters
{
    public class BearerTokenRequiredAttribute : Attribute, IAuthorizationFilter
    {
        private const string ExpectedToken = "SkFabTZibXE1aE14ckpQUUxHc2dnQ2RzdlFRTTM2NFE2cGI4d3RQNjZmdEFITmdBQkE=";
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();

            if (authHeader == null || !authHeader.StartsWith("Bearer "))
            {
                context.Result = new UnauthorizedObjectResult(new { message = "Authorization header missing or invalid." });
                return;
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();

            if (token != ExpectedToken)
            {
                context.Result = new UnauthorizedObjectResult(new { message = "Invalid Bearer token." });
            }
        }
    }
}
