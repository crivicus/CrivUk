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

        [AllowAnonymous]
        [HttpGet("about")]
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";
            ViewData["Title"] = "About";
            var model = new HomeViewModel();
            model.PageContent = GetContent("about");
            SetViewBag(model.PageContent);
            return View(model);
        }

        [AllowAnonymous]
        [HttpGet("contact")]
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";
            ViewData["Title"] = "Contact";
            return View();
        }

        [AllowAnonymous]
        [HttpGet("privacy")]
        public IActionResult Privacy()
        {
            ViewData["Title"] = "Privacy Policy";
            return View();
        }

        [AllowAnonymous]
        [HttpGet("chat")]
        public IActionResult ChatPage()
        {
            ViewData["Message"] = "Your chat page.";
            ViewData["Title"] = "Have a chat";
            return View();
        }

        [AllowAnonymous]
        [HttpGet("random")]
        public IActionResult Random()
        {
            var model = new HomeViewModel();
            model.PageContent = GetContent("random");

            var folder = System.IO.Path.Combine(AppDomain.CurrentDomain.GetData("WebRoot").ToString(), "images", "random").ToString();

            string[] fileEntries = System.IO.Directory.GetFiles(folder);

            model.image_url = Randomise(fileEntries).Replace(AppDomain.CurrentDomain.GetData("WebRoot").ToString(),"");
            model.image_name = model.image_url.Split(System.IO.Path.DirectorySeparatorChar).Last().Split('.').First().Replace('_', ' ').Replace('-', ' ');

            SetViewBag(model.PageContent);
            return View(model);
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
