using CrivServer.Infrastructure.ModelHelpers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrivServer.CrivUk.Models
{
    public class HomeViewModel:ModelHeart
    {
        public string image_url { get; set; }
        public string image_name { get; set; }

        public List<IFormFile> files { get; set; }
    }
}
