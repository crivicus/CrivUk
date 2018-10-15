using System;
using System.Collections.Generic;
using System.Text;
using CrivServer.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CrivServer.Data.Contexts
{
    public class CrivDbContext : IdentityDbContext<ApplicationUser>
    {
        public CrivDbContext(DbContextOptions<CrivDbContext> options)
            : base(options)
        {
        }

        public DbSet<DbContentModel> ContentModels { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.Entity<DbContentModel>(entity =>
            {
                entity.ToTable(name: "site_content", schema: "crivdb");
                entity.Property(e => e.content_id).HasColumnName("content_id");
                entity.Property(e => e.site_id).HasColumnName("site_id");
                entity.Property(e => e.url).HasColumnName("url");
                entity.Property(e => e.title).HasColumnName("title");
                entity.Property(e => e.canonical).HasColumnName("canonical");
                entity.Property(e => e.page_title).HasColumnName("page_title");
                entity.Property(e => e.meta_description).HasColumnName("meta_description");
                entity.Property(e => e.main_content).HasColumnName("main_content");
                entity.Property(e => e.additional_content).HasColumnName("additional_content");
                entity.Property(e => e.layout).HasColumnName("layout");
                entity.Property(e => e.published_date).HasColumnName("published_date");
                entity.Property(e => e.content_type).HasColumnName("content_type");
                entity.Property(e => e.status).HasColumnName("status");
                entity.Property(e => e.redirect_url).HasColumnName("redirect_url");
            });
        }
    }
}
