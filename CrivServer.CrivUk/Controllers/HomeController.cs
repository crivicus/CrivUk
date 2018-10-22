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

namespace CrivServer.CrivUk.Controllers
{
    [Authorize]
    public class HomeController : ControllerHeart//, IRouteConstraint
    {
        public HomeController(IConfiguration configuration, CrivDbContext context) : base(configuration, context) { }

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
            model.PageContent = GetContent();
            return View(model);
        }

        [HttpGet("contact")]
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";
            var model = _crivContext.Users.Select(x => new { userid = x.Id ,user = x.UserName, pass = x.PasswordHash });
            var claims = _crivContext.UserClaims.Select(x=> new { uid = x.UserId, uclaim = x.ClaimValue, uctype = x.ClaimType});
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
