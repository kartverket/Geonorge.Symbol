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
using System.Text.RegularExpressions;
using System.Diagnostics;

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
                        ////To use . instead of , for decimals
                        //var culture = new CultureInfo("en-US");
                        //Thread.CurrentThread.CurrentCulture = culture;
                        //Options options = new Options();
                        //options.Tracing = new ImageTracerNet.OptionTypes.Tracing { LTres = s.LTres, QTres = s.QTres, PathOmit = s.PathOmit };
                        //options.ColorQuantization = new ImageTracerNet.OptionTypes.ColorQuantization { ColorSampling = s.ColorSampling, ColorQuantCycles = s.ColorQuantCycles, MinColorRatio = s.MinColorRatio, NumberOfColors = s.NumberOfColors };
                        //options.Blur = new ImageTracerNet.OptionTypes.Blur { BlurDelta = s.BlurDelta, BlurRadius = s.BlurRadius };
                        //options.SvgRendering = new ImageTracerNet.OptionTypes.SvgRendering { LCpr = s.LCpr, QCpr = s.QCpr, RoundCoords = s.RoundCoords, Scale = s.Scale, SimplifyTolerance = s.SimplifyTolerance, Viewbox = s.Viewbox };

                        string targetFolderSvg = System.Web.HttpContext.Current.Server.MapPath("~/files");
                        string fileNameSvg = Path.GetFileNameWithoutExtension(file.FileName) + ".svg";
                        string targetPathSvg = Path.Combine(targetFolderSvg, fileNameSvg);

                        String command = @"/k java -jar "+ targetFolder + "\\ImageTracer.jar "+ targetPath + " outfilename "+ targetPathSvg + " ltres 1 qtres 4 pathomit 150 colorsampling 1 numberofcolors 8 mincolorratio 0.05 colorquantcycles 15 scale 1 simplifytolerance 1 roundcoords 3 lcpr 0 qcpr 0 desc 1 viewbox 0 blurradius 1 blurdelta 2000";
                        ProcessStartInfo cmdsi = new ProcessStartInfo("cmd.exe");
                        cmdsi.Arguments = command;
                        Process cmd = Process.Start(cmdsi);
                        cmd.WaitForExit();

                        //File.WriteAllText(targetPathSvg, ImageTracer.ImageToSvg(targetPath, options));

                        fileName = fileNameSvg;
                    }

                }
            }

            return fileName;
        }

        public string SaveFileAndCreateThumbnail(HttpPostedFileBase file, Models.Symbol symbol)
        {
            var ext = Path.GetExtension(file.FileName);

            string fileName = CreateFileName(symbol, ext);

            using (MemoryStream memStream = new MemoryStream())
            {
                file.InputStream.CopyTo(memStream);

                using (MagickImage image = new MagickImage(memStream))
                {
                    image.Resize(new MagickGeometry { IgnoreAspectRatio = false, Width = 50 });

                    switch (ext)
                    {
                        case ".png":
                            {
                                image.Format = MagickFormat.Png;
                                break;
                            }
                        case ".jpg":
                            {
                                image.Format = MagickFormat.Jpg;
                                break;
                            }
                        case ".gif":
                            {
                                image.Format = MagickFormat.Gif;
                                break;
                            }
                        case ".ai":
                            {
                                image.Format = MagickFormat.Ai;
                                break;
                            }
                        case ".svg":
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

                    string targetFolder = System.Web.HttpContext.Current.Server.MapPath("~/files/thumbnail");
                    string targetPath = Path.Combine(targetFolder, fileName);
                    image.Write(targetPath);

                    targetFolder = System.Web.HttpContext.Current.Server.MapPath("~/files");
                    targetPath = Path.Combine(targetFolder, fileName);
                    file.SaveAs(targetPath);
                }
            }

            return fileName;

        }

        public string CreateFileName(Models.Symbol symbol, string ext)
        {
            string filename;

            if (symbol.SymbolPackage != null)
                filename = MakeSeoFriendlyString(symbol.SymbolPackage.Name) + "_" + MakeSeoFriendlyString(symbol.Name);
            else
                filename = MakeSeoFriendlyString(symbol.Owner)  +"_" + MakeSeoFriendlyString(symbol.Name);

            string additionalNumber = "";

            string targetFolder = System.Web.HttpContext.Current.Server.MapPath("~/files");
            

            for (int i = 1; ; i++)
            {
                if(!File.Exists(Path.Combine(targetFolder, filename + additionalNumber + ext)))
                {
                    filename = filename + additionalNumber + ext;
                    break;
                }

                additionalNumber = i.ToString();
            }

            return filename;
        }

        public static string MakeSeoFriendlyString(string input)
        {
            string encodedUrl = (input ?? "").ToLower();
            // replace & with and
            encodedUrl = Regex.Replace(encodedUrl, @"\&+", "and");

            // remove characters
            encodedUrl = encodedUrl.Replace("'", "");

            // replace norwegian characters
            encodedUrl = encodedUrl.Replace("å", "a").Replace("æ", "ae").Replace("ø", "o");

            // remove invalid characters
            encodedUrl = Regex.Replace(encodedUrl, @"[^a-z0-9]", "");

            // trim leading & trailing characters
            encodedUrl = encodedUrl.Trim(' ');

            return encodedUrl;
        }

    }
}