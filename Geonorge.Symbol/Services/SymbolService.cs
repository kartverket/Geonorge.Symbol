using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Geonorge.Symbol.Models;
using System.Data.Entity;
using System.Text.RegularExpressions;
using System.IO;

namespace Geonorge.Symbol.Services
{
    public class SymbolService : ISymbolService
    {
        private readonly SymbolDbContext _dbContext;
        private readonly IAuthorizationService _authorizationService;

        public SymbolService(SymbolDbContext dbContext, IAuthorizationService authorizationService)
        {
            _dbContext = dbContext;
            _authorizationService = authorizationService;
        }

        public List<Models.Symbol> GetSymbols(string text = null)
        {
            if (!string.IsNullOrEmpty(text))
            {
               return _dbContext.Symbols.Where(s => s.Name.Contains(text) || s.Description.Contains(text)
               || s.Owner.Contains(text) || s.Theme.Contains(text)  
               ).ToList();
            }
            else { 
            return _dbContext.Symbols.ToList();
            }
        }

        public Models.Symbol AddSymbol(Models.Symbol symbol)
        {
            string owner = _authorizationService.GetSecurityClaim("organization").FirstOrDefault();
            if (_authorizationService.IsAdmin() && !string.IsNullOrEmpty(symbol.Owner))
                owner = symbol.Owner;

            symbol.SystemId = Guid.NewGuid();
            symbol.Owner = owner;
            symbol.LastEditedBy = _authorizationService.GetSecurityClaim("username").FirstOrDefault();

            _dbContext.Symbols.Add(symbol);
            _dbContext.SaveChanges();

            return symbol;
        }

        public List<SymbolPackage> GetPackages()
        {
            return _dbContext.SymbolPackages.OrderBy(o => o.Name).ToList();
        }

        public List<SymbolPackage> GetPackagesWithAccessControl()
        {
            if (_authorizationService.IsAdmin())
                return _dbContext.SymbolPackages.OrderBy(o => o.Name).ToList();
            else
            {
                string owner = _authorizationService.GetSecurityClaim("organization").FirstOrDefault();
                return _dbContext.SymbolPackages.Where(o => o.Owner == owner).OrderBy(o => o.Name).ToList();
            }
        }

        public SymbolPackage GetPackage(Guid systemid)
        {
            return _dbContext.SymbolPackages.Where(p => p.SystemId == systemid).FirstOrDefault();
        }

        public SymbolPackage AddPackage(SymbolPackage symbolPackage)
        {
            symbolPackage.SystemId = Guid.NewGuid();

            string owner = _authorizationService.GetSecurityClaim("organization").FirstOrDefault();
            if (_authorizationService.IsAdmin() && !string.IsNullOrEmpty(symbolPackage.Owner))
                owner = symbolPackage.Owner;
            symbolPackage.Owner = owner;

            symbolPackage.Folder = CreatePackageFolder(symbolPackage.Name);

            _dbContext.SymbolPackages.Add(symbolPackage);
            _dbContext.SaveChanges();

            return symbolPackage;
        }

        public void UpdatePackage(SymbolPackage originalSymbolPackage, SymbolPackage symbolPackage)
        {
            if (originalSymbolPackage.Name != symbolPackage.Name)
                originalSymbolPackage.Folder = RenamePackageFolder(originalSymbolPackage.Name, symbolPackage.Name);

            originalSymbolPackage.Name = symbolPackage.Name;
            originalSymbolPackage.OfficialStatus = symbolPackage.OfficialStatus;
            originalSymbolPackage.Owner = symbolPackage.Owner;
            originalSymbolPackage.Theme = symbolPackage.Theme;


            _dbContext.Entry(originalSymbolPackage).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }

        public void RemovePackage(Guid systemid)
        {
            SymbolPackage symbolPackage = GetPackage(systemid);
            _dbContext.SymbolPackages.Remove(symbolPackage);
            _dbContext.SaveChanges();
        }

