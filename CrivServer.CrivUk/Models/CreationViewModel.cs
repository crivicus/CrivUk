using CrivServer.Data.Enums;
using CrivServer.Data.Models;
using CrivServer.Infrastructure.ModelHelpers;
using CrivServer.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrivServer.CrivUk.Models
{
    public class CreationViewModel : ModelHeart
    {
        public CreationViewModel() { this.StatusTypeList = UtilityExtensions.GetEnumSelectList<StatusTypes>(); }

        public DbContentModel CreationModel { get; set; }

        public IEnumerable<DbContentModel> CreationModelList { get; set; }

        public IEnumerable<SelectListItem> StatusTypeList { get; set; }
    }
}
