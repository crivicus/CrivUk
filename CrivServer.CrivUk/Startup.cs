using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CrivServer.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using CrivServer.Infrastructure.Extensions;
using System;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Http.Internal;
using CrivServer.Data.Contexts;

namespace CrivServer.CrivUk
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfiguration _config;        
        private readonly ILoggerFactory _loggerFactory;
        private readonly IEncryptor _manualEncryptor;

        public Startup(IHostingEnvironment environment, IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _env = environment;
            _config = configuration;
            _loggerFactory = loggerFactory;
            _manualEncryptor = new Encryptor(_config.GetValue<string>("Protector"));
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var logger = _loggerFactory.CreateLogger<Startup>();
            if (_env.IsDevelopment())
            {
                // Development service configuration
                logger.LogInformation("Development environment");
                
            } else {
                // Non-development service configuration
                logger.LogInformation($"Environment: {_env.EnvironmentName}");
                services.AddHsts(options =>
                {
                    options.Preload = true;
                    options.IncludeSubDomains = true;
                    options.MaxAge = TimeSpan.FromDays(30);
                });                
            }
            //var config1 = _config.EncryptConfiguration(_manualEncryptor);
            var config = _config.DecryptConfiguration(_manualEncryptor);
            services.AddSingleton(config);            
            
            //Add the encryption/decryption service
            services.ConfigureDataProtectionService(config, _env);

            var checkConsentNeeded = config.GetValue<string>("CheckNotConsentNeeded");
            bool bConverted = bool.TryParse(checkConsentNeeded, out bool bcheckConsentNotNeeded);

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies 
                // is needed for a given request.
                // CheckConsentNeeded set via configuration for test purposes only.         
                options.CheckConsentNeeded = context => !bcheckConsentNotNeeded;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // Add database and identity services           
            services.ConfigureDataService(config, _env);
            services.ConfigureIdentityService(_env);

            // Add application services.            
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1).AddRazorPagesOptions(options =>
            {
                options.AllowAreas = true;
                options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
                options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            if (_env.IsDevelopment())
            {
                //app.UseStatusCodePagesWithReExecute("/error", "?statusCode={0}");
                //app.UseExceptionHandler("/error/{0}");
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                
                app.UseStatusCodePagesWithReExecute("/error", "?statusCode={0}");
                app.UseExceptionHandler("/error/{0}");
                app.UseHsts();
            }
            // Use this code if you want the App_Data folder to be in wwwroot
            //string baseDir = env.WebRootPath;

            // Use this if you want App_Data off your project root folder
            string baseDir = _env.ContentRootPath;
            AppDomain.CurrentDomain.SetData("DataDirectory", System.IO.Path.Combine(baseDir, "App_Data"));
            //string dataDir = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            
            //Configure database and identity within the app
            app.ConfigureDataApplication(_env, _config);
            app.ConfigureIdentityApplication(_env);

            //Allow rewind
            app.Use(next => context => { context.Request.EnableRewind(); return next(context); });

            app.UseMvc(routes =>
            {
                //routes.MapRoute(
                //    name: "error",
                //    template: "{controller=Error}/{action=Error}/{id?}"
                //);
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}"
                );
                routes.MapRoute(
                    name: "cms",
                    template: "{*url}",
                    defaults: new { controller = "Content", action = "Index", url = UriComponents.AbsoluteUri }
                );                
            });
        }
    }
}