        public Models.Symbol GetSymbol(Guid systemid)
        {
            var symbol = _dbContext.Symbols.Where(s => s.SystemId == systemid).Include(f => f.SymbolFiles).FirstOrDefault();

            return symbol;
        }

        public void UpdateSymbol(Models.Symbol originalSymbol, Models.Symbol symbol)
        {
            if (symbol != null)
            {
                originalSymbol.Name = symbol.Name;
                originalSymbol.Description = symbol.Description;
                originalSymbol.SymbolId = symbol.SymbolId;

                string owner = _authorizationService.GetSecurityClaim("organization").FirstOrDefault();
                if (_authorizationService.IsAdmin() && !string.IsNullOrEmpty(symbol.Owner))
                    owner = symbol.Owner;

                originalSymbol.Owner = owner;
                originalSymbol.Source = symbol.Source;
                originalSymbol.SourceUrl = symbol.SourceUrl;
                var symbolPackages = originalSymbol.SymbolPackages;
                originalSymbol.SymbolPackages = symbol.SymbolPackages;
                originalSymbol.Theme = symbol.Theme;
                if(symbol.Thumbnail != null)
                    originalSymbol.Thumbnail = symbol.Thumbnail;

            originalSymbol.LastEditedBy = _authorizationService.GetSecurityClaim("username").FirstOrDefault();
            _dbContext.Entry(originalSymbol).State = EntityState.Modified;
            _dbContext.SaveChanges();

            }
        }

        public void RemoveSymbol(Models.Symbol symbol)
        {
            var symbolFiles = symbol.SymbolFiles.ToList();
            foreach (var file in symbolFiles)
            {

                if (file.SymbolFileVariant != null)
                {
                    _dbContext.SymbolFileVariants.Remove(file.SymbolFileVariant);
                    _dbContext.SymbolFiles.Remove(file);
                }
                else { _dbContext.SymbolFiles.Remove(file); }
                _dbContext.SaveChanges();
                DeleteFile(file.FileName, symbol.SymbolPackages.FirstOrDefault()?.Folder);
            }

            _dbContext.Symbols.Remove(symbol);
            _dbContext.SaveChanges();
            DeleteThumbnailFile(symbol.Thumbnail, symbol.SymbolPackages.FirstOrDefault()?.Folder);
        }

        public SymbolFile GetSymbolFile(Guid systemid)
        {
            var symbolFile = _dbContext.SymbolFiles.Where(s => s.SystemId == systemid).FirstOrDefault();

            return symbolFile;
        }

        public void AddSymbolFiles(SymbolFile symbolFile, HttpPostedFileBase[] uploadFiles)
        {
            var symbol = GetSymbol(symbolFile.Symbol.SystemId);

            if(symbolFile.SymbolFileVariant == null)
            { 
                SymbolFileVariant variant = new SymbolFileVariant();
                variant.SystemId = Guid.NewGuid();
                variant.Name = GetVariantName(symbol, symbolFile);
                symbolFile.SymbolFileVariant = variant;
            }

            symbol.DateChanged = DateTime.Now;
            symbol.LastEditedBy = _authorizationService.GetSecurityClaim("username").FirstOrDefault();

            ImageService imageService = new ImageService();

            foreach (var uploadFile in uploadFiles)
            {
                int width = imageService.GetWidth(uploadFile);
                uploadFile.InputStream.Position = 0;
                var format = Path.GetExtension(uploadFile.FileName).Replace(".", "");
                symbolFile.Size = GetSize(width);
                var filename = new ImageService().SaveImage(uploadFile, symbol, symbolFile, width, true);
                AddFile(symbolFile, symbol, symbolFile.SymbolFileVariant, filename, format);
            }
        }

        private string GetSize(int width)
        {
            string size = "liten";

            if (width >= 250 && width < 1000)
                size = "middels";
            else if (width >= 1000)
                size = "stor";

            return size;
        }

        public void UpdateSymbolFile(Models.SymbolFile symbolFile)
        {
            _dbContext.Entry(symbolFile).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }

