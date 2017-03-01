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

namespace Geonorge.Symbol.Controllers
{
    public class FilesController : Controller
    {
        ISymbolService _symbolService;
        private readonly IAuthorizationService _authorizationService;

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

            int pageSize = 50;
            int pageNumber = (page ?? 1);

            return View(symbols.ToPagedList(pageNumber, pageSize));
        }

        // GET: Files/Details/5
        public ActionResult Details(Guid? id)
        {

            return View();
        }

        // GET: Files/Create
        public ActionResult Create()
        {
            ViewBag.Types = new SelectList(CodeList.SymbolTypes, "Key", "Value");
            ViewBag.Themes = new SelectList(CodeList.Themes(), "Key", "Value", "Annen");
            ViewBag.SymbolPackages = new SelectList(_symbolService.GetPackages(), "SystemId", "Name");
            ViewBag.IsAdmin = false;
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
        public ActionResult Create(Models.Symbol symbol)
        {

            ViewBag.Types = new SelectList(CodeList.SymbolTypes, "Key", "Value", symbol.Type);
            ViewBag.Themes = new SelectList(CodeList.Themes(), "Key", "Value", symbol.Theme);
            ViewBag.SymbolPackages = new SelectList(_symbolService.GetPackages(), "SystemId", "Name", symbol.SymbolPackage);

            ViewBag.IsAdmin = false;
            if (Request.IsAuthenticated)
            {
                ViewBag.IsAdmin = _authorizationService.IsAdmin();
            }

            if (ModelState.IsValid)
            {
                _symbolService.AddSymbol(symbol);
                return RedirectToAction("Index", "Files");
            }

            return View(symbol);
        }

        // GET: Files/Edit/5
        public ActionResult Edit(Guid? systemid)
        {
            return View();
        }

        // POST: Files/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Models.Symbol symbol)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        // GET: Files/Delete/5
        public ActionResult Delete(Guid? systemid)
        {
            if (systemid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            return View();
        }

        // POST: Files/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(Guid systemid)
        {

            return RedirectToAction("Index");
        }
    }
}
