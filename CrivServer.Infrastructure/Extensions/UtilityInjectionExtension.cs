using CrivServer.Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CrivServer.Infrastructure.Extensions
{
    public static class UtilityInjectionExtension
    {
        public static IServiceCollection ConfigureUtilityServices(this IServiceCollection services, IConfiguration config, IHostingEnvironment env)
        {
            // Add application services.     
            var messageSender = config.GetSection("AuthMessageSenderOptions");
            services.AddEmailSenderService(opts => new AuthMessageSenderOptions() {
                DefaultSenderAddress = messageSender.GetValue<string>("DefaultSenderAddress"),
                SenderClient = messageSender.GetValue<string>("SenderClient"),
                SenderUser = messageSender.GetValue<string>("SenderUser"),
                SenderKey = messageSender.GetValue<string>("SenderKey")
            });

            services.AddSingleton<IFileProvider>(
                new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "public_data")));
            services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");
            if (env.IsDevelopment())
            {
                
            }
            else
            {
                
            }
            return services;
        }

        public static IApplicationBuilder ConfigureApplicationUtilities(this IApplicationBuilder app, IHostingEnvironment env, IConfiguration _config)
        {
            // Use this code if you want the App_Data folder to be in wwwroot
            string webrootDir = env.WebRootPath;

            // Use this if you want App_Data off your project root folder
            string baseDir = env.ContentRootPath;

            AppDomain.CurrentDomain.SetData("DataDirectory", System.IO.Path.Combine(baseDir, "App_Data"));
            //string dataDir = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();

            AppDomain.CurrentDomain.SetData("PublicDirectory", System.IO.Path.Combine(webrootDir, "public_data"));
            //string dataDir = AppDomain.CurrentDomain.GetData("PublicDirectory").ToString();

            AppDomain.CurrentDomain.SetData("WebRoot", System.IO.Path.Combine(webrootDir));
            //string dataDir = AppDomain.CurrentDomain.GetData("WebRoot").ToString();

            if (env.IsDevelopment())
            {

            }
            else
            {
                
            }
            return app;
        }
    }
}
