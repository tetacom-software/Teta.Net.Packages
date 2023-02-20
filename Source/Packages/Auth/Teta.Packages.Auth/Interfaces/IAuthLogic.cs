// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAuthLogic.cs" company="TETA">
// Copyright (c) TETA. Ufa, 2023.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
// <summary>
//  Claims principal check interface
// </summary>

using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Teta.Packages.Auth.Interfaces
{
    /// <summary>
    /// Claims principal check interface
    /// </summary>
    public interface IAuthLogic
    {
        /// <summary>
        /// Claims principal check
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="context">Context</param>
        /// <param name="isSu">User has default tetacom SU flag</param>
        /// <returns>true if validation passes</returns>
        bool CheckAccess(ClaimsPrincipal user, AuthorizationFilterContext context, string[] requiredClaims, bool isSu = false);
    }
}
