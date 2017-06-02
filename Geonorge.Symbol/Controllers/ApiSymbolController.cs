using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Mvc;
using Geonorge.Symbol.Models;
using Geonorge.Symbol.Services;

namespace Geonorge.Symbol.Api
{
    [HandleError]
    public class ApiSymbolController : ApiController
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        ISymbolService _symbolService;

        public ApiSymbolController(ISymbolService symbolService)
        {
            _symbolService = symbolService;
        }

        /// <summary>
        /// List symbols, optional limit by text
        /// </summary>
        [System.Web.Http.Route("api/symbols")]
        [System.Web.Http.HttpGet]
        public List<Models.Api.Symbol> GetSymbols([FromUri] string text = null)
        {
            var symbolFiles = ConvertRegister(_symbolService.GetSymbols(text));

            return symbolFiles;
        }

        /// <summary>
        /// Get files for symbol
        /// </summary>
        [System.Web.Http.Route("api/symbol/{uuid}")]
        [System.Web.Http.HttpGet]
        public List<Models.Api.Symbol> GetSymbol(string uuid)
        {
            var symbol = _symbolService.GetSymbol(Guid.Parse(uuid));
            List<Models.Symbol> symbolList = new List<Models.Symbol>();
            symbolList.Add(symbol);
            var symbolFiles = ConvertRegister(symbolList);

            return symbolFiles;
        }

        private List<Models.Api.Symbol> ConvertRegister(List<Models.Symbol> symbolFiles)
        {
            var symbolList = new List<Models.Api.Symbol>();
            foreach (var symbol in symbolFiles)
            {
                foreach(var symbolFile in symbol.SymbolFiles)
                { 
                    var file = new Models.Api.Symbol();
                    file.SymbolUuid = symbol.SystemId.ToString();
                    file.SymbolName = symbol.Name;
                    file.Owner = symbol.Owner;
                    file.Theme = symbol.Theme;
                    file.PreviewImageUrl = symbol.ThumbnailUrl();
                    file.PackageUuid = (symbol.SymbolPackages != null && symbol.SymbolPackages.Count > 0 ? symbol.SymbolPackages[0].SystemId.ToString() : "");
                    file.PackageName = (symbol.SymbolPackages != null && symbol.SymbolPackages.Count > 0 ? symbol.SymbolPackages[0].Name.ToString() : "");
                    file.FileName = symbolFile.FileName;
                    file.FileUrl = symbolFile.FileUrl();
                    file.SymbolType = symbolFile.Type;
                    file.Format = symbolFile.Format;
                    file.Variant = (symbolFile.SymbolFileVariant != null ? symbolFile.SymbolFileVariant.Name : "");
                    file.Color = symbolFile.Color;
                    file.Size = symbolFile.Size;

                    symbolList.Add(file);
                }
            }

            return symbolList;
        }

        /// <summary>
        ///     Creates new symbol package
        /// </summary>
        /// <param name="package">SymbolPackage model</param>
        /// <returns>
        ///     SymbolPackage model
        /// </returns>
        /// <response code="500">Internal Server Error</response>
        [ApiExplorerSettings(IgnoreApi = true)]
        [System.Web.Http.Route("api/addpackage")]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Authorize]
        [ResponseType(typeof(SymbolPackage))]
        public HttpResponseMessage PostPackage(SymbolPackage package)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _symbolService.AddPackage(package));
            }
            catch (PackageException pex)
            {
                Log.Error("Error API", pex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, new HttpError(pex.Message));
            }
            catch (Exception ex)
            {
                Log.Error("Error API", ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, new HttpError("Kunne ikke legge til pakke"));
            }
        }
    }
}
