using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Geonorge.Symbol.Services;
using Geonorge.Symbol.Models;
using PagedList;
using Ionic.Zip;
using System.IO;

namespace Geonorge.Symbol.Controllers
{
    [HandleError]
    public class FilesController : Controller
    {
        ISymbolService _symbolService;
        private readonly IAuthorizationService _authorizationService;

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public FilesController(ISymbolService symbolService, IAuthorizationService authorizationService)
        {
            _symbolService = symbolService;
            _authorizationService = authorizationService;
        }

        // GET: Files
        public ActionResult Index(string sortOrder, string text, int? page)
        {

            var symbols = _symbolService.GetSymbols(text);

            switch (sortOrder)
            {
                case "symbolname_desc":
                    symbols = symbols.OrderByDescending(s => s.Name).ToList();
                    break;
                case "owner":
                    symbols = symbols.OrderBy(s => s.Owner).ToList();
                    break;
                case "owner_desc":
                    symbols = symbols.OrderByDescending(s => s.Owner).ToList();
                    break;
                case "theme_desc":
                    symbols = symbols.OrderByDescending(s => s.Theme).ToList();
                    break;
                case "theme":
                    symbols = symbols.OrderBy(s => s.Theme).ToList();
                    break;
                default:
                    symbols = symbols.OrderBy(s => s.Name).ToList();
                    break;
            }
            if (string.IsNullOrEmpty(sortOrder))
                sortOrder = "symbolname";

            ViewBag.SymbolnameSortParm = sortOrder == "symbolname" ? "symbolname_desc" : "symbolname";
            ViewBag.Owner = sortOrder == "owner" ? "owner_desc" : "owner";
            ViewBag.Theme = sortOrder == "theme" ? "theme_desc" : "theme";
            ViewBag.SortOrder = sortOrder;
            ViewBag.text = text;
            ViewBag.SymbolsAll = symbols;
            ViewBag.Page = 0;

            int pageSize = 30;
            int pageNumber = (page ?? 1);

            ViewBag.IsAdmin = false;
            if (Request.IsAuthenticated)
                ViewBag.IsAdmin = _authorizationService.IsAdmin();

            return View(symbols);
        }

