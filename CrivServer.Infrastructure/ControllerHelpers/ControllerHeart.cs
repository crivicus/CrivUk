using CrivServer.Data.Contexts;
using CrivServer.Data.Enums;
using CrivServer.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrivServer.Infrastructure.ControllerHelpers
{
    public abstract class ControllerHeart:Controller
    {
        protected readonly IConfiguration _crivConfig;
        protected readonly CrivDbContext _crivContext;
        protected readonly ILogger _logger;
        protected readonly int SiteId;

        public ControllerHeart() { }
        public ControllerHeart(IConfiguration configuration, CrivDbContext context, ILoggerFactory logger)
        {
            _crivConfig = configuration;
            _crivContext = context;
            _logger = logger.CreateLogger<ControllerHeart>();
            SiteId = 1; //ToDo: configure dynamic site id
        }

        public bool UriCheck(string uri) {
            if (_crivContext != null && _crivContext.ContentModels.Any(x => x.url.Equals(uri)))
            {
                if (_crivContext.ContentModels.Any(x => x.status.GetValueOrDefault() > 0)) return true;
            }
            return false;
        }

        public DbContentModel GetContent() { var uri = HttpContext.Request.Path.ToUriComponent(); return GetContent(uri); }
        public DbContentModel GetContent(string uri) { return _crivContext != null ? _crivContext.ContentModels.Where(x=>x.site_id.Equals(SiteId)).FirstOrDefault(x => x.url.Equals(uri)) : null; }
        
        public int AllowAnon(string uri) { return UriCheck(uri) ? _crivContext.ContentModels.Where(x => x.site_id.Equals(SiteId)).FirstOrDefault(x => x.url.Equals(uri)).auth_level.GetValueOrDefault() : 0; }
        public bool IsRedirect(int? status) { return (status.GetValueOrDefault().Equals(StatusTypes.Redirect) || status.GetValueOrDefault().Equals(StatusTypes.RedirectPermanent)); }
        public bool CheckRedirect(string uri, out string redirect, out bool redirectPermanent)
        {
            if (UriCheck(uri))
            {
                var content = _crivContext.ContentModels.Where(x => x.site_id.Equals(SiteId)).FirstOrDefault(x => x.url.Equals(uri));
                if (!string.IsNullOrWhiteSpace(content.redirect_url) && IsRedirect(content.status))
                {
                    redirect = content.redirect_url;
                    redirectPermanent = content.status.GetValueOrDefault().Equals(StatusTypes.RedirectPermanent);
                    return true;
                }
            }
            redirect = null;
            redirectPermanent = false;
            return false;
        }

        public void SetViewBag(DbContentModel model)
        {
            ViewBag.MetaDescription = model.meta_description;
            ViewBag.Canonical = model.canonical;
            ViewBag.Title = model.tab_title;
        }
    }
}
