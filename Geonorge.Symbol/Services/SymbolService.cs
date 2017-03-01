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
               return _dbContext.Symbols.Where(s => s.Name.Contains(text)).ToList();
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
    }
}