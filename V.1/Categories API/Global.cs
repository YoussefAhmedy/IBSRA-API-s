// Global - Application Startup
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace YourApplicationNamespace
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            // Configure Web API
            GlobalConfiguration.Configure(WebApiConfig.Register);
            
            // Configure MVC (if using MVC alongside Web API)
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_BeginRequest()
        {
            // Handle CORS preflight requests
            if (Request.Headers.AllKeys.Contains("Origin") && Request.HttpMethod == "OPTIONS")
            {
                Response.StatusCode = 200;
                Response.End();
            }
        }
    }
}
