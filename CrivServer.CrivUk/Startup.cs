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
using Microsoft.Extensions.Hosting;

namespace CrivServer.CrivUk
{
    public class Startup
    {
        private readonly IHostEnvironment _env;
        private readonly IWebHostEnvironment _env2;
        private readonly IConfiguration _config;        
        private readonly ILoggerFactory _loggerFactory;
        private readonly IEncryptor _manualEncryptor;

        public Startup(IHostEnvironment environment, IWebHostEnvironment webenvironment, IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _env = environment;
            _env2 = webenvironment;
            _config = configuration;
            _loggerFactory = loggerFactory;
            _manualEncryptor = new Encryptor(_config.GetValue<string>("Protector"));
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var logger = _loggerFactory.CreateLogger<Startup>();
            services.AddDatabaseDeveloperPageExceptionFilter();
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
            //Encrypt the appsettings on initial run
            services.ConfigurationInjectionService(_config, _env, _manualEncryptor);     
            
            //Add the encryption/decryption service
            services.ConfigureDataProtectionService(_config, _env);

            var checkConsentNeeded = _config.GetValue<string>("CheckNotConsentNeeded");
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
            services.ConfigureDataService(_config, _env);
            services.ConfigureIdentityService(_env);
            services.ConfigureUtilityServices(_config, _env);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest).AddRazorPagesOptions(options =>
            {
                options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
                options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
            });

            services.AddSignalR();
            services.AddSingleton<Microsoft.AspNetCore.SignalR.IUserIdProvider, Infrastructure.Hubs.CustomUserIdProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            if (_env.IsDevelopment())
            {
                //app.UseStatusCodePagesWithReExecute("/error", "?statusCode={0}");
                //app.UseExceptionHandler("/error/{0}");
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {                
                app.UseStatusCodePagesWithReExecute("/error", "?statusCode={0}");
                app.UseExceptionHandler("/error/{0}");
                app.UseHsts();
            }
                        
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            
            //Configure database and identity within the app
            app.ConfigureDataApplication(_env, _config);
            app.ConfigureIdentityApplication(_env);
            app.ConfigureApplicationUtilities(_env2, _config);

            //Allow rewind
            app.Use(next => context => { context.Request.EnableBuffering(); return next(context); });

            app.UseEndpoints(routes =>
            {
                routes.MapHub<Infrastructure.Hubs.ChatHub>("/chatHub");
                routes.MapHub<Infrastructure.Hubs.StreamHub>("/streamHub");
                routes.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}"
                );
                routes.MapControllerRoute(
                    name: "cms",
                    pattern: "{*url}",
                    defaults: new { controller = "Content", action = "Index", url = UriComponents.AbsoluteUri }
                );
            });
        }
    }
}
