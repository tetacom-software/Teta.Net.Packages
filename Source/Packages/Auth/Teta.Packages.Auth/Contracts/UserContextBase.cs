using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;
using Teta.Packages.Auth.Interfaces;

namespace Teta.Packages.Auth.Contracts
{
    /// <summary>
    /// Base class for user context
    /// </summary>
    public abstract class UserContextBase : IUserContext
    {
        /// <summary>
        /// Context accessor instance
        /// </summary>
        protected IHttpContextAccessor Accessor { get; private set; }

        /// <summary>
        /// Principal
        /// </summary>
        protected ClaimsPrincipal ClaimsPrincipal => Accessor.HttpContext?.User;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="accessor">Http context accessor</param>
        /// <exception cref="ArgumentNullException"></exception>
        public UserContextBase(IHttpContextAccessor accessor)
        {
            Accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
        }

        /// <summary>
        /// User Identity server unique id
        /// </summary>
        public string Sid
        {
            get
            {
                return ClaimsPrincipal.Claims
                    .FirstOrDefault(c => c.Type == KcClaimTypesConstants.Sid)?.Value;
            }
        }

        /// <summary>
        /// Request bearer token
        /// </summary>
        public string Token
        {
            get
            {
                var headerVal = new StringValues(string.Empty);
                if (Accessor.HttpContext?.Request.Headers.TryGetValue("Authorization", out headerVal) ?? false)
                {
                    return headerVal;
                }

                return headerVal;
            }
        }

        /// <summary>
        /// Return true if anonymous user
        /// </summary>
        public bool IsAnonymous
        {
            get
            {
                if (ClaimsPrincipal?.Identity is ClaimsIdentity 
                    && ClaimsPrincipal.Identity.IsAuthenticated)
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Preferred user name
        /// </summary>
        public string PreferredUserName
        {
            get
            {
                return ClaimsPrincipal.Claims
                        .FirstOrDefault(c => 
                        c.Type == KcClaimTypesConstants.PreferredUserName)?.Value;
            }
        }

        /// <summary>
        /// Given name
        /// </summary>
        public string GivenName
        {
            get
            {
                return ClaimsPrincipal.Claims
                        .FirstOrDefault(c =>
                        c.Type == KcClaimTypesConstants.GivenName)?.Value;
            }
        }

        /// <summary>
        /// Email
        /// </summary>
        public string Email
        {
            get
            {
                return ClaimsPrincipal.Claims
                    .FirstOrDefault(c =>
                        c.Type == KcClaimTypesConstants.Email)?.Value;
            }
        }

        /// <inheritdoc/>
        public bool IsSuperUser => ClaimsPrincipal.HasRole(TetaDefaultRoles.SuperUserRole);

        /// <inheritdoc />
        public IEnumerable<int> CompanyIds {
            get
            {
                return ClaimsPrincipal.Claims
                    .Where(c => c.Type == KcClaimTypesConstants.Company).Select(c=>Convert.ToInt32(c.Value));
            }
        }
    }
}
