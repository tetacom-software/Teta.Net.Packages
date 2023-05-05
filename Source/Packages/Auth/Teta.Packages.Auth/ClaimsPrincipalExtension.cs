using System.Security.Claims;
using Teta.Packages.Auth.Contracts;

namespace Teta.Packages.Auth
{
    /// <summary>
    /// Extestions for HttpContext
    /// </summary>
    public static class ClaimsPrincipalExtension
    {
        /// <summary>
        /// Has tole code
        /// </summary>
        /// <param name="principal">Principal entity</param>
        /// <returns></returns>
        public static bool HasRole(this ClaimsPrincipal principal, string roleCode)
        {
            return principal.Claims.Any(c => c.Type == KcClaimTypesConstants.RoleType
                                 && c.Value == roleCode);
        }
    }
}
