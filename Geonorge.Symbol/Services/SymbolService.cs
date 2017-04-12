﻿using System;
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
               || s.Owner.Contains(text) || s.Theme.Contains(text) || s.Type.Contains(text)
               || s.SymbolPackage.Name.Contains(text) || s.SymbolPackage.Theme.Contains(text)
               ).ToList();
            }
            else { 
            return _dbContext.Symbols.ToList();
            }
        }

        public void AddSymbol(Models.Symbol symbol)
        {
            string owner = _authorizationService.GetSecurityClaim("organization").FirstOrDefault();
            if (_authorizationService.IsAdmin() && !string.IsNullOrEmpty(symbol.Owner))
                owner = symbol.Owner;

            symbol.SystemId = Guid.NewGuid();
            symbol.Owner = owner;
            symbol.LastEditedBy = _authorizationService.GetSecurityClaim("username").FirstOrDefault();

            _dbContext.Symbols.Add(symbol);
            _dbContext.SaveChanges();
        }

        public List<SymbolPackage> GetPackages()
        {
            return _dbContext.SymbolPackages.ToList();
        }

        public SymbolPackage GetPackage(Guid systemid)
        {
            return _dbContext.SymbolPackages.Where(p => p.SystemId == systemid).FirstOrDefault();
        }

        public void AddPackage(SymbolPackage symbolPackage)
        {
            symbolPackage.SystemId = Guid.NewGuid();
            _dbContext.SymbolPackages.Add(symbolPackage);
            _dbContext.SaveChanges();
        }

        public void UpdatePackage(SymbolPackage symbolPackage)
        {
            _dbContext.Entry(symbolPackage).State = EntityState.Modified;
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
                originalSymbol.EksternalSymbolID = symbol.EksternalSymbolID;
                originalSymbol.OfficialStatus = symbol.OfficialStatus;

                string owner = _authorizationService.GetSecurityClaim("organization").FirstOrDefault();
                if (_authorizationService.IsAdmin() && !string.IsNullOrEmpty(symbol.Owner))
                    owner = symbol.Owner;

                originalSymbol.Owner = owner;
                originalSymbol.Source = symbol.Source;
                originalSymbol.SourceUrl = symbol.SourceUrl;
                var symbolPackage = originalSymbol.SymbolPackage;
                originalSymbol.SymbolPackage = symbol.SymbolPackage;
                originalSymbol.Theme = symbol.Theme;
                originalSymbol.Type = symbol.Type;
                if(symbol.Thumbnail != null)
                    originalSymbol.Thumbnail = symbol.Thumbnail;

            originalSymbol.LastEditedBy = _authorizationService.GetSecurityClaim("username").FirstOrDefault();
            _dbContext.Entry(originalSymbol).State = EntityState.Modified;
            _dbContext.SaveChanges();

            }
        }

        public void RemoveSymbol(Models.Symbol symbol)
        {
            _dbContext.Symbols.Remove(symbol);
            _dbContext.SaveChanges();
            foreach (var file in symbol.SymbolFiles)
            {
                DeleteFile(file.FileName);
            }
            DeleteThumbnailFile(symbol.Thumbnail);
        }

        public SymbolFile GetSymbolFile(Guid systemid)
        {
            var symbolFile = _dbContext.SymbolFiles.Where(s => s.SystemId == systemid).FirstOrDefault();

            return symbolFile;
        }

        public void AddSymbolFile(SymbolFile symbolFile, HttpPostedFileBase uploadFile)
        {
            var symbol = GetSymbol(symbolFile.Symbol.SystemId);
            SymbolFileVariant variant = new SymbolFileVariant();
            variant.SystemId = Guid.NewGuid();
            variant.Name = symbol.Name + "_" + symbolFile.Type;
            symbolFile.SymbolFileVariant = variant;
            symbol.DateChanged = DateTime.Now;
            symbol.LastEditedBy = _authorizationService.GetSecurityClaim("username").FirstOrDefault();
            symbolFile.Symbol = symbol;
            symbolFile.SystemId = Guid.NewGuid();
            var filename = new ImageService().SaveImage(uploadFile, symbol);
            symbolFile.FileName = filename;
            symbolFile.Format = Path.GetExtension(filename).Replace(".", "");
            _dbContext.SymbolFiles.Add(symbolFile);
            _dbContext.SaveChanges();
        }

        public void UpdateSymbolFile(SymbolFile originalSymbolFile, Models.SymbolFile symbolFile)
        {
            //Todo set changes
            _dbContext.Entry(originalSymbolFile).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }

        public void RemoveSymbolFile(SymbolFile symbolFile)
        {
            _dbContext.SymbolFiles.Remove(symbolFile);
            _dbContext.SaveChanges();
            DeleteFile(symbolFile.FileName);
        }

        public void AddSymbolFilesFromSvg(SymbolFile symbolFile, HttpPostedFileBase uploadFile)
        {
            var symbol = GetSymbol(symbolFile.Symbol.SystemId);
            SymbolFileVariant variant = new SymbolFileVariant();
            variant.SystemId = Guid.NewGuid();
            variant.Name = symbol.Name + "_" + symbolFile.Type;

            ImageService imageService = new ImageService();

            symbolFile.SymbolFileVariant = variant;
            symbol.DateChanged = DateTime.Now;
            symbol.LastEditedBy = _authorizationService.GetSecurityClaim("username").FirstOrDefault();
            symbolFile.Symbol = symbol;
            symbolFile.SystemId = Guid.NewGuid();
            var filename = imageService.SaveImage(uploadFile, symbol);
            symbolFile.FileName = filename;
            symbolFile.Format = Path.GetExtension(filename).Replace(".", "");
            _dbContext.SymbolFiles.Add(symbolFile);
            _dbContext.SaveChanges();

            filename = imageService.ConvertImage(uploadFile, symbol, "png");
            SymbolFile png = new SymbolFile();
            png.SystemId = Guid.NewGuid();
            png.Color = symbolFile.Color;
            png.Size = symbolFile.Size;
            png.Symbol = symbol;
            png.SymbolFileVariant = variant;
            png.Type = symbolFile.Type;
            png.FileName = filename;
            symbolFile.Format = Path.GetExtension(filename).Replace(".", "");
            _dbContext.SymbolFiles.Add(png);
            _dbContext.SaveChanges();

            uploadFile.InputStream.Position = 0;
            filename = imageService.ConvertImage(uploadFile, symbol, "gif");
            SymbolFile gif = new SymbolFile();
            gif.SystemId = Guid.NewGuid();
            gif.Color = symbolFile.Color;
            gif.Size = symbolFile.Size;
            gif.Symbol = symbol;
            gif.SymbolFileVariant = variant;
            gif.Type = symbolFile.Type;
            gif.FileName = filename;
            symbolFile.Format = Path.GetExtension(filename).Replace(".", "");
            _dbContext.SymbolFiles.Add(gif);
            _dbContext.SaveChanges();

        }

        public List<Models.SymbolFile> GetSymbolVariant(Guid systemid)
        {
            var symbolVariant = _dbContext.SymbolFiles.Where(v => v.SymbolFileVariant.SystemId == systemid).ToList();

            return symbolVariant;
        }

        private void DeleteFile(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                string targetFolder = System.Web.HttpContext.Current.Server.MapPath("~/files");
                string targetPath = Path.Combine(targetFolder, fileName);
                if (File.Exists(targetPath))
                    File.Delete(targetPath);
            }
        }

        private void DeleteThumbnailFile(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                string targetFolder = System.Web.HttpContext.Current.Server.MapPath("~/files/thumbnail");
                string targetPath = Path.Combine(targetFolder, fileName);
                if (File.Exists(targetPath))
                    File.Delete(targetPath);
            }
        }
    }
}