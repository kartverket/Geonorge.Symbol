using System;
using System.Collections.Generic;
using Geonorge.Symbol.Models;
using System.Web;

namespace Geonorge.Symbol.Services
{
    public interface ISymbolService
    {
        List<Models.Symbol> GetSymbols(string text = null);
        Models.Symbol AddSymbol(Models.Symbol symbol);
        List<SymbolPackage> GetPackages();
        List<SymbolPackage> GetPackagesWithAccessControl(); 
        SymbolPackage GetPackage(Guid systemid);
        SymbolPackage AddPackage(SymbolPackage symbolPackage);
        void UpdatePackage(SymbolPackage originalSymbolPackage, SymbolPackage symbolPackage);
        void RemovePackage(Guid systemid);
        Models.Symbol GetSymbol(Guid systemid);
        List<Models.SymbolFile> GetSymbolVariant(Guid systemid);
        void RemoveSymbolFileVariant(SymbolFileVariant variant);
        void UpdateSymbol(Models.Symbol originalSymbol, Models.Symbol symbol);
        void RemoveSymbol(Models.Symbol symbol);
        SymbolFile GetSymbolFile(Guid systemid);
        void AddSymbolFiles(Models.SymbolFile symbolFile, HttpPostedFileBase[] uploadFile);
        void AddSymbolFilesFromSvg(Models.SymbolFile symbolFile, HttpPostedFileBase uploadFile);
        void UpdateSymbolFile(Models.SymbolFile symbolFile);
        void RemoveSymbolFile(Models.SymbolFile symbolFile);
        void RenameFile(SymbolFile symbolFile, string newFileName);
    }
}