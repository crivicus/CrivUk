using CrivServer.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrivServer.Infrastructure.ModelHelpers
{
    public abstract class ModelHeart
    {
        public DbContentModel PageContent { get; set; }

        public int SiteId { get; set; }

        public IList<NavbarItems> NavBar { get; set; }
    }

    public class NavbarItems
    {
        public string Navlink { get; set; }
        public string Navname { get; set; }
        public string Navicon { get; set; }
        public string Navauth { get; set; }
    }
}
