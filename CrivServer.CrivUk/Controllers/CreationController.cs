using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrivServer.CrivUk.Models;
using CrivServer.Data.Contexts;
using CrivServer.Data.Models;
using CrivServer.Infrastructure.ControllerHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CrivServer.CrivUk.Controllers
{
    [Authorize(Roles = "Creator")]
    public class CreationController : ControllerHeart
    {
        public CreationController(IConfiguration configuration, CrivDbContext context, ILoggerFactory logger) : base(configuration, context, logger) { }

        public IActionResult Index()
        {
            var model = new CreationViewModel();
            model.CreationModelList = _crivContext.ContentModels.ToList();
            model.PageContent = GetContent("creation");

            //Set viewbag items
            SetViewBag(model.PageContent);

            return View(model);
        }

        #region Create Creations
        [HttpGet]
        public IActionResult Create()
        {
            var model = new CreationViewModel();
            model.CreationModel = new DbContentModel();
            model.PageContent = GetContent("creation");
            //Set viewbag items
            SetViewBag(model.PageContent);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(DbContentModel model)
        {
            var toAdd = model;
            try
            {                
                _crivContext.ContentModels.Add(toAdd);
                await _crivContext.SaveChangesAsync();
            }
            catch { return View(); }
            return RedirectToAction("Edit", toAdd.content_id);
        }
        #endregion

        #region Edit Creations
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var model = new CreationViewModel();
            model.CreationModel = _crivContext.ContentModels.Find(id);
            model.PageContent = GetContent("creation");
            //Set viewbag items
            SetViewBag(model.PageContent);

            return View(model);
        }
        
        [HttpPost]
        public async Task<IActionResult> EditAsync(DbContentModel model)
        {
            var toEdit = model;
            try
            {
                _crivContext.ContentModels.Update(toEdit);
                await _crivContext.SaveChangesAsync();
            }
            catch { return View(); }
            return RedirectToAction("Edit", toEdit.content_id);
        }
        #endregion

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try {
                var toDelete = _crivContext.ContentModels.FirstOrDefault(x => x.content_id.Equals(id));
                _crivContext.ContentModels.Remove(toDelete);
                await _crivContext.SaveChangesAsync();
            }
            catch { return View(); }
            return RedirectToAction("Index");
        }
    }
}
