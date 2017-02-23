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
            List<string> roles = GetSecurityClaim("role");
            foreach (string role in roles)
            {
                if (role == "nd.metadata_admin")
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsOwner(string owner, string user)
        {
            return (!string.IsNullOrEmpty(owner) && !string.IsNullOrEmpty(user)) && (owner.ToLower() == user.ToLower());
        }

        public List<string> GetSecurityClaim(string type)
        {
            List<string> result = new List<string>();
            foreach (var claim in ClaimsPrincipal.Current.Claims)
            {
                if (claim.Type == type && !string.IsNullOrWhiteSpace(claim.Value))
                {
                    result.Add(claim.Value);
                }
            }

            if (result.Count == 0 && type.Equals("organization") && result.Equals("Statens kartverk"))
            {
                result.Add("Kartverket");
            }

            return result;
        }
    }
}