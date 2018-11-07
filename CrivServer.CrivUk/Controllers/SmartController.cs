using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrivServer.Infrastructure.ControllerHelpers;
using Microsoft.AspNetCore.Mvc;

namespace CrivServer.CrivUk.Controllers
{
    public class SmartController : ControllerHeart
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
    }
}
