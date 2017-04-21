using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Geonorge.Symbol.Models;
using Geonorge.Symbol.Services;

namespace Geonorge.Symbol.Controllers
{
    [HandleError]
    public class SymbolFilesController : Controller
    {
        ISymbolService _symbolService;
        private readonly IAuthorizationService _authorizationService;

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public SymbolFilesController(ISymbolService symbolService, IAuthorizationService authorizationService)
        {
            _symbolService = symbolService;
            _authorizationService = authorizationService;
        }

        // GET: SymbolFiles/Create
        public ActionResult Create(Guid systemid)
        {
            ViewBag.Size = new SelectList(CodeList.Size, "Key", "Value");
            ViewBag.SymbolGraphics = new SelectList(CodeList.SymbolGraphics, "Key", "Value");
            SymbolFile symbolFile = new SymbolFile();
            symbolFile.Symbol = _symbolService.GetSymbol(systemid);

            return View(symbolFile);
        }

        // POST: SymbolFiles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create(SymbolFile symbolFile, HttpPostedFileBase[] uploadFiles, bool autogenererFraSvg = false)
        {
            ViewBag.Size = new SelectList(CodeList.Size, "Key", "Value", symbolFile.Size);
            ViewBag.SymbolGraphics = new SelectList(CodeList.SymbolGraphics, "Key", "Value", symbolFile.Type);
            if (ModelState.IsValid)
            {
                if(autogenererFraSvg)
                    _symbolService.AddSymbolFilesFromSvg(symbolFile, uploadFiles[0]);
                else
                    _symbolService.AddSymbolFiles(symbolFile, uploadFiles);


                return RedirectToAction("Details", "Files", new { systemid = symbolFile.Symbol.SystemId });
            }

            return View(symbolFile);
        }

        // GET: SymbolFiles/Edit/5
        public ActionResult Edit(Guid? systemid)
        {
            if (systemid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<SymbolFile> symbolFiles = _symbolService.GetSymbolVariant(systemid.Value);
            if (symbolFiles == null)
            {
                return HttpNotFound();
            }
            return View(symbolFiles);
        }

        // POST: SymbolFiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit(SymbolFile symbolFile, HttpPostedFileBase[] uploadFile, string FileToRemove)
        {
            var variants = _symbolService.GetSymbolVariant(symbolFile.SymbolFileVariant.SystemId);

            if (!_authorizationService.HasAccess(variants[0].Symbol.Owner,
                    _authorizationService.GetSecurityClaim("organization").FirstOrDefault()))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }

            foreach (var variant in variants)
            {
                variant.SymbolFileVariant.Name = symbolFile.SymbolFileVariant.Name;
                _symbolService.UpdateSymbolFile(variant);
            }

            if (uploadFile != null)
            {
                var file = variants.FirstOrDefault();
                symbolFile.SymbolFileVariant = file.SymbolFileVariant;
                symbolFile.Symbol = file.Symbol;
                _symbolService.AddSymbolFiles(symbolFile, uploadFile);
            }

            if (!string.IsNullOrEmpty(FileToRemove))
            {
                var fileToRemove = _symbolService.GetSymbolFile(Guid.Parse(FileToRemove));
                _symbolService.RemoveSymbolFile(fileToRemove);
            }

            variants = _symbolService.GetSymbolVariant(symbolFile.SymbolFileVariant.SystemId);

            return View(variants);
        }

        // GET: SymbolFiles/Delete/5
        public ActionResult Delete(Guid? systemid)
        {
            if (systemid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var symbolFiles = _symbolService.GetSymbolVariant(systemid.Value);
            if (symbolFiles == null)
            {
                return HttpNotFound();
            }
            return View(symbolFiles);
        }

        // POST: SymbolFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(Guid? systemid)
        {
            var variant = _symbolService.GetSymbolVariant(systemid.Value).FirstOrDefault().SymbolFileVariant;
            var symbolFiles = _symbolService.GetSymbolVariant(systemid.Value).ToList();
            var symbolId = symbolFiles[0].Symbol.SystemId;
            if (_authorizationService.HasAccess(symbolFiles[0].Symbol.Owner,
                    _authorizationService.GetSecurityClaim("organization").FirstOrDefault()))
            {
                foreach (var file in symbolFiles)
                {
                    _symbolService.RemoveSymbolFile(file);
                }

                _symbolService.RemoveSymbolFileVariant(variant);
            }
            else { return new HttpStatusCodeResult(HttpStatusCode.Unauthorized); }
            return RedirectToAction("Details", "Files", new { systemid = symbolId });
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            Log.Error("Error", filterContext.Exception);
        }
    }
}