        public void RemoveSymbolFile(SymbolFile symbolFile)
        {
            DeleteFile(symbolFile.FileName, symbolFile.Symbol.SymbolPackages.FirstOrDefault()?.Folder);
            _dbContext.SymbolFiles.Remove(symbolFile);
            _dbContext.SaveChanges();
        }

        public void AddSymbolFilesFromSvg(SymbolFile symbolFile, HttpPostedFileBase uploadFile)
        {
            var symbol = GetSymbol(symbolFile.Symbol.SystemId);
            if (symbolFile.SymbolFileVariant == null)
            {
                SymbolFileVariant variant = new SymbolFileVariant();
                variant.SystemId = Guid.NewGuid();
                variant.Name = GetVariantName(symbol, symbolFile);
                symbolFile.SymbolFileVariant = variant;
            }

            ImageService imageService = new ImageService();

            symbol.DateChanged = DateTime.Now;
            symbol.LastEditedBy = _authorizationService.GetSecurityClaim("username").FirstOrDefault();
            symbolFile.Symbol = symbol;
            symbolFile.SystemId = Guid.NewGuid();
            var filename = imageService.SaveImage(uploadFile, symbol, symbolFile, 0, false);
            symbolFile.FileName = filename;
            symbolFile.Format = "svg";
            symbolFile.Size = "stor";
            _dbContext.SymbolFiles.Add(symbolFile);
            _dbContext.SaveChanges();

            uploadFile.InputStream.Position = 0;
            filename = imageService.ConvertImage(uploadFile, symbol, "tiff", symbolFile, 1500, false);
            symbolFile.Size = "stor";
            AddFile(symbolFile, symbol, symbolFile.SymbolFileVariant, filename, "tiff");

            uploadFile.InputStream.Position = 0;
            filename = imageService.ConvertImage(uploadFile, symbol, "png", symbolFile, 50, false);
            symbolFile.Size = "liten";
            AddFile(symbolFile, symbol, symbolFile.SymbolFileVariant, filename, "png");

            uploadFile.InputStream.Position = 0;
            filename = imageService.ConvertImage(uploadFile, symbol, "png", symbolFile, 150);
            symbolFile.Size = "liten";
            AddFile(symbolFile, symbol, symbolFile.SymbolFileVariant, filename, "png");

            //var gifInputFileName = filename;

            //filename = imageService.ConvertToGif(gifInputFileName, symbol, "gif", symbolFile, 50, false);
            //symbolFile.Size = "liten";
            //AddFile(symbolFile, symbol, symbolFile.SymbolFileVariant, filename, "gif");

            //filename = imageService.ConvertToGif(gifInputFileName, symbol, "gif", symbolFile, 150);
            //symbolFile.Size = "liten";
            //AddFile(symbolFile, symbol, symbolFile.SymbolFileVariant, filename, "gif");

            //uploadFile.InputStream.Position = 0;
            //filename = imageService.ConvertImage(uploadFile, symbol, "ai", symbolFile, 0, false);
            //symbolFile.Size = "stor";
            //AddFile(symbolFile, symbol, symbolFile.SymbolFileVariant, filename, "ai");

        }

        private void AddFile(SymbolFile symbolFile, Models.Symbol symbol, SymbolFileVariant variant, string filename, string format)
        {
            SymbolFile file = new SymbolFile();
            file.SystemId = Guid.NewGuid();
            file.Color = symbolFile.Color;
            file.Size = symbolFile.Size;
            file.Symbol = symbol;
            file.SymbolFileVariant = variant;
            file.Type = symbolFile.Type;
            file.FileName = filename;
            file.Format = format;
            _dbContext.SymbolFiles.Add(file);
            _dbContext.SaveChanges();
        }

        public List<Models.SymbolFile> GetSymbolVariant(Guid systemid)
        {
            var symbolVariant = _dbContext.SymbolFiles.Where(v => v.SymbolFileVariant.SystemId == systemid).ToList();

            return symbolVariant;
        }

        public void RemoveSymbolFileVariant(SymbolFileVariant variant)
        {
            _dbContext.SymbolFileVariants.Remove(variant);
            _dbContext.SaveChanges();
        }

