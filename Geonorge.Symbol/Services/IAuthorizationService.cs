using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Geonorge.Symbol.Services
{
    public interface IAuthorizationService
    {
        bool IsAdmin();
        bool IsOwner(string owner, string user);
        bool HasAccess(string owner, string user);
    }
}