using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Geonorge.Symbol.Services
{
    public class PackageException : Exception
    {
        public PackageException(string message) : base(message)
        {
        }
    }
}