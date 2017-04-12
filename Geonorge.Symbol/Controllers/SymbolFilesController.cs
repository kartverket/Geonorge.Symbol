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
    public class SymbolFilesController : Controller
    {
        ISymbolService _symbolService;
        private readonly IAuthorizationService _authorizationService;

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
        public ActionResult Create(SymbolFile symbolFile, HttpPostedFileBase uploadFile, bool autogenererFraSvg = false)
        {
            ViewBag.Size = new SelectList(CodeList.Size, "Key", "Value", symbolFile.Size);
            ViewBag.SymbolGraphics = new SelectList(CodeList.SymbolGraphics, "Key", "Value", symbolFile.Type);
            if (ModelState.IsValid)
            {
                if(autogenererFraSvg)
                    _symbolService.AddSymbolFilesFromSvg(symbolFile, uploadFile);
                else
                    _symbolService.AddSymbolFile(symbolFile, uploadFile);

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
            SymbolFile symbolFile = _symbolService.GetSymbolFile(systemid.Value);
            if (symbolFile == null)
            {
                return HttpNotFound();
            }
            return View(symbolFile);
        }

        // POST: SymbolFiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit(SymbolFile symbolFile)
        {
            if (ModelState.IsValid)
            {
                var originalSymbolFile = _symbolService.GetSymbolFile(symbolFile.SystemId);
                _symbolService.UpdateSymbolFile(originalSymbolFile, symbolFile);
                return RedirectToAction("Index");
            }
            return View(symbolFile);
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
            var list = _symbolService.GetSymbolVariant(systemid.Value);
            var symbolFiles = _symbolService.GetSymbolVariant(systemid.Value).ToList();
            var symbolId = symbolFiles[0].Symbol.SystemId;
            if (_authorizationService.HasAccess(symbolFiles[0].Symbol.Owner,
                    _authorizationService.GetSecurityClaim("organization").FirstOrDefault()))
            {
                foreach (var file in symbolFiles)
                {
                    _symbolService.RemoveSymbolFile(file);
                }

                _symbolService.RemoveSymbolFileVariant(list[0].SymbolFileVariant);
            }
            else { return new HttpStatusCodeResult(HttpStatusCode.Unauthorized); }
            return RedirectToAction("Details", "Files", new { systemid = symbolId });
        }
    }
}
