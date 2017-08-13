using System.Web.Http;
using WaesObjectDiff.App_Start;

namespace WaesObjectDiff
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "v1/{controller}/{id}/{action}",
                defaults: new { controller = "dummy", action = "index", id = RouteParameter.Optional }
            );

            config.Formatters.Add(new BrowserJsonFormatter());

        }
    }
}
