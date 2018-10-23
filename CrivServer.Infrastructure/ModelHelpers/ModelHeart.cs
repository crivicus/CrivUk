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
    }
}
