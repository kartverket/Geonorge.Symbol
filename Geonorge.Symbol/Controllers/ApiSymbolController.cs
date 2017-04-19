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
        ///     Creates new symbol package
        /// </summary>
        /// <param name="package">SymbolPackage model</param>
        /// <returns>
        ///     SymbolPackage model
        /// </returns>
        /// <response code="500">Internal Server Error</response>
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
            catch (Exception ex)
            {
                Log.Error("Error API", ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, new HttpError("Kunne ikke legge til pakke"));
            }
        }
    }
}
