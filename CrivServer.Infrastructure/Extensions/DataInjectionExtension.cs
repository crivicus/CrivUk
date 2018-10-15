using CrivServer.Data.Contexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CrivServer.Infrastructure.Extensions
{
    public static class InjectionExtension
    {
        public static IServiceCollection ConfigureDataService(this IServiceCollection services, IConfiguration _config, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                services.AddDbContext<InMemoryContext>(opt => opt.UseInMemoryDatabase("crivdbtest"));
            }
            else
            {
                var connectionString = _config.GetConnectionString("DefaultConnection");
                services.AddDbContext<CrivDbContext>(options =>
                    options.UseMySql(connectionString));
            }
            return services;
        }

        public static IApplicationBuilder ConfigureDataApplication(this IApplicationBuilder app, IHostingEnvironment env, IConfiguration _config)
        {
            if (env.IsDevelopment())
            {
                var testdb = app.ApplicationServices.GetService<InMemoryContext>();
                
                AddTestData(testdb, _config);
            }
            else
            {

            }
            return app;
        }

        private static void AddTestData(InMemoryContext context, IConfiguration _config)
        {
            var testuser = _config.GetSection("TestUser");
            var user = new CrivServer.Data.Models.ApplicationUser {
                UserName = testuser.GetValue<string>("UserName"),
                Email = testuser.GetValue<string>("Email"),
                NormalizedUserName = testuser.GetValue<string>("NormalizedUserName"),
                NormalizedEmail = testuser.GetValue<string>("NormalizedEmail"),
                PasswordHash = testuser.GetValue<string>("PasswordHash"),
                UserType = testuser.GetValue<int>("UserType")
            };

            context.ApplicationUsers.Add(user);

            var page = new CrivServer.Data.Models.DbContentModel {
                content_id = 1,
                site_id = 1,
                url = "test",
                title = "Test page",
                canonical = "",
                page_title = "Test page title",
                meta_description = "A test page description",
                main_content = "A main content on a test page",
                additional_content = "Additional test content",
                layout = "",
                published_date = new System.DateTime(),
                content_type = 0,
                status = 0,
                redirect_url = "",
            };
            context.ContentModels.Add(page);
        }
    }
}
