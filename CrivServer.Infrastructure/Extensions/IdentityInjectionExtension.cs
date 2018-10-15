using CrivServer.Data.Contexts;
using CrivServer.Data.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace CrivServer.Infrastructure.Extensions
{
    public static class IdentityInjectionExtension
    {
        public static IServiceCollection ConfigureIdentityService(this IServiceCollection services, IHostingEnvironment env)
        {         
            if (env.IsDevelopment())
            {
                services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<InMemoryContext>()
                    .AddDefaultTokenProviders();
            }
            else
            {
                services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<CrivDbContext>()
                    .AddDefaultTokenProviders();
            }

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 7;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.SignIn.RequireConfirmedEmail = false;
            });

            return services;
        }

        public static IApplicationBuilder ConfigureIdentityApplication(this IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAuthentication();
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
