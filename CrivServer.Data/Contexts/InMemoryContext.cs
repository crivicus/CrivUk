using CrivServer.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrivServer.Data.Contexts
{
    public class InMemoryContext : IdentityDbContext<ApplicationUser>
    {
        public InMemoryContext() { }
        public InMemoryContext(DbContextOptions<InMemoryContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<DbContentModel> ContentModels { get; set; }
    }
}
