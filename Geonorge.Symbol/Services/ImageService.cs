using ImageMagick;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;

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
            image.Format = MagickFormat.Png;
            string targetFolder = System.Web.HttpContext.Current.Server.MapPath("~/files");
            string fileName = "test.png";
            string targetPath = Path.Combine(targetFolder, fileName);
            image.Write(targetPath);
        }

    }
}