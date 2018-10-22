using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CrivServer.CrivUk.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CrivServer.CrivUk.Controllers
{
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Index(int? statusCode = null)
        {
            var feature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            ViewData["ErrorUrl"] = feature?.OriginalPath;
            if (statusCode.HasValue)
            {
                if (statusCode.Value == 403 || statusCode.Value == 404 || statusCode.Value == 500)
                {
                    var viewName = string.Format("{0}",statusCode.ToString());
                    return View(viewName);
                }                
                if (statusCode.Value == 401)
                    return Redirect("/Identity/Account/Login?ReturnUrl="+ feature?.OriginalPath);
            }
            return View();
        }
    }
}