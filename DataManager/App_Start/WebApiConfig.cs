using System.Web.Http;
using System.Web.Http.Cors;
using DataManager.GlobalFaultExceptionHandler;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Converters;

namespace DataManager
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            var json = config.Formatters.JsonFormatter;
            json.SerializerSettings.Converters.Add(new StringEnumConverter());

            // Web API routes
            config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute("DefaultApiWithId", "api/{controller}/{id}", new { id = RouteParameter.Optional });
            //config.Routes.MapHttpRoute("DefaultApiWithAction", "api/{controller}/{action}");
            //config.Routes.MapHttpRoute("DefaultApiGet", "api/{controller}", new { action = "Get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            //config.Routes.MapHttpRoute("DefaultApiPost", "api/{controller}", new { action = "Post" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            config.Filters.Add(new ApiExceptionHandelling());
        }
    }
}
