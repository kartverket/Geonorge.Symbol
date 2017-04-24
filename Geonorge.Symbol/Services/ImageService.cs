using ImageMagick;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Globalization;
using System.Threading;
using Geonorge.Symbol.Models;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Geonorge.Symbol.Services
{
    public class ImageService
    {
        public string ConvertImage(HttpPostedFileBase file, Models.Symbol symbol, string format)
        {
            string targetFolder = System.Web.HttpContext.Current.Server.MapPath("~/files");

            var ext = "." + format;

            MagickReadSettings readerSettings = new MagickReadSettings();
            readerSettings.BackgroundColor = MagickColors.Transparent;
            if (file.ContentType.Equals("image/svg+xml"))
            {
                readerSettings.Format = MagickFormat.Svg;
            }

            string fileName = CreateFileName(symbol, ext, targetFolder);

            using (MemoryStream memStream = new MemoryStream())
            {
                file.InputStream.CopyTo(memStream);

                using (MagickImage image = new MagickImage(memStream, readerSettings))
                {
                    switch (format)
                    {
                        case "png":
                            {
                                image.Format = MagickFormat.Png32;
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
                                image.Settings.ColorType = ColorType.TrueColorAlpha;
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
                        case "tif":
                            {
                                image.Format = MagickFormat.Tif;
                                break;
                            }
                        default:
                            {
                                image.Format = MagickFormat.Png;
                                fileName = Path.GetFileNameWithoutExtension(file.FileName) + ".png";
                                break;
                            }
                    }

                    string targetPath = Path.Combine(targetFolder, fileName);
                    image.Write(targetPath);

                }
            }

            return fileName;
        }

        public string SaveImage(HttpPostedFileBase file, Models.Symbol symbol)
        {

            string targetFolder = System.Web.HttpContext.Current.Server.MapPath("~/files");

            var ext = Path.GetExtension(file.FileName);

            string fileName = CreateFileName(symbol, ext, targetFolder);

            string targetPath = Path.Combine(targetFolder, fileName);
            file.SaveAs(targetPath);

            return fileName;

        }

        public string SaveThumbnail(HttpPostedFileBase file, Models.Symbol symbol)
        {

            string targetFolder = System.Web.HttpContext.Current.Server.MapPath("~/files/thumbnail");

            var ext = Path.GetExtension(file.FileName);

            string fileName = CreateFileName(symbol, ext, targetFolder);

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

                    string targetPath = Path.Combine(targetFolder, fileName);
                    image.Write(targetPath);

                }
            }

            return fileName;

        }

        public string CreateFileName(Models.Symbol symbol, string ext, string targetFolder = null)
        {
            if(string.IsNullOrEmpty(targetFolder))
                targetFolder = System.Web.HttpContext.Current.Server.MapPath("~/files");

            string filename;

            if (symbol.SymbolPackages != null)
                filename = MakeSeoFriendlyString(symbol.SymbolPackages[0].Name) + "_" + MakeSeoFriendlyString(symbol.Name);
            else
                filename = MakeSeoFriendlyString(symbol.Owner)  +"_" + MakeSeoFriendlyString(symbol.Name);

            string additionalNumber = "";
            

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