        private void DeleteFile(string fileName, string packageFolder)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                string targetFolder = System.Web.HttpContext.Current.Server.MapPath("~/files");
                if (!string.IsNullOrEmpty(packageFolder))
                    targetFolder = targetFolder + "\\" + packageFolder;
                string targetPath = Path.Combine(targetFolder, fileName);
                if (File.Exists(targetPath))
                    File.Delete(targetPath);
            }
        }

        private void DeleteThumbnailFile(string fileName, string packageFolder)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                string targetFolder = System.Web.HttpContext.Current.Server.MapPath("~/files/thumbnail");
                if (!string.IsNullOrEmpty(packageFolder))
                    targetFolder = targetFolder + "/" + packageFolder;
                string targetPath = Path.Combine(targetFolder, fileName);
                if (File.Exists(targetPath))
                    File.Delete(targetPath);
            }
        }

        private string CreatePackageFolder(string packageName)
        {

            packageName = CreateValidFileString(packageName);
            string targetFolder = System.Web.HttpContext.Current.Server.MapPath("~/files/" + packageName);
            if (Directory.Exists(targetFolder))
                throw new PackageException("Pakkenavnet er opptatt, velg et annet pakkenavn");

            Directory.CreateDirectory(targetFolder);

            return packageName;
        }

        private string RenamePackageFolder(string packageName, string newPackageName)
        {

            newPackageName = CreateValidFileString(newPackageName);
            string sourceFolder = System.Web.HttpContext.Current.Server.MapPath("~/files/" + packageName);
            string destinationFolder = System.Web.HttpContext.Current.Server.MapPath("~/files/" + newPackageName);

            if(Directory.Exists(destinationFolder))
                throw new PackageException("Pakkenavnet er opptatt, velg et annet pakkenavn");

            if (Directory.Exists(sourceFolder))
            {
                if (Directory.Exists(destinationFolder))
                    Directory.CreateDirectory(destinationFolder);

                Directory.Move(sourceFolder, destinationFolder);
            }
                

            return newPackageName;
        }

        public static string CreateValidFileString(string input)
        {
            string encodedUrl = (input ?? "").ToLower();

            // trim leading & trailing characters
            encodedUrl = encodedUrl.Trim(' ');

            // replace space with underscore
            encodedUrl = Regex.Replace(encodedUrl, " ", "_");

            // replace & with and
            encodedUrl = Regex.Replace(encodedUrl, @"\&+", "and");

            // remove characters
            encodedUrl = encodedUrl.Replace("'", "");

            // replace norwegian characters
            encodedUrl = encodedUrl.Replace("å", "a").Replace("æ", "ae").Replace("ø", "o");

            // remove invalid characters
            encodedUrl = Regex.Replace(encodedUrl, @"[^a-z0-9_]", "");

            return encodedUrl;
        }

        private string GetVariantName(Models.Symbol symbol, SymbolFile symbolFile)
        {
            string variantName = symbol.Name;

            if (!string.IsNullOrEmpty(symbolFile.Type))
                variantName = variantName + " " + symbolFile.Type;

            if (!string.IsNullOrEmpty(symbolFile.Color))
                variantName = variantName + " " + symbolFile.Color;

            return variantName;
        }

        public void RenameFile(SymbolFile symbolFile, string newFileName)
        {
            var fileExists = _dbContext.SymbolFiles.Where(f => f.FileName == newFileName && f.SystemId != symbolFile.SystemId);
            if(fileExists.Any())
                throw new FileException("Filnavn finnes fra før");

            string targetFolder = System.Web.HttpContext.Current.Server.MapPath("~/files");
            if (!string.IsNullOrEmpty(symbolFile.Symbol.SymbolPackages[0].Folder))
                targetFolder = targetFolder + "\\" + symbolFile.Symbol.SymbolPackages[0].Folder;
            string targetPath = Path.Combine(targetFolder, symbolFile.FileName);
 
            string destinationPath = Path.Combine(targetFolder, newFileName);

            File.Move(targetPath, destinationPath);
        }
    }
}