using System.Web.Http;
using Thinktecture.IdentityModel.Http.Cors.WebApi;

namespace nec.nexu
{
    public class CorsConfig
    {
        public static void RegisterCors(HttpConfiguration httpConfig)
        {
            WebApiCorsConfiguration corsConfig = new WebApiCorsConfiguration();

            // this adds the CorsMessageHandler to the HttpConfiguration's
            // MessageHandlers collection
            corsConfig.RegisterGlobal(httpConfig);

            // This is for testing only
            // In Production, specific options should be used to control CORS
            corsConfig
                .ForAll()
                .AllowAll();



            //corsConfig
            //    .ForAllResources()
            //    .ForAllOrigins()
            //    .AllowAllMethodsAndAllRequestHeaders()
            //    .AllowAll();

            /*
              // this allow all CORS requests to the Products controller
              // from the http://foo.com origin.
              corsConfig
                 .ForResources("Products")
                 .ForOrigins("http://foo.com");
        
    
                }
                corsConfig
                .ForResources("Values1")
                .ForOrigins("http://localhost")
                .AllowMethods("GET", "POST");

                corsConfig
                    .ForResources("Values2")
                    .ForOrigins("http://localhost")
                    .AllowMethods("GET", "POST")
                    .AllowCookies()
                    .AllowResponseHeaders("Foo");

                corsConfig
                    .ForResources("Values3")
                    .ForOrigins("http://localhost")
                    .AllowMethods("GET", "POST", "PUT")
                    .AllowRequestHeaders("Content-Type");

                corsConfig
                    .ForResources("Values4")
                    .ForOrigins("http://localhost")
                    .AllowAllMethods()
                    .AllowCookies()
                    .AllowRequestHeaders("Content-Type", "Foo", "Authorization")
                    .AllowResponseHeaders("Foo");
             */
        }
    }
}