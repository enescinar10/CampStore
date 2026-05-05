// WebApiConfig.cs
// API route ayarları ve JSON formatı burada yapılandırılır

using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Web.Http.Cors;

namespace CampStore.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);
            // CORS — tüm kaynaklara izin ver (geliştirme ortamı için)
            config.EnableCors();

            // Route tanımı — api/controller/id formatı
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // JSON formatını varsayılan yap — XML'i kaldır
            var formatters = config.Formatters;
            formatters.Remove(formatters.XmlFormatter);

            // JSON ayarları — camelCase ve güzel format
            var jsonSettings = formatters.JsonFormatter.SerializerSettings;
            jsonSettings.Formatting = Formatting.Indented;
            jsonSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            jsonSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
        }
    }
}