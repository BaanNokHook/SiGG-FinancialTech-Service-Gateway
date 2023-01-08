using GM.CommonLibs.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using NLog.Web;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace GM.Service.Gateway
{
   
    public class Startup  
    {
      public Startup(IHostingEnvironment env)   
      {
        var builder = new Microsoft.Extensions.Configuration.ConfigurationBuilder();  
            builder.SetBasePath(env.ContentRootPath)
                   .AddJsonFile("appsettings.json")
                   .AddJsonFile("configuration.json", optional: false, reloadOnChange: true)
                   .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

      
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddMvc(config =>
            {
                foreach (var formatter in config.InputFormatters)
                {
                    if (formatter.GetType() == typeof(JsonInputFormatter))
                        ((JsonInputFormatter)formatter).SupportedMediaTypes.Add(
                            Microsoft.Net.Http.Headers.MediaTypeHeaderValue.Parse("text/plain"));
                }
                config.AllowEmptyInputInBodyModelBinding = true;
            });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddMvcCore().AddJsonOptions(options =>
            {
                options.SerializerSettings.Converters.Insert(0, new TrimmingStringConverter());
            });

            services.AddOcelot(Configuration);
        }


        public async void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            env.ConfigureNLog("nlog.config");

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default_route",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" }
                );
            });

            await app.UseOcelot();
        }
    }
}