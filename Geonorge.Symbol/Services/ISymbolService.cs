using System;
using System.Collections.Generic;
using Geonorge.Symbol.Models;
using System.Web;

namespace Geonorge.Symbol.Services
{
    public interface ISymbolService
    {
        List<Models.Symbol> GetSymbols(string text = null);
        void AddSymbol(Models.Symbol symbol);
        List<SymbolPackage> GetPackages();
        SymbolPackage GetPackage(Guid systemid);
        void AddPackage(SymbolPackage symbolPackage);
        void UpdatePackage(SymbolPackage symbolPackage);
        void RemovePackage(Guid systemid);
        Models.Symbol GetSymbol(Guid systemid);
        List<Models.SymbolFile> GetSymbolVariant(Guid systemid);
        void UpdateSymbol(Models.Symbol originalSymbol, Models.Symbol symbol);
        void RemoveSymbol(Models.Symbol symbol);
        SymbolFile GetSymbolFile(Guid systemid);
        void AddSymbolFile(Models.SymbolFile symbolFile, HttpPostedFileBase uploadFile);
        void AddSymbolFilesFromSvg(Models.SymbolFile symbolFile, HttpPostedFileBase uploadFile);
        void UpdateSymbolFile(Models.SymbolFile originalSymbolFile, Models.SymbolFile symbolFile);
        void RemoveSymbolFile(Models.SymbolFile symbolFile);
    }
}