        // GET: Files/Details/5
        public ActionResult Details(Guid? systemid)
        {

            if (systemid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Models.Symbol symbol = _symbolService.GetSymbol(systemid.Value);

            if (symbol == null)
            {
                return HttpNotFound();
            }

            ViewBag.HasAccess = false;
            ViewBag.IsAdmin = false;
            if (Request.IsAuthenticated)
            {
                ViewBag.HasAccess = _authorizationService.HasAccess(symbol.Owner,
                    _authorizationService.GetSecurityClaim("organization").FirstOrDefault());
                ViewBag.IsAdmin = _authorizationService.IsAdmin();
            }

            return View(symbol);
        }

        // GET: Files/Create
        public ActionResult Create()
        {
            ViewBag.Themes = new SelectList(CodeList.Themes(), "Key", "Value", "Annen");
            ViewBag.SymbolPackages = new MultiSelectList(_symbolService.GetPackagesWithAccessControl(), "SystemId", "Name");
            ViewBag.IsAdmin = false;
            ViewBag.Owner = _authorizationService.GetSecurityClaim("organization").FirstOrDefault();
            if (Request.IsAuthenticated)
            {
                ViewBag.IsAdmin = _authorizationService.IsAdmin();
            }

            return View();
        }

        // POST: Files/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create(Models.Symbol symbol, HttpPostedFileBase uploadFile, string[] packages)
        {
            ViewBag.Themes = new SelectList(CodeList.Themes(), "Key", "Value", symbol.Theme);
            ViewBag.SymbolPackages = new SelectList(_symbolService.GetPackagesWithAccessControl(), "SystemId", "Name");
            symbol.SymbolPackages = new List<SymbolPackage>();
            if (packages != null)
            {
                foreach (var package in packages)
                    symbol.SymbolPackages.Add(_symbolService.GetPackage(Guid.Parse(package)));
            }

            ViewBag.IsAdmin = false;
            if (Request.IsAuthenticated)
            {
                ViewBag.IsAdmin = _authorizationService.IsAdmin();
            }

            if (ModelState.IsValid)
            {
                ImageService img = new ImageService();
                if(uploadFile != null)
                    symbol.Thumbnail = img.SaveThumbnail(uploadFile, symbol);

                var addedSymbol = _symbolService.AddSymbol(symbol);
                return RedirectToAction("Details", "Files", new { systemid = addedSymbol.SystemId });
            }

            return View(symbol);
        }

        // GET: Files/Edit/5
        public ActionResult Edit(Guid? systemid)
        {

            if (systemid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Models.Symbol symbol = _symbolService.GetSymbol(systemid.Value);

            if (symbol == null)
            {
                return HttpNotFound();
            }

            ViewBag.Themes = new SelectList(CodeList.Themes(), "Key", "Value", symbol.Theme);
            ViewBag.SymbolPackages = new MultiSelectList(_symbolService.GetPackagesWithAccessControl(), "SystemId", "Name", symbol.SymbolPackages.Select(c => c.SystemId).ToArray());

            ViewBag.IsAdmin = false;
            if (Request.IsAuthenticated)
            {
                ViewBag.IsAdmin = _authorizationService.IsAdmin();
            }

            return View(symbol);
        }

        // POST: Files/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.Symbol symbol, HttpPostedFileBase uploadFile, string[] packages)
        {
            symbol.SymbolPackages = new List<SymbolPackage>();
            if (packages != null)
            {
                foreach (var package in packages)
                    symbol.SymbolPackages.Add(_symbolService.GetPackage(Guid.Parse(package)));
            }
            ImageService img = new ImageService();
            if(uploadFile != null)
                symbol.Thumbnail = img.SaveThumbnail(uploadFile, symbol);

            Models.Symbol originalSymbol = _symbolService.GetSymbol(symbol.SystemId);

            ViewBag.IsAdmin = false;
            if (Request.IsAuthenticated)
            {
                ViewBag.IsAdmin = _authorizationService.IsAdmin();
            }

            if (!_authorizationService.HasAccess(originalSymbol.Owner,
                    _authorizationService.GetSecurityClaim("organization").FirstOrDefault()))
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);


            if (ModelState.IsValid)
            {
                _symbolService.UpdateSymbol(originalSymbol, symbol);

                return RedirectToAction("Index");
            }

            ViewBag.Themes = new SelectList(CodeList.Themes(), "Key", "Value", symbol.Theme);
            ViewBag.SymbolPackages = new MultiSelectList(_symbolService.GetPackagesWithAccessControl(), "SystemId", "Name", symbol.SymbolPackages.Select(c => c.SystemId).ToArray());


            return View(symbol);
        }

        // GET: Files/Delete/5
        public ActionResult Delete(Guid? systemid)
        {
            if (systemid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Models.Symbol symbol = _symbolService.GetSymbol(systemid.Value);

            if (symbol == null)
            {
                return HttpNotFound();
            }

            return View(symbol);
        }

        // POST: Files/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(Guid systemid)
        {

            Models.Symbol symbol = _symbolService.GetSymbol(systemid);

            bool hasAccess = _authorizationService.HasAccess(symbol.Owner,
                _authorizationService.GetSecurityClaim("organization").FirstOrDefault());

            if (!hasAccess)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }

            _symbolService.RemoveSymbol(symbol);
            return RedirectToAction("Index");
        }

        public FileResult Download(Guid? systemid)
        {
            var symbol = _symbolService.GetSymbol(systemid.Value);
            string targetFolder = System.Web.HttpContext.Current.Server.MapPath("~/files/");
            if (!string.IsNullOrEmpty(symbol.SymbolPackages.FirstOrDefault()?.Folder))
                targetFolder = targetFolder + symbol.SymbolPackages.FirstOrDefault()?.Folder + "\\";
            using (ZipFile zip = new ZipFile())
            {
                foreach (var file in symbol.SymbolFiles)
                {
                        zip.AddFile(targetFolder + file.FileName, file.SymbolFileVariant.Name);
                }
                MemoryStream output = new MemoryStream();
                zip.Save(output);
                output.Position = 0;

                return File(output, "application/zip", ImageService.MakeSeoFriendlyString(symbol.Name) + ".zip");
            }
        }

        public FileResult DownloadVariant(Guid? systemid)
        {
            var symbolFiles = _symbolService.GetSymbolVariant(systemid.Value);
            string targetFolder = System.Web.HttpContext.Current.Server.MapPath("~/files/");
            if (!string.IsNullOrEmpty(symbolFiles[0].Symbol.SymbolPackages.FirstOrDefault()?.Folder))
                targetFolder = targetFolder + "\\" + symbolFiles[0].Symbol.SymbolPackages.FirstOrDefault()?.Folder + "\\";

            using (ZipFile zip = new ZipFile())
            {
                foreach (var file in symbolFiles)
                {
                    zip.AddFile(targetFolder + file.FileName, "");
                }
                MemoryStream output = new MemoryStream();
                zip.Save(output);
                output.Position = 0;

                return File(output, "application/zip", ImageService.MakeSeoFriendlyString(symbolFiles[0].SymbolFileVariant.Name) + ".zip");
            }
        }

        public ActionResult SymbolList(int page, string sortOrder, string text)
        {
            var symbols = _symbolService.GetSymbols(text);
            switch (sortOrder)
            {
                case "symbolname_desc":
                    symbols = symbols.OrderByDescending(s => s.Name).ToList();
                    break;
                case "owner":
                    symbols = symbols.OrderBy(s => s.Owner).ToList();
                    break;
                case "owner_desc":
                    symbols = symbols.OrderByDescending(s => s.Owner).ToList();
                    break;
                case "theme_desc":
                    symbols = symbols.OrderByDescending(s => s.Theme).ToList();
                    break;
                case "theme":
                    symbols = symbols.OrderBy(s => s.Theme).ToList();
                    break;
                default:
                    symbols = symbols.OrderBy(s => s.Name).ToList();
                    break;
            }
            if (string.IsNullOrEmpty(sortOrder))
                sortOrder = "symbolname";

            ViewBag.SymbolnameSortParm = sortOrder == "symbolname" ? "symbolname_desc" : "symbolname";
            ViewBag.Owner = sortOrder == "owner" ? "owner_desc" : "owner";
            ViewBag.Theme = sortOrder == "theme" ? "theme_desc" : "theme";
            ViewBag.SortOrder = sortOrder;
            ViewBag.text = text;

            int rangeStart = 0;
            int rangeLength = 0;

            int pageSize = 30;
            int pageNumber = page;

            if ((pageNumber * pageSize) > symbols.Count)
                return new EmptyResult();

            if (((pageNumber * pageSize) + pageSize) > symbols.Count)
            {
                rangeLength = symbols.Count % pageSize;
                rangeStart = symbols.Count - rangeLength;
            }
            else
            {
                rangeStart = (pageNumber * pageSize);
                rangeLength = pageSize;
            }

            symbols = symbols.GetRange(rangeStart, rangeLength);

            ViewBag.Page = pageNumber + 1;
            return PartialView("_SymbolList", symbols);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            Log.Error("Error", filterContext.Exception);
        }
    }
}
