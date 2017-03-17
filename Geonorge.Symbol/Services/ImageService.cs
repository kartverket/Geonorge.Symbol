using ImageMagick;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using ImageTracerNet;
using System.Globalization;
using System.Threading;

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
                        //case "svg":
                        //    {
                        //        image.Format = MagickFormat.Svg;
                        //        break;
                        //    }
                        default:
                            {
                                image.Format = MagickFormat.Png;
                                fileName = Path.GetFileNameWithoutExtension(file.FileName) + ".png";
                                break;
                            }
                    }

                    string targetFolder = System.Web.HttpContext.Current.Server.MapPath("~/files");
                    string targetPath = Path.Combine(targetFolder, fileName);
                    image.Write(targetPath);

                    if (format == "svg")
                    {
                        //To use . instead of , for decimals
                        var culture = new CultureInfo("en-US");
                        Thread.CurrentThread.CurrentCulture = culture;
                        Options options = new Options();
                        options.Tracing = new ImageTracerNet.OptionTypes.Tracing { LTres = 1f, QTres = 1f, PathOmit = 8 };
                        options.ColorQuantization = new ImageTracerNet.OptionTypes.ColorQuantization { ColorSampling = 0f };
                        options.Blur = new ImageTracerNet.OptionTypes.Blur { BlurDelta = 20f, BlurRadius = 1 };

                        string targetFolderSvg = System.Web.HttpContext.Current.Server.MapPath("~/files");
                        string fileNameSvg = Path.GetFileNameWithoutExtension(file.FileName) + ".svg";
                        string targetPathSvg = Path.Combine(targetFolderSvg, fileNameSvg);

                        File.WriteAllText(targetPathSvg, ImageTracer.ImageToSvg(targetPath, options));

                        fileName = fileNameSvg;
                    }

                }
            }

            return fileName;
        }

    }
}