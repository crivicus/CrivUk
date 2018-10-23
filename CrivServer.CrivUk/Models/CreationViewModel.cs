using CrivServer.Data.Models;
using CrivServer.Infrastructure.ModelHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrivServer.CrivUk.Models
{
    public class CreationViewModel : ModelHeart
    {
        public DbContentModel CreationModel { get; set; }

        public IEnumerable<DbContentModel> CreationModelList { get; set; }
    }
}
