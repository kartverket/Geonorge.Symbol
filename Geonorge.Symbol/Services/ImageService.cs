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
        public string ConvertImage(HttpPostedFileBase file, Models.Symbol symbol, string format, Models.SymbolFile symbolFile, int maxWidth = 0, bool useWidthInFilname = true)
        {
            string targetFolder = System.Web.HttpContext.Current.Server.MapPath("~/files");
            if (!string.IsNullOrEmpty(symbol.SymbolPackages.FirstOrDefault()?.Folder))
                targetFolder = targetFolder + "\\" + symbol.SymbolPackages.FirstOrDefault().Folder;

            var ext = "." + format;

            string fileName;

            MagickReadSettings readerSettings = new MagickReadSettings();
            readerSettings.Height = 1500;
            readerSettings.Width = 1500;
            readerSettings.BackgroundColor = MagickColors.Transparent;
            if (file.ContentType.Equals("image/svg+xml"))
            {
                readerSettings.Format = MagickFormat.Svg;
            }

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
                        case "tiff":
                            {
                                image.Format = MagickFormat.Tif;
                                break;
                            }
                        default:
                            {
                                image.Format = MagickFormat.Png;
                                break;
                            }
                    }

                    if(maxWidth > 0)
                        image.Resize(new MagickGeometry { IgnoreAspectRatio = false, Width = maxWidth, Height=0 });

                    fileName = CreateFileName(symbol, ext, targetFolder, symbolFile, image.Width.ToString(), useWidthInFilname);
                    string targetPath = Path.Combine(targetFolder, fileName);
                    image.Write(targetPath);

                }
            }

            return fileName;
        }

        public string ConvertToGif(string inputFileName, Models.Symbol symbol, string format, Models.SymbolFile symbolFile, int maxWidth = 0, bool useWidthInFilname = true)
        {
            string targetFolder = System.Web.HttpContext.Current.Server.MapPath("~/files");
            if (!string.IsNullOrEmpty(symbol.SymbolPackages.FirstOrDefault()?.Folder))
                targetFolder = targetFolder + "\\" + symbol.SymbolPackages.FirstOrDefault().Folder;

            var ext = "." + format;

            string fileName;

            MagickReadSettings readerSettings = new MagickReadSettings();
            readerSettings.Height = 1500;
            readerSettings.Width = 1500;
            readerSettings.BackgroundColor = MagickColors.Transparent;

            using (MemoryStream memStream = new MemoryStream())
            {
                using (MagickImage image = new MagickImage(targetFolder + "\\" + inputFileName, readerSettings))
                {     
                    image.Format = MagickFormat.Gif;
                    image.Settings.ColorType = ColorType.TrueColorAlpha;

                    if (maxWidth > 0)
                        image.Resize(new MagickGeometry { IgnoreAspectRatio = false, Width = maxWidth, Height = 0 });

                    fileName = CreateFileName(symbol, ext, targetFolder, symbolFile, image.Width.ToString(), useWidthInFilname);
                    string targetPath = Path.Combine(targetFolder, fileName);
                    image.Write(targetPath);
                }

            }

            return fileName;
        }

        public string SaveImage(HttpPostedFileBase file, Models.Symbol symbol, Models.SymbolFile symbolFile, int width = 0, bool useWidthInFilname = false)
        {

            string targetFolder = System.Web.HttpContext.Current.Server.MapPath("~/files");
            if (!string.IsNullOrEmpty(symbol.SymbolPackages.FirstOrDefault()?.Folder))
                targetFolder = targetFolder + "\\" + symbol.SymbolPackages.FirstOrDefault().Folder;

            var ext = Path.GetExtension(file.FileName);

            string fileName = CreateFileName(symbol, ext, targetFolder, symbolFile, width.ToString(), useWidthInFilname);

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

        public string CreateFileName(Models.Symbol symbol, string ext, string targetFolder = null, Models.SymbolFile symbolFile = null, string width = null, bool useWidthInFilname = true)
        {
            if(string.IsNullOrEmpty(targetFolder))
                targetFolder = System.Web.HttpContext.Current.Server.MapPath("~/files");
 
            string filename;

            filename = MakeSeoFriendlyString(symbol.Name);
            if (symbolFile != null)
            {
                if (!string.IsNullOrEmpty(symbolFile.Type))
                    filename = filename + "_" + MakeSeoFriendlyString(symbolFile.Type);

                if (!string.IsNullOrEmpty(symbolFile.Color))
                    filename = filename + "_" + MakeSeoFriendlyString(symbolFile.Color);

                if (useWidthInFilname && !string.IsNullOrEmpty(width))
                    filename = filename + "_" + MakeSeoFriendlyString(width);

            }

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

        public int GetWidth(HttpPostedFileBase uploadFile)
        {
            int width = 0;
            using (MemoryStream memStream = new MemoryStream())
            {
                uploadFile.InputStream.CopyTo(memStream);

                using (MagickImage image = new MagickImage(memStream))
                {
                    width = image.Width;
                }
            }

            return width;
        }

        public static string MakeSeoFriendlyString(string input)
        {
            string encodedUrl = (input ?? "").ToLower();

            // trim leading & trailing characters
            encodedUrl = encodedUrl.Trim(' ');

            // replace space with minus
            encodedUrl = Regex.Replace(encodedUrl, " ", "-");

            // replace & with and
            encodedUrl = Regex.Replace(encodedUrl, @"\&+", "and");

            // remove characters
            encodedUrl = encodedUrl.Replace("'", "");

            // replace norwegian characters
            encodedUrl = encodedUrl.Replace("å", "aa").Replace("æ", "ae").Replace("ø", "oe");

            // remove invalid characters
            encodedUrl = Regex.Replace(encodedUrl, @"[^a-z0-9_-]", "");


            return encodedUrl;
        }

    }
}