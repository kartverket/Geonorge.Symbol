using Geonorge.AuthLib.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace Geonorge.Symbol.Services
{
    public class AuthorizationService : IAuthorizationService
    {

        private ClaimsPrincipal _claimsPrincipal;

        public AuthorizationService(ClaimsPrincipal claimsPrincipal)
        {
            _claimsPrincipal = claimsPrincipal;
        }

        public bool HasAccess(string owner, string user)
        {
            return (IsAdmin() || IsOwner(owner, user));
        }

        public virtual bool IsAdmin()
        {
            return ClaimsPrincipal.Current.IsInRole(GeonorgeRoles.MetadataAdmin);
        }

        public bool IsOwner(string owner, string user)
        {
            return (!string.IsNullOrEmpty(owner) && !string.IsNullOrEmpty(user)) && (owner.ToLower() == user.ToLower());
        }
    }
}