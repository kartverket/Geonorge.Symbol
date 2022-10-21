using Geonorge.Symbol.Models;
using ImageMagick;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Geonorge.Symbol.Services
{
    public class BatchService
    {
        private readonly SymbolDbContext _dbContext;
        string _targetFolder;

        public BatchService(string targetFolder)
        {
            _targetFolder = targetFolder;
            _dbContext  = new SymbolDbContext();
        }

        internal void TiffCompressWithZip()
        {
            List<Models.Symbol> symbols = _dbContext.Symbols.ToList();
            foreach (var symbol in symbols)
            {
                var tiffs = symbol.SymbolFiles.Where(f => f.Format == "tiff").ToList();

                foreach(var tiff in tiffs)
                {
                    string targetFolder = _targetFolder;
                    if (!string.IsNullOrEmpty(symbol.SymbolPackages.FirstOrDefault()?.Folder))
                        targetFolder = targetFolder + "\\" + symbol.SymbolPackages.FirstOrDefault().Folder;

                    MagickReadSettings readerSettings = new MagickReadSettings();
                    readerSettings.Height = 1500;
                    readerSettings.Width = 1500;
                    readerSettings.BackgroundColor = MagickColors.Transparent;

                    string targetPath = Path.Combine(targetFolder, tiff.FileName);

                    if (File.Exists(targetPath)) { 

                        using (MagickImage image = new MagickImage(targetPath, readerSettings))
                        {                           
                            image.Format = MagickFormat.Tif;
                            //image.CompressionMethod = CompressionMethod.Zip;
                            image.Write(targetPath);
                        }
                    }
                }

              }
          }
     }
}