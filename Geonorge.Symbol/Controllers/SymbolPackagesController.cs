﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Geonorge.Symbol.Models;
using Geonorge.Symbol.Services;
using System.IO;
using Ionic.Zip;

namespace Geonorge.Symbol.Controllers
{
    public class SymbolPackagesController : Controller
    {
        ISymbolService _symbolService;
        private readonly IAuthorizationService _authorizationService;

        public SymbolPackagesController(ISymbolService symbolService, IAuthorizationService authorizationService)
        {
            _symbolService = symbolService;
            _authorizationService = authorizationService;
        }

        // GET: SymbolPackages
        public ActionResult Index()
        {
            return View(_symbolService.GetPackages());
        }

        // GET: SymbolPackages/Details/5
        public ActionResult Details(Guid? systemid)
        {
            if (systemid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SymbolPackage symbolPackage = _symbolService.GetPackage(systemid.Value);
            if (symbolPackage == null)
            {
                return HttpNotFound();
            }
            return View(symbolPackage);
        }

        // GET: SymbolPackages/Create
        public ActionResult Create()
        {
            ViewBag.Themes = new SelectList(CodeList.Themes(), "Key", "Value", "Annen");
            return View();
        }

        // POST: SymbolPackages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SymbolPackage symbolPackage)
        {
            ViewBag.Themes = new SelectList(CodeList.Themes(), "Key", "Value", symbolPackage.Theme);

            if (ModelState.IsValid)
            {
                _symbolService.AddPackage(symbolPackage);
                return RedirectToAction("Index");
            }

            return View(symbolPackage);
        }

        // GET: SymbolPackages/Edit/5
        public ActionResult Edit(Guid? systemid)
        {
            if (systemid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SymbolPackage symbolPackage = _symbolService.GetPackage(systemid.Value);
            if (symbolPackage == null)
            {
                return HttpNotFound();
            }

            ViewBag.Themes = new SelectList(CodeList.Themes(), "Key", "Value", symbolPackage.Theme);

            return View(symbolPackage);
        }

        // POST: SymbolPackages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SymbolPackage symbolPackage)
        {
            if (ModelState.IsValid)
            {
                _symbolService.UpdatePackage(symbolPackage);
                return RedirectToAction("Index");
            }
            return View(symbolPackage);
        }

        // GET: SymbolPackages/Delete/5
        public ActionResult Delete(Guid? systemid)
        {
            if (systemid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SymbolPackage symbolPackage = _symbolService.GetPackage(systemid.Value);
            if (symbolPackage == null)
            {
                return HttpNotFound();
            }
            return View(symbolPackage);
        }

        // POST: SymbolPackages/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid systemid)
        {
            _symbolService.RemovePackage(systemid);

            return RedirectToAction("Index");
        }


        public FileResult Download(Guid? systemid)
        {
            var package = _symbolService.GetPackage(systemid.Value);
            string targetFolder = System.Web.HttpContext.Current.Server.MapPath("~/files/");

            using (ZipFile zip = new ZipFile())
            {
                foreach (var symbol in package.Symbols)
                {
                    foreach (var file in symbol.SymbolFiles)
                    {
                        zip.AddFile(targetFolder + file.FileName, symbol.Name + @"\" + file.SymbolFileVariant.Name);
                    }
                }

                MemoryStream output = new MemoryStream();
                zip.Save(output);
                output.Position = 0;

            return File(output, "application/zip", ImageService.MakeSeoFriendlyString(package.Name) + ".zip");
            }

        }

    }
}
