using System.Collections.Generic;
using Geonorge.Symbol.Models;

namespace Geonorge.Symbol.Services
{
    public interface ISymbolService
    {
        List<Models.Symbol> GetSymbols(string text = null);
        void AddSymbol(Models.Symbol symbol);
    }
}