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
        public string ConvertImage(HttpPostedFileBase file, string format)
        {

            string fileName = Path.GetFileNameWithoutExtension(file.FileName) + "." + format;

            using (MemoryStream memStream = new MemoryStream())
            {
                file.InputStream.CopyTo(memStream);

                using (MagickImage image = new MagickImage(memStream))
                {
                    switch (format)
                    {
                        case "png":
                            {
                                image.Format = MagickFormat.Png;
                                break;
                            }
                        case "jpg":
                            {
                                image.Format = MagickFormat.Jpg;
                                break;
                            }
                        case "gif":
                            {
                                image.Format = MagickFormat.Gif;
                                break;
                            }
                        case "ai":
                            {
                                image.Format = MagickFormat.Ai;
                                break;
                            }
                        case "svg":
                            {
                                image.Format = MagickFormat.Svg;
                                break;
                            }
                        default:
                            {
                                image.Format = MagickFormat.Png;
                                break;
                            }
                    }

                    string targetFolder = System.Web.HttpContext.Current.Server.MapPath("~/files");
                    string targetPath = Path.Combine(targetFolder, fileName);
                    image.Write(targetPath);
                }
            }

            return fileName;
        }

    }
}