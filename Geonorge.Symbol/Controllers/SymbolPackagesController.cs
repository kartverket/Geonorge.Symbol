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
using System.Threading;
using System.Security.Claims;
using Geonorge.AuthLib.Common;

namespace Geonorge.Symbol.Controllers
{
    [HandleError]
    public class SymbolPackagesController : Controller
    {
        ISymbolService _symbolService;
        private readonly IAuthorizationService _authorizationService;

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public SymbolPackagesController(ISymbolService symbolService, IAuthorizationService authorizationService)
        {
            _symbolService = symbolService;
            _authorizationService = authorizationService;
        }

        // GET: SymbolPackages
        public ActionResult Index()
        {
            return View(_symbolService.GetPackagesWithAccessControl());
        }

        // GET: SymbolPackages/Details/5
        public ActionResult Details(Guid? systemid, string sortOrder)
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

            switch (sortOrder)
            {
                case "symbolname_desc":
                    symbolPackage.Symbols = symbolPackage.Symbols.OrderByDescending(s => s.Name).ToList();
                    break;
                case "owner":
                    symbolPackage.Symbols = symbolPackage.Symbols.OrderBy(s => s.Owner).ToList();
                    break;
                case "owner_desc":
                    symbolPackage.Symbols = symbolPackage.Symbols.OrderByDescending(s => s.Owner).ToList();
                    break;
                case "theme_desc":
                    symbolPackage.Symbols = symbolPackage.Symbols.OrderByDescending(s => s.Theme).ToList();
                    break;
                case "theme":
                    symbolPackage.Symbols = symbolPackage.Symbols.OrderBy(s => s.Theme).ToList();
                    break;
                default:
                    symbolPackage.Symbols = symbolPackage.Symbols.OrderBy(s => s.Name).ToList();
                    break;
            }
            if (string.IsNullOrEmpty(sortOrder))
                sortOrder = "symbolname";

            ViewBag.SymbolnameSortParm = sortOrder == "symbolname" ? "symbolname_desc" : "symbolname";
            ViewBag.Owner = sortOrder == "owner" ? "owner_desc" : "owner";
            ViewBag.Theme = sortOrder == "theme" ? "theme_desc" : "theme";
            ViewBag.SortOrder = sortOrder;


            return View(symbolPackage);
        }

        // GET: SymbolPackages/Create
        public ActionResult Create()
        {
            SymbolPackage symbolPackage = new SymbolPackage();
            symbolPackage.Owner = ClaimsPrincipal.Current.GetOrganizationName();
            ViewBag.IsAdmin = false;
            if (Request.IsAuthenticated)
            {
                ViewBag.IsAdmin = _authorizationService.IsAdmin();
            }
            ViewBag.Themes = new SelectList(CodeList.Themes(), "Key", "Value", "Annen");
            return View(symbolPackage);
        }

        // POST: SymbolPackages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SymbolPackage symbolPackage)
        {
            ViewBag.IsAdmin = false;
            if (Request.IsAuthenticated)
            {
                ViewBag.IsAdmin = _authorizationService.IsAdmin();
            }

            if (!ViewBag.IsAdmin)
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

            ViewBag.Themes = new SelectList(CodeList.Themes(), "Key", "Value", symbolPackage.Theme);

            if (ModelState.IsValid)
            {
                try
                {
                    _symbolService.AddPackage(symbolPackage);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    ModelState.AddModelError("error",ex.Message);
                }
            }

            return View(symbolPackage);
        }

        // GET: SymbolPackages/Edit/5
        [Authorize]
        public ActionResult Edit(Guid? systemid)
        {
            ViewBag.IsAdmin = false;
            if (Request.IsAuthenticated)
            {
                ViewBag.IsAdmin = _authorizationService.IsAdmin();
            }

            if (systemid == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            if (!ViewBag.IsAdmin)
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

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
            ViewBag.IsAdmin = false;
            if (Request.IsAuthenticated)
                ViewBag.IsAdmin = _authorizationService.IsAdmin();

            if (!ViewBag.IsAdmin)
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

            SymbolPackage symbolPackageOriginal = _symbolService.GetPackage(symbolPackage.SystemId);

            ViewBag.Themes = new SelectList(CodeList.Themes(), "Key", "Value", symbolPackage.Theme);

            if (ModelState.IsValid)
            {
                try
                {
                    _symbolService.UpdatePackage(symbolPackageOriginal, symbolPackage);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    ModelState.AddModelError("error", ex.Message);
                }               
            }
            return View(symbolPackage);
        }

        // GET: SymbolPackages/Delete/5
        [Authorize]
        public ActionResult Delete(Guid? systemid)
        {
            if (systemid == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

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
            Models.SymbolPackage symbolPackage = _symbolService.GetPackage(systemid);

            ViewBag.IsAdmin = false;
            if (Request.IsAuthenticated)
                ViewBag.IsAdmin = _authorizationService.IsAdmin();

            if (!ViewBag.IsAdmin)
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

            _symbolService.RemovePackage(systemid);

            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult TiffCompressWithZip()
        {
            string targetFolder = System.Web.HttpContext.Current.Server.MapPath("~/files");

            if (!_authorizationService.IsAdmin())
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

            new Thread(() => new BatchService(targetFolder).TiffCompressWithZip()).Start();

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }


        public object Download(Guid? systemid)
        {

            var package = _symbolService.GetPackage(systemid.Value);
            string targetFolder = System.Web.HttpContext.Current.Server.MapPath("~/files/");

            //zip exists?
            bool zipExists = false;
            var packageFolder = package.Symbols[0].SymbolPackages.FirstOrDefault()?.Folder;
            if (!string.IsNullOrEmpty(packageFolder))
            { 
                packageFolder = ImageService.MakeSeoFriendlyString(packageFolder);
                var zipFile = targetFolder + packageFolder + "\\" + packageFolder + ".zip";
                if (System.IO.File.Exists(zipFile))
                {
                    zipExists = true;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    Response.Cookies.Add(new HttpCookie("downloadStarted", "1") { Expires = DateTime.Now.AddSeconds(10) });
                    return File("~/files/" + packageFolder + "/" + packageFolder + ".zip", "application/zip", packageFolder + ".zip");      
                }

            }

            if (!zipExists)
            {
                using (ZipFile zip = new ZipFile())
                {
                    foreach (var symbol in package.Symbols)
                    {
                        string folder = targetFolder;
                        if (!string.IsNullOrEmpty(symbol.SymbolPackages.FirstOrDefault()?.Folder))
                            folder = folder + symbol.SymbolPackages.FirstOrDefault()?.Folder + "\\";

                        foreach (var file in symbol.SymbolFiles)
                        {
                            zip.AddFile(folder + file.FileName, symbol.Name + @"\" + file.SymbolFileVariant.Name);
                        }
                    }

                    Response.ClearContent();
                    Response.ClearHeaders();
                    Response.ContentType = "application/zip";
                    Response.Cookies.Add(new HttpCookie("downloadStarted", "1") { Expires = DateTime.Now.AddSeconds(59) });
                    Response.AppendHeader("content-disposition", "attachment; filename=" + ImageService.MakeSeoFriendlyString(package.Name) + ".zip");

                    zip.Save(Response.OutputStream);
                    Response.Flush();
                }
            }

            return null;

        }

        protected override void OnException(ExceptionContext filterContext)
        {
            Log.Error("Error", filterContext.Exception);
        }

    }
}
