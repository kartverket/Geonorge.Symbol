using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Geonorge.Symbol.Services
{
    public class FileException : Exception
    {
        public FileException(string message) : base(message)
        {
        }
    }
}