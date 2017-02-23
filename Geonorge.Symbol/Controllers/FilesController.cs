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
        public ActionResult Index()
        {
            return View(_symbolService.GetSymbols());
        }

        // GET: Files/Details/5
        public ActionResult Details(Guid? id)
        {

            return View();
        }

        // GET: Files/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Files/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Models.Symbol symbol)
        {
            if (ModelState.IsValid)
            {
                _symbolService.AddSymbol(symbol);
                return RedirectToAction("Index", "Files");
            }

            return View(symbol);
        }

        // GET: Files/Edit/5
        public ActionResult Edit(Guid? id)
        {
            return View();
        }

        // POST: Files/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
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
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            return View();
        }

        // POST: Files/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {

            return RedirectToAction("Index");
        }
    }
}
