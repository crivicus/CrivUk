﻿using CrivServer.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CrivServer.Data.Contexts
{
    public class CrivDbContext : IdentityDbContext<ApplicationUser>
    {
        public CrivDbContext() { }
        public CrivDbContext(DbContextOptions<CrivDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<DbContentModel> ContentModels { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable(name: "AspNetUser", schema: "crivdb");
                entity.Property(e => e.Id).HasColumnName("AspNetUserId");
                entity.Property(e => e.UserType).HasColumnName("UserType");
                entity.Property(e => e.UserFolder).HasColumnName("UserFolder");
                entity.Property(e => e.AvatarImage).HasColumnName("AvatarImage");
            });

            builder.Entity<DbContentModel>(entity =>
            {
                entity.ToTable(name: "site_content", schema: "crivdb");
                entity.Property(e => e.content_id).HasColumnName("content_id");
                entity.Property(e => e.site_id).HasColumnName("site_id");
                entity.Property(e => e.parent_id).HasColumnName("parent_id");
                entity.Property(e => e.parent_url).HasColumnName("parent_url");
                entity.Property(e => e.url).HasColumnName("url");
                entity.Property(e => e.tab_title).HasColumnName("tab_title");
                entity.Property(e => e.canonical).HasColumnName("canonical");
                entity.Property(e => e.page_title).HasColumnName("page_title");
                entity.Property(e => e.meta_description).HasColumnName("meta_description");
                entity.Property(e => e.content_h1).HasColumnName("content_h1");
                entity.Property(e => e.main_content).HasColumnName("main_content");
                entity.Property(e => e.additional_content).HasColumnName("additional_content");
                entity.Property(e => e.layout).HasColumnName("layout");
                entity.Property(e => e.published_date).HasColumnName("published_date");
                entity.Property(e => e.content_type).HasColumnName("content_type");
                entity.Property(e => e.status).HasColumnName("status");
                entity.Property(e => e.redirect_url).HasColumnName("redirect_url");
                entity.Property(e => e.auth_level).HasColumnName("auth_level");
            });
        }
    }
}
