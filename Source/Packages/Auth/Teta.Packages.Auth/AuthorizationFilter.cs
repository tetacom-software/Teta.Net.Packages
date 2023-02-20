// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorizationFilter.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Filter implements user claim check logic
// </summary>

using System.Diagnostics.CodeAnalysis;
using System.Security.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Teta.Packages.Auth.Contracts;
using Teta.Rir23.Auth.Interfaces;

namespace Teta.Packages.Auth
{
    /// <summary>
    /// Filter implements user claim check logic
    /// </summary>
    public class AuthorizationFilter : IAsyncAuthorizationFilter
    {
        private readonly string[] _claims;
        private readonly IAuthLogic _authImplementation;

        /// <summary>
        /// Initilize new instance of class <see cref="AuthorizationFilter"/>.
        /// </summary>
        /// <param name="claims">User claims</param>
        /// <param name="authImplementation">Implementation of auth validation check</param>
        public AuthorizationFilter(
            [NotNull] string[] claims,
            [NotNull] IAuthLogic authImplementation)
        {
            _claims = claims ?? throw new ArgumentNullException(nameof(claims));
            _authImplementation = authImplementation ?? throw new ArgumentNullException(nameof(authImplementation));
        }

        /// <summary>
        /// Called early in the filter pipeline to confirm request is authorized.
        /// </summary>
        /// <param name="context">The <see cref="T:Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext" />.</param>
        /// <returns>
        /// A <see cref="T:System.Threading.Tasks.Task" /> that on completion indicates the filter has executed.
        /// </returns>
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            await Task.Factory.StartNew(() =>
            {
                var endpointFeature = context.HttpContext.Features.Get<IEndpointFeature>();
                var metadata = endpointFeature?.Endpoint?.Metadata;
                var allowAnonymous = metadata?.GetMetadata<AllowAnonymousAttribute>();
                if (allowAnonymous != null)
                {
                    return;
                }

                var user = context.HttpContext.User;
                if (user.Identity!.IsAuthenticated)
                {
                    if (_claims.Length == 0)
                    {
                        throw new AuthenticationException($"Method [{context.HttpContext.Request.Path}] has authentication attribute but does not have claims requirements, use AllowAnonymousAttribute explicitly");
                    }

                    var isSu = user.HasRole(TetaDefaultRoles.SuperUserRole);
                        
                    var checkClaimsResult = _authImplementation.CheckAccess(user, context, _claims, isSu);
                    if (checkClaimsResult)
                    {
                        return;
                    }

                    context.Result = new ForbidResult();
                }
                else
                {
                    context.Result = new UnauthorizedResult();
                }
            });
        }

        /*
        private bool CheckHasClaims(ClaimsPrincipal claimsPrincipal, string[] claims, AuthorizationFilterContext context)
        {
            if (claimsPrincipal.Claims.Any(c => c.Type == TetaClaimTypes.TetaRoleClaimType && c.Value.Contains(TetaRoleCodes.SystemAdmin)))
            {
                return true;
            }

            foreach (var claim in claims)
            {
                // Контроллер допускает доступ базового пользователя без прав
                if (claim == TetaRoleCodes.RegistratedUser
                    && claimsPrincipal.Claims.Any(c => c.Type == TetaClaimTypes.TetaRoleClaimType && c.Value.Contains(claim)))
                {
                    return true;
                }

                if (context.RouteData.Values.ContainsKey(WebConstants.CompanyRouteKey))
                {
                    var companyId = context.HttpContext?.Request?.GetRouteValue(WebConstants.CompanyRouteKey, null);

                    // Выполнить проверку для случая, когда в роуте имеется идентификатор компании
                    if (claimsPrincipal.Claims.Any(c => c.Value == TetaClaimTypes.Compile(companyId, claim)))
                    {
                        return true;
                    }
                }
                else
                {
                    // Иначе выполнить проверку безотносительно компании
                    if (claimsPrincipal.Claims.Any(c => c.Value.Contains(claim)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }*/
    }
}
