using System.Web.Http;
using System.Web.Http.Cors;

public static class WebApiConfig
{
    public static void Register(HttpConfiguration config)
    {
        // Enable CORS for all origins, headers, and methods
        var cors = new EnableCorsAttribute("*", "*", "*");
        config.EnableCors(cors);

        // Web API configuration and services
        config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = 
            Newtonsoft.Json.ReferenceLoopHandling.Ignore;

        config.Formatters.JsonFormatter.SerializerSettings.DateFormatHandling = 
            Newtonsoft.Json.DateFormatHandling.IsoDateFormat;

        // Web API routes
        config.MapHttpAttributeRoutes();

        config.Routes.MapHttpRoute(
            name: "DefaultApi",
            routeTemplate: "api/{controller}/{id}",
            defaults: new { id = RouteParameter.Optional }
        );

        // Remove XML formatter to return JSON by default
        config.Formatters.Remove(config.Formatters.XmlFormatter);
    }
}
