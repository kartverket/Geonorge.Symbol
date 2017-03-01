using ImageMagick;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Geonorge.Symbol.Services
{
    public class ImageService
    {
        public void ConvertImage(HttpPostedFileBase file)
        {
            Stream data = new MemoryStream();
            file.InputStream.CopyTo(data);
            MagickImage image = new MagickImage(data);
            //image.Format = MagickFormat.Svg;
            image.Format = MagickFormat.Pdf;
            string targetFolder = System.Web.HttpContext.Current.Server.MapPath("~/files");
            string fileName = "test.pdf";
            string targetPath = Path.Combine(targetFolder, fileName);
            image.Write(targetPath);
        }

    }
}