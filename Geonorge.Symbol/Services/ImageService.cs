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
using Geonorge.Symbol.Models;

namespace Geonorge.Symbol.Services
{
    public class ImageService
    {
        public string ConvertImage(HttpPostedFileBase file, ImageSettings s, string format)
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
                        options.Tracing = new ImageTracerNet.OptionTypes.Tracing { LTres = s.LTres, QTres = s.QTres, PathOmit = s.PathOmit };
                        options.ColorQuantization = new ImageTracerNet.OptionTypes.ColorQuantization { ColorSampling = s.ColorSampling, ColorQuantCycles = s.ColorQuantCycles, MinColorRatio = s.MinColorRatio, NumberOfColors = s.NumberOfColors };
                        options.Blur = new ImageTracerNet.OptionTypes.Blur { BlurDelta = s.BlurDelta, BlurRadius = s.BlurRadius };
                        options.SvgRendering = new ImageTracerNet.OptionTypes.SvgRendering { LCpr = s.LCpr, QCpr = s.QCpr, RoundCoords = s.RoundCoords, Scale = s.Scale, SimplifyTolerance = s.SimplifyTolerance, Viewbox = s.Viewbox };

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