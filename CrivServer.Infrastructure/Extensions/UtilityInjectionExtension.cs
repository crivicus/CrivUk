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
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddSingleton<IFileProvider>(
                new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "public_data")));

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
            //string dataDir = AppDomain.CurrentDomain.GetData("public_data").ToString();

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
