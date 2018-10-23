using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CrivServer.CrivUk.Models;
using CrivServer.Data.Contexts;
using CrivServer.Infrastructure.ControllerHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CrivServer.CrivUk.Controllers
{
    public class ContentController : ControllerHeart//, IRouteConstraint
    {
        public ContentController(IConfiguration configuration, CrivDbContext context, ILoggerFactory logger) : base(configuration, context, logger) { }

        public IActionResult Index(string[] url)
        {
            var uri = url.Last();
            //404 if no page is found
            if (!UriCheck(uri))
                return NotFound();

            var model = new ContentViewModel() { PageContent = GetContent(uri) };

            if (CheckRedirect(uri, out var redirect, out var permanent)) return permanent? RedirectPermanent(redirect): Redirect(redirect);

            if(model.PageContent.auth_level.GetValueOrDefault()>0 && !User.Identity.IsAuthenticated)
                return Unauthorized();

            var viewName = "TypeA";

            if (!string.IsNullOrWhiteSpace(model.PageContent.layout))
                viewName = model.PageContent.layout;
            //Set viewbag items
            ViewBag.MetaDescription = model.PageContent.meta_description;
            ViewBag.Canonical = model.PageContent.canonical;
            ViewBag.Title = model.PageContent.tab_title;

            return View(viewName,model);
        }
    }
}