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
            return View(ReCreateModel(new DbContentModel(), "creation"));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreationViewModel createmodel)
        {
            var toAdd = createmodel.CreationModel;
            try
            {                
                _crivContext.ContentModels.Add(toAdd);
                await _crivContext.SaveChangesAsync();
            }
            catch(Exception e) {
                _logger.LogError(e, "Creation Create Error", toAdd);
                return View(ReCreateModel(toAdd, "creation"));
            }
            return RedirectToAction("Edit", new { id = toAdd.content_id });
        }
        #endregion

        #region Edit Creations
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var editPage = _crivContext.ContentModels.Find(id);
            return View(ReCreateModel(editPage, "creation"));
        }
        
        [HttpPost]
        public async Task<IActionResult> Edit(CreationViewModel editmodel)
        {
            var toEdit = editmodel.CreationModel;
            try
            {
                _crivContext.ContentModels.Update(toEdit);
                await _crivContext.SaveChangesAsync();
            }
            catch(Exception e) {
                _logger.LogError(e, "Creation Edit Error", toEdit);
                return View(ReCreateModel(toEdit, "creation"));
            }
            return RedirectToAction("Edit", new { id = toEdit.content_id});
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
            catch { return RedirectToAction("Index"); }
            return RedirectToAction("Index");
        }

        private CreationViewModel ReCreateModel(DbContentModel createmodel, string pagecontent) {
            var model = new CreationViewModel();
            model.CreationModel = createmodel;
            model.PageContent = GetContent(pagecontent);
            //Set viewbag items
            SetViewBag(model.PageContent);
            return model;
        }
    }
}
