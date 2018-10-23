using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CrivServer.CrivUk.Models;
using Microsoft.AspNetCore.Authorization;
using CrivServer.Infrastructure.ControllerHelpers;
using Microsoft.Extensions.Configuration;
using CrivServer.Data.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace CrivServer.CrivUk.Controllers
{
    [Authorize]
    public class HomeController : ControllerHeart
    {
        public HomeController(IConfiguration configuration, CrivDbContext context, ILoggerFactory logger) : base(configuration, context, logger) { }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("about")]
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";
            var model = new HomeViewModel();
            model.PageContent = GetContent("about");
            SetViewBag(model.PageContent);
            return View(model);
        }

        [HttpGet("contact")]
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        [AllowAnonymous]
        [HttpGet("privacy")]
        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
