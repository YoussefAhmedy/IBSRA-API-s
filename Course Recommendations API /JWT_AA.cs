// JWT Authorization Attribute
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

public class JwtAuthorizationAttribute : AuthorizationFilterAttribute
{
    private readonly JwtHelper _jwtHelper;

    public JwtAuthorizationAttribute()
    {
        _jwtHelper = new JwtHelper();
    }

    public override void OnAuthorization(HttpActionContext actionContext)
    {
        var authHeader = actionContext.Request.Headers.Authorization;

        if (authHeader == null || authHeader.Scheme != "Bearer")
        {
            actionContext.Response = actionContext.Request.CreateErrorResponse(
                HttpStatusCode.Unauthorized, "Missing or invalid authorization header");
            return;
        }

        var token = authHeader.Parameter;
        var principal = _jwtHelper.ValidateToken(token);

        if (principal == null)
        {
            actionContext.Response = actionContext.Request.CreateErrorResponse(
                HttpStatusCode.Unauthorized, "Invalid or expired token");
            return;
        }

        // Set the current principal
        actionContext.RequestContext.Principal = principal;
    }
}
