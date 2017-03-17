using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Geonorge.Symbol.Services;
using Geonorge.Symbol.Models;

namespace Geonorge.Symbol.Controllers
{
    public class ImageController : Controller
    {
        // GET: Image
        public ActionResult Index()
        {
            ImageSettings model = new ImageSettings();
            ViewBag.FileUrl = "";
            return View(model);
        }

        public ActionResult Create(HttpPostedFileBase uploadFile, ImageSettings settings, string format)
        {
            SymbolFile file = new SymbolFile();
            file.FileName = new ImageService().ConvertImage(uploadFile, settings, format);
            ViewBag.FileUrl = file.FileUrl();
   
            return View("Index", settings);
        }
    }
}