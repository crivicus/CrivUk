﻿using CrivServer.Data.Contexts;
using CrivServer.Data.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace CrivServer.Infrastructure.Extensions
{
    public static class IdentityInjectionExtension
    {
        public static IServiceCollection ConfigureIdentityService(this IServiceCollection services, IHostEnvironment env)
        {         
            if (env.IsDevelopment())
            {
                services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<CrivDbContext>()
                    .AddDefaultUI()
                    .AddDefaultTokenProviders();
            }
            else
            {
                services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<CrivDbContext>()
                    .AddSignInManager<SignInManager<ApplicationUser>>()
                    .AddDefaultUI()
                    .AddDefaultTokenProviders();
            }

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 7;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.SignIn.RequireConfirmedEmail = false;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;
            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie authentication settings.
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                options.LoginPath = "/Identity/Account/Login";
                options.LogoutPath = "/Identity/Account/Logout";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

            return services;
        }

        public static IApplicationBuilder ConfigureIdentityApplication(this IApplicationBuilder app, IHostEnvironment env)
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
