using CrivServer.Data.Contexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CrivServer.Infrastructure.Extensions
{
    public static class InjectionExtension
    {
        public static IServiceCollection ConfigureDataService(this IServiceCollection services, IConfiguration _config, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                services.AddDbContext<CrivDbContext>(opt => opt.UseInMemoryDatabase("crivdbtest"));
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
                var testdb = app.ApplicationServices.GetService<CrivDbContext>();
                
                AddTestData(testdb, _config);
            }
            else
            {
                app.ApplicationServices.GetService<CrivDbContext>().Database.Migrate();
            }
            return app;
        }

        private static void AddTestData(CrivDbContext context, IConfiguration _config)
        {
            var testuser = _config.GetSection("TestUser");
            var user = new CrivServer.Data.Models.ApplicationUser {
                Id = "1",
                UserName = testuser.GetValue<string>("UserName"),
                Email = testuser.GetValue<string>("Email"),
                NormalizedUserName = testuser.GetValue<string>("NormalizedUserName"),
                NormalizedEmail = testuser.GetValue<string>("NormalizedEmail"),
                PasswordHash = testuser.GetValue<string>("PasswordHash"),
                UserType = testuser.GetValue<int>("UserType"),
                SecurityStamp = Guid.NewGuid().ToString()
            };
            context.ApplicationUsers.Add(user);

            var role = new Microsoft.AspNetCore.Identity.IdentityRole() { Id = "1", Name = "Creator", NormalizedName="creator", ConcurrencyStamp = Guid.NewGuid().ToString() };
            context.Roles.Add(role);

            var userrole = new Microsoft.AspNetCore.Identity.IdentityUserRole<string>() { RoleId = "1", UserId = "1"};
            context.UserRoles.Add(userrole);

            var page = new CrivServer.Data.Models.DbContentModel {
                content_id = 1,
                site_id = 1,
                url = "test",
                tab_title = "Test page",
                canonical = "",
                page_title = "Test page title",
                meta_description = "A test page description",
                content_h1 = "h1 test 1",
                main_content = "A main content on a test page",
                additional_content = "Additional test content",
                layout = "",
                published_date = new System.DateTime(),
                content_type = 0,
                status = 1,
                redirect_url = "",
                auth_level = null
            };
            context.ContentModels.Add(page);
            var page2 = new CrivServer.Data.Models.DbContentModel
            {
                content_id = 2,
                site_id = 1,
                url = "test-2",
                tab_title = "Test page 2",
                canonical = "",
                page_title = "Test page2 title",
                meta_description = "A test page2 description",
                content_h1 = "h1 test 2",
                main_content = "A main content on a test page2",
                additional_content = "Additional test content",
                layout = "",
                published_date = new System.DateTime(),
                content_type = 0,
                status = 1,
                redirect_url = "",
                auth_level = 0
            };
            context.ContentModels.Add(page2);
            var aboutpage = new CrivServer.Data.Models.DbContentModel
            {
                content_id = 3,
                site_id = 1,
                url = "about",
                tab_title = "About page",
                canonical = "",
                page_title = "About page title",
                meta_description = "An about page description",
                content_h1 = "h1 about page",
                main_content = "A main content on the about page",
                additional_content = "Additional about content",
                layout = "",
                published_date = new System.DateTime(),
                content_type = 0,
                status = 1,
                redirect_url = "",
                auth_level = 1
            };
            context.ContentModels.Add(aboutpage);

            var creationpage = new CrivServer.Data.Models.DbContentModel
            {
                content_id = 4,
                site_id = 1,
                url = "creation",
                tab_title = "creation page",
                canonical = "",
                page_title = "creation page title",
                meta_description = "An creation page description",
                content_h1 = "h1 creation page",
                main_content = "A main content on the creation page",
                additional_content = "Additional creation content",
                layout = "",
                published_date = new System.DateTime(),
                content_type = 0,
                status = 1,
                redirect_url = "",
                auth_level = 1
            };
            context.ContentModels.Add(creationpage);
            context.SaveChanges();
        }
    }
}
